using Application.InfrastructureContracts.SharedProject.Hasher.Interface;
using System.Security.Cryptography;
using System.Text;

namespace Infra.SharedService.StringHasherService.Class
{
    public class StringHasher : IStringHasher
    {

        public string HashString(string input)
        {
            using SHA256 sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(hash)[..32]; // دارای 32 کاراکتر هست .
        }

    }
}
