using GraphQL.Server;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace SkyHop.Hosting.Graph.Modules.Authorization
{
    public static class BuilderExtensions
    {
        public static IGraphQLBuilder AddPolicyValidation(this IGraphQLBuilder builder)
        {
            builder.Services
                .AddHttpContextAccessor()
                .AddTransient<IOperationMessageListener, SubscriptionPrincipalInitializer>()
                .AddTransient<IValidationRule, PolicyValidationRule>()
                .AddAuthorization();

            return builder;
        }
    }
}
