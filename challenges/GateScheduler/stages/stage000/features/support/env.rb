require 'rspec'
require 'faraday'
require 'faraday_middleware'
require 'net/ping'

class LogHttpError < Faraday::Response::Middleware
  extend Forwardable
  def_delegators :@logger, :debug, :info, :warn, :error, :fatal

  ClientErrorStatuses = 400...600

  def initialize(app, options = {})
    @app = app
    @logger = options.fetch(:logger) {
      require 'logger'
      ::Logger.new($stdout)
    }
  end

  def call(env)
    @app.call(env).on_complete do
	    case env[:status]
	      when ClientErrorStatuses
	        info "#{env.method} [#{env.url.to_s}] -> [#{env.status}] #{env.body}"
	        # env.response_headers
	    end
    end
  end
end

class FormatConnectionError < Faraday::Middleware
  def call(env)
    begin
      @app.call(env)
    rescue Faraday::Error::ConnectionFailed => e
	  url = env[:url].to_s.gsub(env[:url].path, '')
  	  $stderr.puts "Server is not available at #{url}"
    end
  end
end

module RestClient

  @@testPort = ENV['features_test_port'] || 40000

  url = "http://127.0.0.1:#{@@testPort}"
  raise "Server is not available at #{url}" unless Net::Ping::TCP.new('127.0.0.1', @@testPort, 0.3).ping?
      
	@@conn = Faraday::Connection.new(
	  :url => url,
	  :headers => { :accept =>  'application/json', :user_agent => 'Acceptance tests'}
  ) do |c|
		c.request :json
  		c.response :json
  		c.use LogHttpError
  		c.use FormatConnectionError
      begin
        # show traffic in Fiddler
        if Net::Ping::TCP.new('127.0.0.1', 8888, 0.3).ping?
          c.proxy 'http://127.0.0.1:8888'
        end
      rescue
      end
		c.adapter  Faraday.default_adapter

    status = c.get "#{url}/status"
    raise "Server does not accept request at '/status': #{status}" unless status.status == 200
	end

	def get(path, arg)
		@@conn.get(path, arg)
	end

	def post(path, arg)
		@@conn.post(path, arg)
	end

  def put(path, arg)
    @@conn.put(path, arg)
  end

  def delete(path, arg)
    @@conn.delete(path, arg)
  end

  def patch(path, arg)
    @@conn.patch(path, arg)
  end
end


World(RestClient)