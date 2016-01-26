using DCS.Core.IO;

namespace DCS.ServerRuntime.Services.FileSystem
{
    public class BuildsDirectory : PathDirectory
    {
        public BuildsDirectory(string path) : base(path)
        {
        }

        public StageBuildDirectory this[string dirName]
        {
            get { return new StageBuildDirectory(this.Directory(dirName).Path); }
        }
    }
}