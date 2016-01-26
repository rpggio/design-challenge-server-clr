using System;
using System.IO;
using System.Text;
using DCS.Contracts;
using DCS.Core;
using DCS.ServerRuntime.Framework;
using log4net;

namespace DCS.ServerRuntime.Services
{
    [RegisterComponent]
    public class ScmClient
    {
        private readonly Shell _shell;
        private readonly ILog _log;

        public ScmClient(Shell shell, ILog log)
        {
            _shell = shell;
            _log = log;
        }

        public bool Clone(string repoUrl, string targetDir)
        {
            return Git(string.Format("-c http.sslVerify=false clone {0} {1}", repoUrl, targetDir));
        }

        public bool Commit(string dir, string message)
        {
            return Git(string.Format("-c http.sslVerify=false commit -a -m \"{0}\"", message), dir);
        }

        public bool Push(string dir, string origin = "master")
        {
            var builder = new StringBuilder("-c http.sslVerify=false push");
            if (origin != null)
            {
                builder.AppendFormat(" -u origin {0}", origin);
            }
            return Git(builder.ToString(), dir);
        }

        public bool AddNewFiles(string dir)
        {
            return Git("-c http.sslVerify=false add --all .", dir);
        }

        public bool SetUser(string dir, IScmUser user)
        {
            return Git(string.Format("config user.name \"{0}\"", user.Username), dir)
                && Git(string.Format("config user.email \"{0}\"", user.Email), dir);
        }

        public bool ConfigureSslVerify(string dir, bool sslVerify)
        {
            return Git("config http.sslVerify {0}".FormatFrom(sslVerify.ToString().ToLowerInvariant()), dir);
        }

        public bool AddCommitPush(string dir, string message)
        {
            return AddNewFiles(dir)
                   && Commit(dir, message)
                   && Push(dir);
        }

        private bool Git(string command, string workingDirectory = null)
        {
            if (command.StartsWith("git", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Two gits don't make a right");
            }
            var output = new StringWriter();
            bool result = _shell.RunBackground("git", command, workingDirectory, output) == 0;
            if (!result)
            {
                _log.WarnFormat("Git command failed in {0} ({1}): {2}", 
                    workingDirectory, 
                    command, 
                    output.ToString().Left(500));
            }
            return result;
        }

        public bool Init(string dir)
        {
            return Git("init {0}".FormatFrom(dir), dir);
        }

        public bool RemoteOrigin(string dir, string url)
        {
            return Git("remote add origin {0}".FormatFrom(url), dir);
        }
    }
}