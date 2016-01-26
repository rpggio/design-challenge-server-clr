using System;
using DCS.Contracts;
using DCS.Core;
using DCS.Core.IO;
using DCS.ServerRuntime.Entities;
using DCS.ServerRuntime.Framework;
using DCS.ServerRuntime.Services.FileSystem;

namespace DCS.ServerRuntime.Services
{
    [RegisterComponent]
    public class DcsScm
    {
        private readonly AppSettings _settings;
        private readonly DcsFilesystem _dcsFiles;
        private readonly Users _users;
        private readonly ScmClient _scm;

        public DcsScm(AppSettings settings, DcsFilesystem dcsFiles, Users users, ScmClient scm)
        {
            _settings = settings;
            _dcsFiles = dcsFiles;
            _users = users;
            _scm = scm;
        }

        // ssh://brock.lee:password@localhost:29418/projects/brocklee-gatescheduler.git
        // ssh://dcs-system:design@localhost:29418/projects/brocklee-gatescheduler.git
        // https://admin:admin@localhost:8443/r/projects/brocklee-gatescheduler.git/
        public string GetRepoUrl(string repo, IScmUser user, bool embedPassword)
        {
            if (!repo.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
            {
                repo += ".git";
            }
            var gitSettings = _settings.Git;
            return string.Format("{0}://{1}{2}@{3}/{4}/{5}",
                gitSettings.Protocol,
                user.Username,
                embedPassword ? ":" + user.Password : null,
                gitSettings.ServerPublic,
                gitSettings.ProjectsPath,
                repo);
        }

        public string GetRepoPath(string repo)
        {
            if (!repo.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
            {
                repo += ".git";
            }
            var gitSettings = _settings.Git;
            return string.Format("{0}/{1}", 
                gitSettings.ProjectsPath, 
                repo);
        }

        public IDirectory CloneRepoAsEndUser(Guid userId, string repoName)
        {
            var userRepoDir = _dcsFiles.GetUserRepo(userId, repoName);
            userRepoDir.Create();
            userRepoDir.DeleteContents();
            var user = _users.Get(userId);
            var repoUrl = GetRepoUrl(repoName, user, true);
            _scm.Clone(repoUrl, userRepoDir.Path).RequireTrue();
            _scm.SetUser(userRepoDir.Path, user).RequireTrue();
            _scm.ConfigureSslVerify(userRepoDir.Path, false).RequireTrue();
            return userRepoDir;
        }
    }
}