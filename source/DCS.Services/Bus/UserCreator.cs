using System;
using System.IO;
using DCS.Contracts;
using DCS.Contracts.Entities;
using DCS.Core;
using DCS.ServerRuntime.Entities;
using DCS.ServerRuntime.Services.GitblitApi;
using Rebus;

namespace DCS.Services.Bus
{
    public class UserCreator : IHandleMessages<CreateUser>
    {
        private readonly Users _users;
        private readonly IBus _bus;
        private readonly GitblitClient _gitblitClient;

        public UserCreator(Users users, IBus bus, GitblitClient gitblitClient)
        {
            _users = users;
            _bus = bus;
            _gitblitClient = gitblitClient;
        }

        public void Handle(CreateUser createUser)
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Username = createUser.Username,
                Email = createUser.Email,
                Password = createUser.Password,
                IsTestUser = createUser.IsTestUser
            };
            _users.Save(user);

            _gitblitClient.CreateUser(
                new GitblitUser()
                {
                    username = user.Username,
                    password = user.Password,
                    displayName = user.Username,
                    emailAddress = user.Email,
                    canAdmin = false,
                    canFork = false,
                    canCreate = false,
                    disabled = false,
                    accountType = "LOCAL"
                });

            // Temporary: automatically initialize with challenge and specific solution starter

            const string challengeName = "GateScheduler";
            const string starterName = "cs-nancy";
            var repoName = string.Format("{0}-{1}", challengeName, 
                Path.GetFileNameWithoutExtension(Path.GetTempFileName()));
            _bus.Publish(new InitializeChallenge()
            {
                ChallengeName = challengeName,
                Username = user.Username,
                Repository = repoName,
                StarterName = starterName
            });
        }
    }
}