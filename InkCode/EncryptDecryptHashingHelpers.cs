using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace InkCode
{
    public class EncryptDecryptHashingHelpers
    {
            public static string Base64Encode(string text)
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(text)).TrimEnd('=').Replace('+', '-')
                    .Replace('/', '_');
            }

            public static string Base64Decode(string text)
            {
                text = text.Replace('_', '/').Replace('-', '+');
                switch (text.Length % 4)
                {
                    case 2:
                        text += "==";
                        break;
                    case 3:
                        text += "=";
                        break;
                }
                return Encoding.UTF8.GetString(Convert.FromBase64String(text));
            }

            public static string SHA1Encrypt(string text)
            {
                var hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(text));
                return string.Concat(hash.Select(b => b.ToString("x2")));
            }

        public static string SHA2Encrypt(string text)
        {
            var hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(text));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        public static string SHA3256Encrypt(string text)
        {
            var hash = SHA3_256.Create().ComputeHash(Encoding.UTF8.GetBytes(text));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        public static string SHA3Encrypt(string text)
        {
            var hash = SHA384.Create().ComputeHash(Encoding.UTF8.GetBytes(text));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        public static string SHA4Encrypt(string text)
        {
            var hash = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(text));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }
    }
}