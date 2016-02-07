using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vorbote
{
    public static class Extensions
    {
        public static string Base64Decode(this string value)
        {
            var data = Convert.FromBase64String(value);
            var result = Encoding.UTF8.GetString(data);
            return result;
        }

        public static string Base64Encode(this string value)
        {
            var data = Encoding.UTF8.GetBytes(value);
            var result = Convert.ToBase64String(data);
            return result;
        }
    }
}
