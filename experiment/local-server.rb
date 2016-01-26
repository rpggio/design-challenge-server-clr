require 'json'
require 'sinatra'
require 'multi_json'

set :port, 12345

@@gates = []

get '/gates' do
	content_type :json
	@@gates.to_json
end

post '/gates' do
	@@gates = JSON.parse(request.body.read)
	return
end

# class App <  Sinatra::Application
#   configure do
#     # Don't log them. We'll do that ourself
#     set :dump_errors, false
 
#     # Don't capture any errors. Throw them up the stack
#     set :raise_errors, true
 
#     # Disable internal middleware for presenting errors
#     # as useful HTML pages
#     set :show_exceptions, false
#   end
# end
 
# class ExceptionHandling
#   def initialize(app)
#     @app = app
#   end
 
#   def call(env)
#     begin
#       @app.call env
#     rescue => ex
#       env['rack.errors'].puts ex
#       env['rack.errors'].puts ex.backtrace.join("\n")
#       env['rack.errors'].flush
 
#       hash = { :message => ex.to_s }
#       #hash[:backtrace] ex.backtrace if RACK_ENV['development']
 
#       [500, {'Content-Type' => 'application/json'}, [Json.dump(hash)]]
#     end
#   end
# end
 
# use ExceptionHandling
# run App