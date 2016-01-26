using System.Collections.Generic;
using System.Linq;
using DCS.Core.IO;

namespace DCS.ServerRuntime.Services.FileSystem
{
    public class StageSourceDirectory : PathDirectory
    {
        public StageSourceDirectory(string path) : base(path)
        {
        }

        public IDirectory Features
        {
            get { return this.Directory("features"); }
        }

        public IDictionary<string, IDirectory> Solutions
        {
            get { return this.Directory("solutions").Directories()
                .ToDictionary(d => d.Name()); }
        }

        public IFile Readme
        {
            get { return this.File("stage-readme.txt"); }
        }

        /// <summary>
        /// This is for use against _expanded_ stage source.
        /// Copy the files for specified solution into target dir.
        /// Copy the root files and features into target dir.
        /// </summary>
        public void CopySolutionIntoBuildDir(string solution, StageBuildDirectory targetDir)
        {
            var solutionDir = Solutions[solution];
            solutionDir.CopyContentsInto(targetDir);
            this.CopyFilesInfo(targetDir, false);
            Features.CopyContentsInto(targetDir.Features, false);
        }
    }
}