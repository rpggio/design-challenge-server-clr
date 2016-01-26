using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using DCS.Contracts;
using DCS.Core;
using DCS.Core.Validation;
using DCS.ServerRuntime.Framework;
using System.ComponentModel.DataAnnotations;

namespace DCS.ServerRuntime.Services
{
    [RegisterComponent(RegisterWith.SingleInstance)]
    public class AppSettings
    {
        public readonly ServerSettings Server;
        public readonly GitSettings Git;
        public readonly EnvSettings Env;
        public readonly SmtpSettings Smtp;
        private readonly NameValueCollection _settings;

        public AppSettings()
        {
            _settings = new NameValueCollection(ConfigurationManager.AppSettings);

            var missing = Values.FirstOrDefault(kvp => kvp.Value.ToUpper().Trim() == "#TBD");
            if(missing.Key != null)
            {
                throw new ConfigurationErrorsException(
                    "Missing required configuration value {0}".FormatFrom(missing.Key));
            }

            _settings = _settings.ExpandTokens();

            Server = new ServerSettings(_settings);
            Git = new GitSettings(_settings);
            Env = new EnvSettings(_settings);
            Smtp = new SmtpSettings(_settings);
        }

        public IEnumerable<KeyValuePair<string, string>> Values
        {
            get
            {
                return _settings.AllKeys
                    .Select(k => new KeyValuePair<string, string>(k, _settings[k]));
            }
        }

        public class ServerSettings
        {
            private readonly NameValueCollection _settings;

            public ServerSettings(NameValueCollection settings)
            {
                _settings = settings;
            }
            
            public string EmailFrom
            {
                get { return _settings.GetRequiredValue("server.email-from"); }
            }
        }

        public class SmtpSettings
        {
            private readonly NameValueCollection _settings;

            public SmtpSettings(NameValueCollection settings)
            {
                _settings = settings;
            }

            public SmtpConnectionSettings Test
            {
                get { return new SmtpConnectionSettings(
                    _settings.GetRequiredValue("smtp.test.host"),
                    _settings["smtp.test.username"],
                    _settings["smtp.test.password"]
                    ); 
                }
            }

            public SmtpConnectionSettings Real
            {
                get
                {
                    return new SmtpConnectionSettings(
                        _settings.GetRequiredValue("smtp.real.host"),
                        _settings.GetRequiredValue("smtp.real.username"),
                        _settings.GetRequiredValue("smtp.real.password")
                        );
                }
            }
        }

        public class SmtpConnectionSettings
        {
            public SmtpConnectionSettings(string server, string username, string password)
            {
                Server = server;
                Username = username;
                Password = password;
            }

            public string Server { get; private set; }
            public string Username { get; private set; }
            public string Password { get; private set; }
        }

        public class GitSettings
        {
            private readonly NameValueCollection _settings;

            public GitSettings(NameValueCollection settings)
            {
                _settings = settings;
            }

            public ScmUser SystemUser
            {
                get
                {
                    return new ScmUser(_settings.GetRequiredValue("git.system-user.name"),
                        _settings.GetRequiredValue("git.system-user.email"),
                        _settings.GetRequiredValue("git.system-user.password"));
                }
            }

            public ScmUser AdminUser
            {
                get
                {
                    return new ScmUser(_settings.GetRequiredValue("git.admin-user.name"),
                        _settings["git.admin-user.email"],
                        _settings["git.admin-user.password"]);
                }
            }

            public string ServerPrivate
            {
                get { return _settings.GetRequiredValue("git.server.private"); }
            }

            public string ServerPublic
            {
                get { return _settings.GetRequiredValue("git.server.public"); }
            }

            public string ProjectsPath
            {
                get { return _settings.GetRequiredValue("git.projects-path").Trim('/'); }
            }

            public string Protocol
            {
                get { return _settings.GetRequiredValue("git.protocol"); }
            }

            public string RpcUrl
            {
                get { return _settings.GetRequiredValue("git.rpc-url"); }
            }
        }

        public class EnvSettings
        {
            private readonly NameValueCollection _settings;

            public EnvSettings(NameValueCollection settings)
            {
                _settings = settings;
            }

            public void Validate()
            {
                // No longer legit. Need app-specific validation.
                ValidationUtil.AssertIsValid(this);
            }

            public bool IsDcsDbSqlite
            {
                get { return DcsDbConn.ContainsIgnoreCase("sqlite"); }
            }

            public string DcsDbConn
            {
                get
                {
                    return _settings.GetRequiredValue("env.dcs-db-conn");
                }
            }

            public string WebsiteHref
            {
                get { return _settings.GetRequiredValue("env.website-href");  }
            }

            [Required]
            public string BuildsDirectory
            {
                get { return _settings.GetRequiredValue("env.builds-directory"); }
            }

            [Required]
            public string TempDirectory
            {
                get { return _settings.GetRequiredValue("env.temp-directory"); }
            }

            [Required]
            public string BinDirectory
            {
                get { return _settings.GetRequiredValue("env.bin-directory"); }
            }

            [Required]
            public string ExpandDirectory
            {
                get { return _settings.GetRequiredValue("env.expand-directory"); }
            }

            [Required]
            public string UserDownloadsDirectory
            {
                get { return _settings.GetRequiredValue("env.user-downloads-directory"); }
            }

            [Required]
            public string UserReposDirectory
            {
                get { return _settings.GetRequiredValue("env.user-repos-directory"); }
            }

            [Required]
            public string ChallengesSourceDirectory
            {
                get { return _settings.GetRequiredValue("env.challenges-source-directory"); }
            }

            public string HostWebAtAddress
            {
                get { return _settings.GetRequiredValue("env.host-web-at-address"); }
            }

            public string SolutionExecUsername
            {
                get { return _settings.GetRequiredValue("env.solution-exec-username"); }
            }

            public string SolutionExecPassword
            {
                get { return _settings.GetRequiredValue("env.solution-exec-password"); }
            }

            public IReadOnlyCollection<int> SolutionPorts
            {
                get
                {
                    return _settings.GetRequiredValue("env.solution-ports")
                        .Split(',')
                        .SelectMany(unit =>
                        {
                            if (unit.Contains("-"))
                            {
                                var parts = unit.Split('-');
                                int start = int.Parse(parts[0]);
                                int end = int.Parse(parts[1]);
                                if (start == 0 || end == 0 || start > end)
                                {
                                    throw new ConfigurationErrorsException("invalid start or end for port range");
                                }
                                return Enumerable.Range(start, end-start);
                            }
                            return new[] {int.Parse(unit)};
                        })
                        .ToArray();
                }
            }

            [Required]
            public string RubyBin
            {
                get { return _settings.GetRequiredValue("env.ruby-bin"); }
            }

            public string RabbitMqServer
            {
                get { return _settings.GetRequiredValue("env.rabbitmq-server"); }
            }

            public string RabbitMqUsername
            {
                get { return _settings.GetRequiredValue("env.rabbitmq-username"); }
            }

            public string RabbitMqPassword
            {
                get { return _settings.GetRequiredValue("env.rabbitmq-password"); }
            }
        }
    }
}