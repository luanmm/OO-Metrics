using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Core.Security
{
    public static class HashExtensions
    {
        public static string ToMD5(this string data)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(data))).Replace("-", String.Empty).ToLowerInvariant();
            }
        }

        public static string ToSHA1(this string data)
        {
            using (var sha1 = SHA1.Create())
            {
                return BitConverter.ToString(sha1.ComputeHash(Encoding.Default.GetBytes(data))).Replace("-", String.Empty).ToLowerInvariant();
            }
        }
    }
}
