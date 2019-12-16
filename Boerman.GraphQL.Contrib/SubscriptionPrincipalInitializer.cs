using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Boerman.GraphQL.Contrib
{
    public class SubscriptionPrincipalInitializer : IOperationMessageListener
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAuthorizationService _authorizationService;
        private readonly JwtValidator _jwtValidator;

        public SubscriptionPrincipalInitializer(
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService,
            JwtValidator jwtValidator)
        {
            _contextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
            _jwtValidator = jwtValidator;
        }

        public async Task BeforeHandleAsync(MessageHandlingContext context)
        {
            if (context.Terminated) return;

            var message = context.Message;

            if (message.Type == MessageType.GQL_CONNECTION_INIT)
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

                string token = ((string)((dynamic)message.Payload)
                    ?.Authorization)
                    ?.Replace("Bearer ", "");

                if (token == null) return;

                var user = _jwtValidator.ValidateJwt(token);

                _contextAccessor.HttpContext.User = user;
            }
        }

        public Task HandleAsync(MessageHandlingContext context)
        {
            return Task.CompletedTask;
        }

        public Task AfterHandleAsync(MessageHandlingContext context)
        {
            return Task.CompletedTask;
        }
    }
}
