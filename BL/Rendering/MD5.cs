using System.Text;
using Crypto = System.Security.Cryptography;

namespace KoobecaFeedController.BL {
    public class Md5 {
        public static string CalculateMd5Hash(string input) {
            // step 1, calculate MD5 hash from input

            var md5 = Crypto.MD5.Create();

            var inputBytes = Encoding.ASCII.GetBytes(input);

            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            var sb = new StringBuilder();

            for (var i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("X2"));

            return sb.ToString().ToLower();
        }
    }
}