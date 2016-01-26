using DCS.ServerRuntime.Entities;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Nancy.Authentication.Token;

namespace DCS.WebServices.Api
{
    public class AuthModule : NancyModule
    {
        public AuthModule(ITokenizer tokenizer, Users users)
            : base("/auth")
        {
            Post["/"] = x =>
            {
                var request = this.Bind<AuthRequest>();

                if (string.IsNullOrEmpty(request.Identifier))
                {
                    return HttpStatusCode.Unauthorized;
                }
                var user = users.ValidateUsernameOrEmail(request.Identifier, request.Password);
                
                if (user == null)
                {
                    return HttpStatusCode.Unauthorized;
                }

                var userIdentity = new UserIdentity(user);
                var token = tokenizer.Tokenize(userIdentity, Context);

                return new
                {
                    Token = token,
                    Username = user.Username,
                    UserId = user.Id
                };
            };

            Get["/"] = _ =>
            {
                this.RequiresAuthentication();
                return Context.CurrentUser;
            };
        }
    }

    public class AuthRequest
    {
        public string Identifier { get; set; }
        public string Password { get; set; }
    }
}
