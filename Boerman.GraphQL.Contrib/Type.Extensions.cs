using System;
using System.Text;

namespace Boerman.GraphQL.Contrib
{
    public static class TypeExtensions
    {
        // See: https://codereview.stackexchange.com/questions/101636/performance-byte-to-generic
        public static T ConvertTo<T>(this byte[] bytes, int offset = 0)
        {
            var type = typeof(T);
            if (type == typeof(Guid)) return (T)(object)new Guid(bytes);
            if (type == typeof(string)) return (T)(object)Encoding.UTF8.GetString(bytes);
            if (type == typeof(sbyte)) return (T)(object)(sbyte)bytes[offset];
            if (type == typeof(byte)) return (T)(object)bytes[offset];
            if (type == typeof(short)) return (T)(object)BitConverter.ToInt16(bytes, offset);
            if (type == typeof(ushort)) return (T)(object)BitConverter.ToUInt16(bytes, offset);
            if (type == typeof(int)) return (T)(object)BitConverter.ToInt32(bytes, offset);
            if (type == typeof(uint)) return (T)(object)BitConverter.ToUInt32(bytes, offset);
            if (type == typeof(long)) return (T)(object)BitConverter.ToInt64(bytes, offset);
            if (type == typeof(ulong)) return (T)(object)BitConverter.ToUInt64(bytes, offset);

            throw new NotImplementedException();
        }

        public static T ConvertTo<T>(this string str)
        {
            return Convert
                .FromBase64String(str)
                .ConvertTo<T>();
        }

        public static T As<T>(this object o)
        {
            return (T)o;
        }
    }
}
