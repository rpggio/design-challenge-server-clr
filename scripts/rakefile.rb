
require 'logger'
require 'json'
require 'faraday'
require 'faraday_middleware'

# configHash = JSON.load(open('rake-config.json'))
# config = Hashie::Mash.new(configHash)

class ScriptConfig < Hash
    def initialize(params)
        require_relative 'script-config'
        self.update(scriptConfig)
        self.update(params)
    end

      def method_missing(m, *args, &block)  
          i = 0
          result = self[m]
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
    body = {
        username: $config.username,
        password: $config.password,
        displayName: $config.username,
        emailAddress: $config.userEmail,
        canAdmin: false,
        canFork: false,
        canCreate: false,
        excludeFromFederation: false,
        disabled: false,
        repositories: [$config.repoPath],
        permissions: {
            $config.repoPath => "RW"
        },
        teams: [],
        isAuthenticated: true,
        accountType: "LOCAL",
        userPreferences: {
            username: $config.username,
            emailMeOnMyTicketChanges: true,
            repositoryPreferences: {
            }
        }
    }
    methodName = {
        create: 'CREATE_USER',
        update: 'EDIT_USER'
    }[method]
    conn = createRestClient($config.gitblitRpcUrl)
    reponse = conn.post("/rpc?req=#{methodName}", body)
    #puts response.inspect[0..500]
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
        c.proxy 'http://127.0.0.1:8888'
        c.basic_auth($config.adminUsername, $config.adminPassword)
    end
end

task :default => [:loggerSetup] do

    puts 'done'	 
end

task :loggerSetup do
	$log = Logger.new(STDOUT)
	if defined?(Rails) && (Rails.env == 'development')
	  Rails.logger = $log
	end
end

def configure(suffix)
    raise 'suffix argument is required' if suffix.nil?
    return ScriptConfig.new({ suffix: suffix })
end

task :createRepo, [:repoName] do |t, args|
  sh %{ echo 'readme file' > README.md }
  sh %{ git init }
  sh %{ git add . }
  sh %{ git commit -m "genesis" }
  sh %{ git remote add origin #{config.repoUrl} }
  sh %{ git push -u origin master }
end


$config = configure('04')

sendGitblitUser(:create)
