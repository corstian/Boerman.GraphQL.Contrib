using System;
using System.Text;

namespace Boerman.GraphQL.Contrib
{
    public static class Cursor
    {
        // GUID //

        /// <summary>
        /// Convert a guid to a cursor (base64 string).
        /// </summary>
        /// <param name="guid">The guid to transform</param>
        /// <returns>A cursor</returns>
        public static string ToCursor(this Guid guid) => Convert.ToBase64String(guid.ToByteArray());

        /// <summary>
        /// Converts a cursor (string) to it's GUID representation.
        /// </summary>
        /// <param name="base64">A base64 string</param>
        /// <returns>A guid</returns>
        public static Guid FromCursorToGuid(this string base64) => new Guid(Convert.FromBase64String(base64));

        /// <summary>
        /// Converts a cursor (base64 string) to it's GUID representation.
        /// </summary>
        /// <param name="base64">A base64 string</param>
        /// <returns>A guid</returns>
        public static Guid FromCursor(this string base64) => base64.FromCursorToGuid();


        // STRING //

        /// <summary>
        /// Convert a string to a cursor (base64 string).
        /// </summary>
        /// <param name="str">The string to be used as a cursor</param>
        /// <returns>A cursor</returns>
        public static string ToCursor(this string str) => Convert.ToBase64String(Encoding.UTF8.GetBytes(str));

        /// <summary>
        /// Converts a cursor (base64 string) back a string.
        /// </summary>
        /// <param name="base64">The cursor to convert</param>
        /// <returns>A string</returns>
        public static string FromCursorToString(this string base64) => Encoding.UTF8.GetString(Convert.FromBase64String(base64));


        // INT //

        /// <summary>
        /// Convert an int to a cursor (base64 string).
        /// </summary>
        /// <param name="i">The int to be used as a cursor</param>
        /// <returns>A cursor</returns>
        public static string ToCursor(this int i) => Convert.ToBase64String(BitConverter.GetBytes(i));

        /// <summary>
        /// Converts a cursor (base64 string) to an int
        /// </summary>
        /// <param name="base64">The cursor to convert</param>
        /// <returns>An int</returns>
        public static int FromCursorToInt(this string base64) => BitConverter.ToInt32(Convert.FromBase64String(base64), 0);


        // BYTES //

        /// <summary>
        /// Convert some bytes to a base64 string for use as cursor.
        /// </summary>
        /// <param name="bytes">The bytes to convert to a base64 cursor.</param>
        /// <returns>A cursor</returns>
        public static string ToCursor(this byte[] bytes) => Convert.ToBase64String(bytes);

        /// <summary>
        /// Converts a cursor (base64 string) to a byte array.
        /// </summary>
        /// <param name="base64">The cursor to convert</param>
        /// <returns>A byte array</returns>
        public static byte[] FromCursorToBytes(this string base64) => Convert.FromBase64String(base64);
    }
}
