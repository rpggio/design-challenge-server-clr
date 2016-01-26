using DCS.Contracts;
using DCS.ServerRuntime.Entities;
using Nancy;

namespace DCS.WebServices.Api
{
    public class CommitsEndpoint : NancyModule
    {
        public CommitsEndpoint(Commits commits)
            : base("/commits")
        {
            Get["/{commitId}/testOutput.html"] = _ =>
            {
                var commit = commits.Get((string) _.commitId);
                if (commit == null)
                {
                    return new NotFoundResponse();
                }
                if (string.IsNullOrEmpty(commit.TestOutput))
                {
                    return null;
                }
                return Response.AsText(commit.TestOutput, commit.TestOutputFormat.ToMimeType());
            };

            Get["/{commitId}/buildOutput.txt"] = _ =>
            {
                var commit = commits.Get((string)_.commitId);
                if (commit == null)
                {
                    return new NotFoundResponse();
                }
                return Response.AsText(commit.BuildLog, "text/plain");
            };
        }
    }
}