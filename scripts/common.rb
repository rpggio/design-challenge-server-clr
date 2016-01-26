
require 'logger'
require 'json'
require 'faraday'
require 'faraday_middleware'
require 'pathname'
require 'net/ping'
require 'pathname'
require 'fileutils'

# configHash = JSON.load(open('rake-config.json'))
# config = Hashie::Mash.new(configHash)

class AppConfig < Hash
    def initialize(configFile, params)
        require_relative configFile
        appConfig = ENV['dcs-env'] == 'staging' ? appConfigStaging : appConfigLocal
        puts
        puts "Environment target: #{appConfig[:name]}"
        puts
        
        self.update(appConfig)
        self.update(params)
    end

      def method_missing(m, *args, &block)  
          i = 0
          result = self[m]
          return nil if result.nil?
            loop do
                lastResult = result
                result = result % self
                break if (i >= 9) || result.nil? || result == lastResult
                i += 1
            end
            result
      end  

    def to_s
        self.map { |k,v| "#{k}: #{self.send(k)}" }
            .join(', ')            
    end
end

def sendGitblitUser(method)
    config = $config
    puts "-- sending gitblit user #{config.username} --"

    body = {
        username: config.username,
        password: config.password,
        displayName: config.username,
        emailAddress: config.userEmail,
        canAdmin: false,
        canFork: false,
        canCreate: false,
        excludeFromFederation: false,
        disabled: false,
        repositories: [config.repoPath],
        permissions: {
            config.repoPath => "RW"
        },
        teams: [],
        isAuthenticated: true,
        accountType: "LOCAL",
        userPreferences: {
            username: config.username,
            emailMeOnMyTicketChanges: true,
            repositoryPreferences: {
            }
        }
    }
    methodName = {
        create: 'CREATE_USER',
        update: 'EDIT_USER'
    }[method]
    
    sendGitBlitRpc(methodName, body)
end

def sendGitBlitRpc(method, body)
  conn = createRestClient($config.gitblitRpcUrl)
  conn.post("/rpc?req=#{method}", body)
end

def deleteRepositories(pattern)
  repos = sendGitBlitRpc('LIST_REPOSITORIES', nil)
  patternRegexp = Regexp.new(pattern)
  repos.body.each do |key, repo|
    repoName = repo['name']
    if patternRegexp =~ repoName
      puts "deleting #{repoName}"
      sendGitBlitRpc('DELETE_REPOSITORY', { name: repoName })
    end
  end
end 

def createRestClient(url)
    Faraday::Connection.new(
      :url => url,
      :headers => { :accept =>  'application/json' },
      :ssl => {:verify => false}
    ) do |c|
        c.request :json
        c.response :json
        c.adapter  Faraday.default_adapter
      begin
        if Net::Ping::TCP.new('127.0.0.1', 8888, 0.3).ping?
          c.proxy 'http://127.0.0.1:8888'
        end
      rescue
      end
        c.basic_auth($config.adminUsername, $config.adminPassword)
    end
end

def windir(path)
    return path.gsub('/', '\\')
end

def sh(command)
    command = command
    puts "[ #{command} ]"
    result = `#{command}`
    puts result
end


def areChangesPending
  sh %{ git diff --exit-code --quiet }
  return true if $? != 0

  sh %{ git diff --cached --exit-code --quiet }
  return $? != 0
end

def commitAndPushIfNeeded(message)
  sh %{ git add --ignore-removal . }
   
   if areChangesPending()    
      sh %{ git commit -q -a -m #{message} }
      sh %{ git push -q }
   else
      puts "no changes to commit"
   end
end

def resetToRemote(repoDir)
  Dir.chdir(windir(repoDir)) do
    throw "must be run somewhere under /dcs-test, pwd is #{Dir.pwd}" unless Dir.pwd =~ /^c:\/dcs\-test\//i
    sh %{ git clean -df }
    sh %{ git checkout -- . }
    sh %{ git reset --hard HEAD }
    sh %{ git pull --rebase }
  end
end

def createRepo()
  puts
  puts "-- creating user repo -- "
  repoUrl = "#{$config.transport}://#{$config.adminUsername}:#{$config.adminPassword}@#{$config.gitblitServer}/r/#{$config.repoPath}" 
  repoDir = File.join($config.adminReposDir, $config.username, $config.repoName)
  throw 'repository directory already exists' if File.exists?(repoDir)
  sh %{ mkdir #{windir(repoDir)} }
  Dir.chdir(windir(repoDir)) do
      sh %{ cd }
      sh %{ echo 'readme file' > README.md }
      sh %{ git init }
      sh %{ git config user.name "#{$config.adminUsername}" }
      sh %{ git config user.email "admin@localhost" }
      sh %{ git add . }
      sh %{ git commit -m "genesis" }
      sh %{ git config http.sslVerify false }
      sh %{ git remote add origin #{repoUrl} }
      sh %{ git push -u origin master }
  end
  return repoDir
end

def verifyUserRepo
  puts
  puts "-- verifying user repo --"
  repoUrl = "#{$config.transport}://#{$config.username}:#{$config.password}@#{$config.gitblitServer}/r/#{$config.repoPath}" 
  repoDir = File.join($config.userReposDir, $config.username, $config.repoName)
  sh %{ mkdir #{windir(repoDir)} } unless File.exists?(repoDir)
  sh %{ git clone -c http.sslVerify=false #{repoUrl} #{repoDir} }
  touchAndPushRepo repoDir, "verify"
  return repoDir
end

def touchAndPushRepo(repoDir, message)
  Dir.chdir(windir(repoDir)) do
      sh %{ cd }
      sh %{ echo 'bump' >> README.md }
      sh %{ git config user.name "#{$config.username}" }
      sh %{ git config user.email "#{$config.userEmail}" }
      sh %{ git add . }
      sh %{ git commit -m "#{message}" }
      sh %{ git config http.sslVerify false }
      sh %{ git push origin master }
  end
end

def getEmailMessagesTo(userEmail)
  conn = createRestClient($config.mailServerApiUrl)
  messages = conn.get("/messages").body
  messages.select {|m| 
    m['recipients'].one? { |r| r.downcase().include?(userEmail.downcase()) }
  }
end

def clearEmailMessages()
  conn = createRestClient($config.mailServerApiUrl)
  conn.delete("/messages", {})
end

def waitForMessage(recipient, maxTries, subjectPattern)
  tries = 0
  while(true)
    messages = getEmailMessagesTo(recipient)
    found = messages.select {|m| 
      subject = m['subject']
      subjectPattern =~ subject
    } 
    if(found.length > 0)
      return found
    end

    puts "waiting for message #{subjectPattern}"
    sleep 3 + 1.7 ** tries
    tries += 1
    return [] if(tries >= maxTries)
  end
end

def waitForInitializedMessage
  puts '-- waiting for initialized message --'
  messages = waitForMessage($config.userEmail, 12, /Welcome/);
  throw "Did not receive challenge initialized message" if messages.length == 0
end

def waitForUserAdvancedMessage(stageName)
  puts '-- waiting for user advanced message --'
  messages = waitForMessage($config.userEmail, 12, /You have advanced to #{stageName}/);
  throw "Did not receive advanced message" if messages.length == 0
end

def waitForCompletedAllStagesMessage()
  puts '-- waiting for completed all message --'
  messages = waitForMessage($config.userEmail, 12, /You have completed all of the stages/);
  throw "Did not receive advanced message" if messages.length == 0
end

def initializeChallenge
  puts
  puts '-- initializing challenge --'
  message = {
     challengeName: "GateScheduler",
     starterName: "cs-nancy",
     username: $config.username,
     repository: $config.repoName
  }
  conn = createRestClient($config.dcsApiUrl)
  reponse = conn.post("/messages/?type=InitializeChallenge", message)
end

def createDcsUser
  puts
  puts '-- creating DCS user --'
  message = {
     username: $config.username,
     email: $config.userEmail,
     password: $config.password,
     isTestUser: true,
     notifyUser: true
  }
  conn = createRestClient($config.dcsApiUrl)
  reponse = conn.post("/messages/?type=CreateUser", message)
end


def buildRepo(username, repo)
  puts '-- triggering repo build --'
  message = {
     username: username,
     repository: repo
  }
  conn = createRestClient($config.dcsApiUrl)
  reponse = conn.post("/messages/?type=UserSolutionPush", message)
end

def sendDcsMessage(type, message)
  conn = createRestClient($config.dcsApiUrl)
  reponse = conn.post("/messages/?type=#{type}", message)
end

def getRepoDir()
  File.join($config.userReposDir, $config.username, $config.repoName)
end

def pushRefSolutionForStage(stage)
  puts
  puts "-- pushing solution for stage #{stage} --"
  repoUrl = "#{$config.transport}://#{$config.username}:#{$config.password}@#{$config.gitblitServer}/r/#{$config.repoPath}" 
  repoDir = File.join($config.userReposDir, $config.username, $config.repoName)

  Dir.chdir(windir(repoDir)) do
    resetToRemote(repoDir)

    refSource = Pathname.new($config.stagesDir) + stage + 'solutions' + $config.referenceSolution
    copyContents(refSource, Pathname.new(repoDir))

    sh %{ cd }
    sh %{ git add . }
    sh %{ git commit -m "solution for #{stage}" }
    sh %{ git push origin master }
  end
end

def copyContents(from, to)
  fromPath = File.join(from.to_path(), '.')
  puts "> cp_r #{fromPath} #{to}"
  FileUtils.cp_r(fromPath, to) unless $dryRun
end

def mkpath(*args)
  puts "> mkpath #{args.join(' ')}"
  FileUtils.mkpath *args unless $dryRun
end

def expandSolution(challenge, solution)
    solutionExpandDir = Pathname.new($config.expandDir) + challenge + solution

    stagesDir = Pathname.new($config.stagesDir)

    previousStage = nil
    Pathname.new($config.stagesDir).each_child do |stageDir|
      next unless stageDir.directory?

      stage = stageDir.basename

      stageExpandDir = solutionExpandDir + stage
      mkpath.call stageExpandDir

      featuresExpandDir = stageExpandDir + 'features'
      featuresExpandDir.rmtree if featuresExpandDir.exist?
      
      solutionsExpandDir = stageExpandDir + 'solutions'
      solutionsExpandDir.rmtree if solutionsExpandDir.exist?

      # copy last stage
      unless previousStage.nil?
        copyContents.call(solutionExpandDir + previousStage, stageExpandDir)
      end

      stageSourceDir = stagesDir + stage

      # copy stage solution
      copyContents.call(stageSourceDir + 'solutions' + solution, stageExpandDir)

      featuresSourceDir = stageSourceDir + 'features'
      if featuresSourceDir.exist?      
        # copy stage features
        mkpath.call featuresExpandDir
        copyContents.call(featuresSourceDir, featuresExpandDir)
      end

      previousStage = stage
    end
end

def testCsStage(challenge, stage)
    puts
    puts "testing #{stage}"
    solution = 'cs-nancy'

    stageDir = Pathname.new($config.expandDir) + challenge + solution + stage

    # todo: read build / exe from solution config

    vsSolution = stageDir + 'source/GateScheduler/GateScheduler.csproj'
    sh %{ msbuild #{windir(vsSolution.to_path)} }
    if $? != 0
      puts
      puts "#{stage}: FAIL"
      puts
      return false
    end

    exe = stageDir + 'source/GateScheduler/bin/debug/GateScheduler.exe'
    appProcessId = Process.spawn exe.to_path

    begin
      sh %{ cucumber #{stageDir} }
      if $? != 0
        puts
        puts "#{stage}: FAIL"
        puts
        return false
      end
    ensure
      Process.kill 9, appProcessId
    end

    puts
    puts "#{stage}: PASS"
    puts
    return true
end

def testAllCsStages(challenge)
  solution = 'cs-nancy'
  stagesDir = Pathname.new($config.expandDir) + challenge + solution
  stagesDir.each_child do |stageDir|
    if !testCsStage(challenge, stageDir.basename)
      return
    end
  end
end

def setupChallenge()

  # clearEmailMessages

  sendGitblitUser(:create)

  createRepo()

  userRepo = verifyUserRepo

  createDcsUser

  initializeChallenge

  waitForInitializedMessage

  resetToRemote(userRepo)

  puts "challenge set up at #{userRepo}"

end

def testWorkflow()

  # clearEmailMessages

  setupChallenge

  maxStage = 10;

  (1..maxStage-1).each do |stageNum|
      pushRefSolutionForStage("stage%03d" % stageNum)
      waitForUserAdvancedMessage("Stage #{stageNum + 1}")
  end

  pushRefSolutionForStage("stage%03d" % maxStage)
  waitForCompletedAllStagesMessage();

  puts
  puts '---- DONE! ----'
  puts
end

def createRepoArchive()

  setupChallenge

  sendDcsMessage('CreateRepoArchive', 
    {
       username: $config.username,
       repoName: $config.repoName
    }
  )

end


def testFeaturesAreRefreshed()

  setupChallenge

  repoDir = getRepoDir()

  stage = 'stage001'

  Dir.chdir(windir(repoDir)) do
    resetToRemote(repoDir)

    refSource = Pathname.new($config.stagesDir) + stage + 'solutions' + $config.referenceSolution
    copyContents(refSource, Pathname.new(repoDir))

    gatesFeatureFile = Pathname.new(repoDir) + 'features/gates.feature'
    raise 'no gates file!' unless gatesFeatureFile.exist?
    gatesFeatureFile.open('w') do |f|
      f.puts 'Crash feature crash!!'
    end

    sh %{ cd }
    sh %{ git add . }
    sh %{ git commit -m "solution for #{stage}" }
    sh %{ git push origin master }
  end

  waitForUserAdvancedMessage

  puts '-- done! --'
end

def testRepeatedCheckin
  setupChallenge
  repoDir = getRepoDir()
  
  touchAndPushRepo(repoDir, "touch 2")
  sleep 1
  touchAndPushRepo(repoDir, "touch 3")
  sleep 0.5

  pushRefSolutionForStage("stage001")
end

def sendThrowTest()
  message = {
     message: "Testing exception handling"
  }
  sendDcsMessage('ThrowExceptionMessage', message)
end


def initConfig(suffix)
  suffix = Random.rand(89999) + 10000 if suffix.nil?
  $config = AppConfig.new('dcsa-config', { suffix: suffix })
end

$log = Logger.new(STDOUT)
if defined?(Rails) && (Rails.env == 'development')
  Rails.logger = $log
end

$dryRun = false