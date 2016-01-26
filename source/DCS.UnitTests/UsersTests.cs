using System;
using DCS.Contracts;
using DCS.Contracts.Entities;
using DCS.ServerRuntime.Entities;
using Xunit;
using Xunit.Ioc;

namespace DCS.UnitTests
{
    [RunWith(typeof(IocTestClassCommand))]
    [DependencyResolverBootstrapper(typeof(AutofacContainerBootstrapper))]
    public class UsersTests
    {
        private readonly Users _users;
        private readonly Repositories _repositories;

        public UsersTests(Users users, Repositories repositories)
        {
            _users = users;
            _repositories = repositories;
        }

        [Fact]
        public void Can_save_user()
        {
            var username = Any.Name("user");
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = Any.Email(username)
            };
            _users.Save(user);

            var gotUserById = _users.Get(user.Id);
            Assert.Equal(user.Username, gotUserById.Username);
            Assert.Equal(user.Email, gotUserById.Email);

            var gotUserByName = _users.Get(user.Username);
            Assert.Equal(user.Id, gotUserByName.Id);
            Assert.Equal(user.Email, gotUserByName.Email);
        }

        [Fact]
        public void Can_load_user_by_case_insensitive_username()
        {
            var username = Any.Name("user");
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = Any.Email(username)
            };
            _users.Save(user);

            var gotUserByName = _users.Get(user.Username.ToUpperInvariant());
            Assert.NotNull(gotUserByName);
            Assert.Equal(user.Id, gotUserByName.Id);
            Assert.Equal(user.Email, gotUserByName.Email);
        }

        [Fact]
        public void Can_save_userRepository()
        {
            var username = Any.Name("user");
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = Any.Email(username)
            };

            var repo = new RepositoryEntity()
            {
                Id = Guid.NewGuid(),
                Challenge = Any.Name(),
                Name = Any.Name(),
                UserId = user.Id
            };
            user.Repositories.Add(repo);

            _users.Save(user);

            var loadedUser = _users.Get(user.Id);
            Assert.NotEmpty(loadedUser.Repositories);
            var loadedRepo = loadedUser.Repositories[0];
            Assert.Equal(repo.Name, loadedRepo.Name);
        }
    }
}

