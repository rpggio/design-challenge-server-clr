using System;
using System.Linq;
using DCS.Core;
using DCS.ServerRuntime.Entities;
using DCS.ServerRuntime.Services;
using Nancy;

namespace DCS.WebServices.Api
{
    public class UsersEndpoint : NancyModule
    {
        public UsersEndpoint(Users users, Commits commits, AppSettings settings, DcsScm scm)
            : base("/users")
        {
            Get["/"] = _ =>
            {
                string token = Request.Query.token;
                if (token.ToNullableGuid() != FakeyAuth.Token)
                {
                    return new Response
                    {
                        StatusCode = HttpStatusCode.Forbidden
                    };
                }

                return users.GetAll()
                    .Select(u =>
                    {
                        var user = new
                        {
                            Username = u.Username,
                            u.Id
                        };

                        return user;
                    }
                    );
            };

            Get["/{userId}"] = _ =>
            {
                var user = users.Get((Guid) _.userId);
                if (user == null)
                {
                    return new NotFoundResponse();
                }
                var repo = user.Repositories.FirstOrDefault();
                return new
                {
                    Username = user.Username,
                    user.Id,
                    user.Email,
                    RepositoryHref = repo == null ? null : scm.GetRepoUrl(repo.Name, user, true),
                    Challenges = user.Challenges.Select(c =>
                    new {
                        c.Name,
                        c.StageNumber,
                        c.RepositoryId
                    })
                };
            };

            Get["/{userId}/commits"] = _ =>
                commits.ForUser((Guid) _.userId)
                    .OrderByDescending(c => c.CommittedAt)
                    .Select(c =>
                        new
                        {
                            c.Id,
                            c.CommittedAt,
                            c.UserId,
                            c.Username,
                            c.RepositoryId,
                            c.RepositoryName,
                            c.Challenge,
                            c.CurrentStageNumber,
                            c.ResultsUpdatedAt,
                            c.Outcome,
                            c.OutcomeDetail,
                            HasTestOutput = !string.IsNullOrEmpty(c.TestOutput)
                        });
        }
    }
}