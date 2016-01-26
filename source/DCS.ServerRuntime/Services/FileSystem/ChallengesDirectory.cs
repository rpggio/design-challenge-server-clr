using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DCS.Core.IO;

namespace DCS.ServerRuntime.Services.FileSystem
{
    public class ChallengesDirectory : PathDirectory
    {
        public ChallengesDirectory(string path) : base(path)
        {
        }

        public IDictionary<string, ChallengeSourceDirectory> Challenges
        {
            get
            {
                return this.Directories()
                    .Select(d => new ChallengeSourceDirectory(d.Path))
                    .ToDictionary(d => d.Name());
            }
        }
    }
}