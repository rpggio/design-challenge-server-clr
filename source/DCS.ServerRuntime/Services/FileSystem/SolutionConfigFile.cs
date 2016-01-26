using DCS.Contracts;
using DCS.Core.IO;
using Newtonsoft.Json;

namespace DCS.ServerRuntime.Services.FileSystem
{
    public class SolutionConfigFile : PathFile
    {
        public SolutionConfigFile(string path) : base(path)
        {
        }

        public SolutionSettings Read()
        {
            return JsonConvert.DeserializeObject<SolutionSettings>(
                this.ReadAllText());
        }

        public void Write(SolutionSettings settings)
        {
            this.WriteAllText(JsonConvert.SerializeObject(this));
        }
    }
}