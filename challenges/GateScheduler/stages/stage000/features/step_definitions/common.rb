
Given(/^the system is reset$/) do
  resp = put('/status', { :state => 'reset' })
  expect(resp.status).to eq(200)
end

When(/^the client adds (\w+):$/) do |resourceName, resources|
  # resources is a Cucumber::Ast::Table
  url = '/' + resourceName
  post(url, resources.hashes)
end

When(/^the client requests ([\w\/]+)$/) do |resourceName|
  resp = get("/" + resourceName, nil)
  @priorRequest = resp.body
end

Then(/^the response is:$/) do |table|
    actual = trimEmpties(@priorRequest)
    expected = trimEmpties(table.hashes)
    diff = Cucumber::Ast::Table.new(actual)
    expect(actual).to match(expected), "--- The response was incorrect: --- #{diff.to_s(:color => false)}"
end

def trimEmpties(listOfHashes)
    listOfHashes.each do |hash|
        hash.delete_if { |k,v| v.nil? || (v.is_a?(String) && v.length == 0) }
    end
end