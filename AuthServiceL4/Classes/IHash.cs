using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceL4.Classes
{
    interface IHash
    {
        string GetHash(string input);
        bool VerifyHash(string input, string hash);

    }
}
