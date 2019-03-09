using System;

namespace Boerman.GraphQL.Contrib
{
    public static class Cursor
    {
        public static string ToCursor(this Guid guid) => Convert.ToBase64String(guid.ToByteArray());
        public static Guid FromCursor(this string base64Guid) => new Guid(Convert.FromBase64String(base64Guid));
    }
}
