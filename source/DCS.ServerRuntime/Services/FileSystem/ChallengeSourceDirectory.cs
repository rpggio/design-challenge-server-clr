using System.Collections.Generic;
using System.Linq;
using DCS.Contracts;
using DCS.Core.IO;

namespace DCS.ServerRuntime.Services.FileSystem
{
    public class ChallengeSourceDirectory : PathDirectory
    {
        public ChallengeSourceDirectory(string path) : base(path)
        {
        }

        public IEnumerable<StageSourceDirectory> Stages
        {
            get
            {
                return this.Directory("stages").Directories()
                    .Select(d => new StageSourceDirectory(d.Path));
            }
        }

        public IFile Readme 
        {
            get { return this.File("challenge.readme.txt"); }
        }

        public StageSourceDirectory Stage(Stage stage)
        {
            return Stage(stage.Name);
        }

        public StageSourceDirectory Stage(string name)
        {
            return new StageSourceDirectory(this.Directory("stages").Directory(name).Path);
        }
    }
}