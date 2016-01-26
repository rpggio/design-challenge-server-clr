
require_relative 'common'

throw "command required" if ARGV.length < 1

initConfig(nil)

command = ARGV.shift
case command.downcase
    when 'test-stage'
        stage = ARGV.shift
        expandSolution('GateScheduler','cs-nancy')
        testCsStage('GateScheduler',stage)
    when 'test-stages'
        expandSolution('GateScheduler','cs-nancy')
        testAllCsStages('GateScheduler')
    when 'expand-solution'
        expandSolution('GateScheduler','cs-nancy')
    when 'test-workflow'
        testWorkflow()
    when 'test-repeated-checkin'
        testRepeatedCheckin()
    when 'setup-challenge'
        setupChallenge()
    when 'build-repo'
        buildRepo(ARGV.shift, ARGV.shift)
    when 'test-cases'
        testFeaturesAreRefreshed()
    when 'delete-repos'
        deleteRepositories(ARGV.shift)
    else
        send command
end