using System.Text.RegularExpressions;
using DCS.Contracts;
using DCS.Core;
using DCS.Core.IO;
using DCS.ServerRuntime.Entities;
using DCS.ServerRuntime.Services;
using DCS.ServerRuntime.Services.FileSystem;
using log4net;
using Rebus;

namespace DCS.Services.Bus
{
    public class RepoArchiveCreator : IHandleMessages<CreateRepoArchive>
    {
        private readonly DcsFilesystem _filesystem;
        private readonly DcsScm _dcsScm;
        private readonly IBus _bus;
        private readonly ILog _log;
        private readonly Users _users;
        private readonly DcsShell _dcsShell;

        public RepoArchiveCreator(DcsFilesystem filesystem, DcsScm dcsScm, IBus bus, ILog log, Users users,
            DcsShell dcsShell)
        {
            _filesystem = filesystem;
            _dcsScm = dcsScm;
            _bus = bus;
            _log = log;
            _users = users;
            _dcsShell = dcsShell;
        }

        public void Handle(CreateRepoArchive message)
        {
            var user = _users.Get(message.Username);

            if (user == null)
            {
                _log.InfoFormat("Could not find user [{0}]. Ignoring", message.Username);
                return;
            }

            string repoName = Regex.Replace(message.RepoName, @"\.git", "", RegexOptions.IgnoreCase);
            var clonedDir = _dcsScm.CloneRepoAsEndUser(user.Id, repoName);
            var targetArchive = _filesystem.UserDownloads.File(
                "{0}_{1}.zip".FormatFrom(user.Id.Abbreviate().ToUpperInvariant(), repoName));
            if (targetArchive.Exists()) targetArchive.Delete();
            _dcsShell.ZipDirectoryContents(clonedDir, targetArchive);
            _log.InfoFormat("User repository archive created at {0}", targetArchive);
        }
    }
}