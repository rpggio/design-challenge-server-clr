using DCS.Contracts;
using DCS.Core.IO;
using DCS.ServerRuntime.Services.FileSystem;
using Nancy;

namespace DCS.WebServices.Api
{
    public class ChallengesEndpoint : NancyModule
    {
        public ChallengesEndpoint(DcsFilesystem dcsFiles)
            : base("/challenges")
        {
            Get["/{name}/stages/{number}/readme.txt"] = _ =>
            {
                string name = _.name;
                int number = _.number;
                var readmeFile = dcsFiles.ChallengesSource
                    .Challenges[name]
                    .Stage(new Stage(number))
                    .Readme;
                if (!readmeFile.Exists())
                {
                    return new NotFoundResponse();
                }
                return Response.AsText(readmeFile.ReadAllText(), "text/plain");
            };

            Get["/{name}/readme.txt"] = _ =>
            {
                string name = _.name;
                var readmeFile = dcsFiles.ChallengesSource.Challenges[name].Readme;
                if (!readmeFile.Exists())
                {
                    return new NotFoundResponse();
                }
                return Response.AsText(readmeFile.ReadAllText(), "text/plain");
                //using (var stream = readmeFile.OpenRead())
                //{
                //    return Response.FromStream(stream, "text/plain");
                //}
            };
        }
    }
}