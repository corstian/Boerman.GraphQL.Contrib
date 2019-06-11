using System;

namespace Boerman.GraphQL.Contrib
{
    [Obsolete("Using this interface makes maintenance harder, and should not be a requirement anymore to use the `.ToConnection` extension method.")]
    public interface IId
    {
        Guid Id { get; set; }
    }
}
