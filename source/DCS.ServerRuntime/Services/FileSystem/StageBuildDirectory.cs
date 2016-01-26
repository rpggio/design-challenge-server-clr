using DCS.Core.IO;

namespace DCS.ServerRuntime.Services.FileSystem
{
    public class StageBuildDirectory : PathDirectory
    {
        public StageBuildDirectory(string path) : base(path)
        {
        }

        public IDirectory Features
        {
            get { return this.Directory("features"); }
        }

        public SolutionConfigFile SolutionConfigFile
        {
            get
            {
                return new SolutionConfigFile(this.File("solution-config.json").Path);
            }
        }
    }
}