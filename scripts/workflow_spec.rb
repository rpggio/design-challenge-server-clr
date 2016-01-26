# bowling_spec.rb
require_relative 'common'

class Dcs

    #todo: move 'common' methods into here
    #todo: support different users/repos within test run

end

describe Dcs do
  it "will build and test user solutions" do
      setupChallenge
      (1..4).each do |stageNum|
          pushRefSolutionForStage("stage%03d" % stageNum)
          waitForUserAdvancedMessage("stage%03d" % (stageNum + 1))
      end
  end
end
