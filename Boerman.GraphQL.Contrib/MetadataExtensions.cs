using GraphQL.Builders;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;

namespace SkyHop.Hosting.Graph.Modules.Authorization
{
    public static class MetadataExtensions
    {
        public const string PolicyKey = "__auth-policies";

        public static bool RequiresAuthorization(this IProvideMetadata type)
        {
            var policy = type.GetMetadata<AuthorizationPolicy>(PolicyKey);

            return policy != null && policy.Requirements.Any();
        }

        public static void WithPolicy(
            this IProvideMetadata type,
            Action<AuthorizationPolicyBuilder> policy)
        {
            var policyBuilder = new AuthorizationPolicyBuilder();

            policy.Invoke(policyBuilder);

            var authorizationPolicy = type.GetMetadata(PolicyKey, policyBuilder.Build());

            type.Metadata[PolicyKey] = authorizationPolicy;
        }

        public static FieldBuilder<TSourceType, TReturnType> WithPolicy<TSourceType, TReturnType>(
            this FieldBuilder<TSourceType, TReturnType> builder,
            Action<AuthorizationPolicyBuilder> policy)
        {
            builder.FieldType.WithPolicy(policy);
            
            return builder;
        }

        public static ConnectionBuilder<TSourceType> WithPolicy<TSourceType>(
            this ConnectionBuilder<TSourceType> builder,
            Action<AuthorizationPolicyBuilder> policy)
        {
            builder.FieldType.WithPolicy(policy);

            return builder;
        }

        public static AuthorizationPolicy GetPolicy(this IProvideMetadata type) => type.GetMetadata<AuthorizationPolicy>(PolicyKey);
    }
}
