using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Services.Generators
{
    public class DefaultRandomStringGenerator : IRandomStringGenerator
    {
        private static Random Random = new();
        public Task<string> Genetare(int length)
        {
            var lengths = new[] { length / 2, 1, length / 2 };
            var sb = new StringBuilder();
            using (var csprng = new RNGCryptoServiceProvider())
            {
                for (var i = 0; i < lengths.Count(); i++)
                {
                    var bytes = new byte[lengths.ElementAt(i)];
                    csprng.GetNonZeroBytes(bytes);
                    var tmp = BitConverter.ToString(bytes);
                    if (tmp.Contains(" ")) i--;
                    sb.Append(tmp);
                }
            }
            var g = sb.ToString()
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .ToLower();

            var res = string.Empty;
            do
            {
                var r1 = Random.Next(0, length / 2);
                res += g.Substring(r1, Random.Next(length / 2, length));
            } while (res.Length < length);

            return Task.FromResult(g[..length]);
        }
    }
}
