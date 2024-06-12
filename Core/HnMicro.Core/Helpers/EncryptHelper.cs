using System.Security.Cryptography;
using System.Text;

namespace HnMicro.Core.Helpers
{
    public static class EncryptHelper
    {
        public static string Md5(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            var sb = new StringBuilder();
            var bytes = Encoding.Default.GetBytes(input);
            bytes = MD5.Create().ComputeHash(bytes);
            for (var x = 0; x <= bytes.Length - 1; x++)
                sb.Append(bytes[x].ToString("x2"));

            return sb.ToString().ToLower();
        }

        public static string Sha(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using var provider = new SHA256Managed();
            var d = new StringBuilder();
            var bytes = provider.ComputeHash(Encoding.UTF8.GetBytes(input));
            foreach (var itemByte in bytes)
            {
                d.Append(itemByte.ToString("x2"));
            }
            return d.ToString();
        }

        public static string Base64Encode(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public static string Base64Decode(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return Encoding.UTF8.GetString(Convert.FromBase64String(input));
        }

        public static string CustomPassword(this string password, string hash)
        {
            return string.Format("{0}.{1}", password, hash).Md5();
        }

        public static string DecodePassword(this string password)
        {
            string result = string.Empty;
            // Decode from Base64 to byte array
            byte[] decodedBytes = Convert.FromBase64String(password);

            // Decode from ASCII bytes to string
            string decodedText = System.Text.Encoding.ASCII.GetString(decodedBytes);

            // Split the string into individual codes (assuming space separated)
            int[] asciiCodes = decodedText.Split(' ').Select(int.Parse).ToArray();

            foreach (int code in asciiCodes)
            {
                result += Convert.ToChar(code);
            }

            return result;
        }
    }
}
