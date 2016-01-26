require_relative 'common'


throw "test suffix parameter required" if ARGV.length < 1 

suffix = ARGV[0]
$config = ScriptConfig.new({ suffix: suffix })

clearEmailMessages

sendGitblitUser($config, :create)

createRepo($config)

verifyUserRepo

initializeChallenge

waitForInitializedMessage

puts '-- done! --'

# todo: port stage promotion test from Powershell
# todo: run stages 1-5

