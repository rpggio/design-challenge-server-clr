using System;
using System.IO;
using DCS.Core;
using DCS.Core.IO;
using DCS.ServerRuntime.Framework;

namespace DCS.ServerRuntime.Services.FileSystem
{
    [RegisterComponent]
    public class DcsFilesystem
    {
        private readonly AppSettings _settings;

        public DcsFilesystem(AppSettings settings)
        {
            _settings = settings;
        }

        public StageBuildDirectory CreateBuildDir(string prefix)
        {
            string buildDir = Path.Combine(_settings.Env.BuildsDirectory,
                string.Format("{0}_{1}",
                    prefix.Replace("/", "").Replace("\\", ""),
                    Path.GetFileNameWithoutExtension(Path.GetTempFileName())));
            var dir = new StageBuildDirectory(buildDir);
            dir.Create();
            return dir;
        }

        public BuildsDirectory Builds
        {
            get { return new BuildsDirectory(_settings.Env.BuildsDirectory); }
        }

        public ChallengesDirectory ChallengesSource
        {
            get { return new ChallengesDirectory(_settings.Env.ChallengesSourceDirectory); }
        }

        public ChallengesDirectory Expand
        {
            get { return new ChallengesDirectory(_settings.Env.ExpandDirectory); }
        }

        public StageBuildDirectory GetUserRepo(Guid userId, string repoName)
        {
            return new StageBuildDirectory(
                Path.Combine(_settings.Env.UserReposDirectory, 
                userId.Abbreviate().ToUpperInvariant(), 
                repoName.Replace(".git", "")));
        }

        public IDirectory UserDownloads
        {
            get { return new PathDirectory(_settings.Env.UserDownloadsDirectory); }
        }
    }
}