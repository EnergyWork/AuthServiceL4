using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace AuthServiceL4.Classes
{
    class MD5hash : IHash
    {
        private string getHash(string input)
        {
            byte[] data = md5hash.ComputeHash(Encoding.ASCII.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (uint i = 0; i < data.Length; i++)
                sb.Append(data[i].ToString("x2"));
            return sb.ToString();
        }
        private bool verifyHash(string input, string hash)
        {
            string hashOfInput = getHash(input);
            StringComparer sc = StringComparer.OrdinalIgnoreCase;
            return sc.Compare(hashOfInput, hash) == 0 ? true : false;
        }
        readonly MD5 md5hash;
        public MD5hash()
        {
            md5hash = new MD5CryptoServiceProvider();
        }
        string IHash.GetHash(string input)
        {
            return getHash(input);
        }

        bool IHash.VerifyHash(string input, string hash)
        {
            return verifyHash(input, hash);
        }
    }
}
