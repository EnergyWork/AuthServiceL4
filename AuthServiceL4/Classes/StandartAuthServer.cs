using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceL4.Classes
{
    class StandartAuthServer
    {
        readonly string database;
        readonly IHash hash;
        public StandartAuthServer()
        {
            database = ".\\DataBase.txt";
            hash = new MD5hash();
        }
        public bool SetNewUser(string login, string password)
        {
            if (!File.Exists(database))
            {
                using (FileStream fs = File.Create(database)) { }
            }
            using (StreamReader sr = new StreamReader(database))
            {
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    string[] tmp = str.Split(' ');
                    if (tmp[0] == login)
                    {
                        return false;
                    }
                    Array.Clear(tmp, 0, tmp.Length);
                }
            }
            using (StreamWriter sw = new StreamWriter(database, true))
            {
                sw.WriteLine(login + ' ' + hash.GetHash(password));
                return true;
            }
        }
        public uint Verify(string login, string password)
        {
            if (!File.Exists(database))
            {
                using (FileStream fs = File.Create(database)) { }
                return 0;
            }
            using (StreamReader sr = new StreamReader(database))
            {
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    string[] tmp = str.Split(' ');
                    if (tmp[0] == login)
                    {
                        if (!hash.VerifyHash(password, tmp[1]))
                        {
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    Array.Clear(tmp, 0, tmp.Length);
                }
                return 0;
            }
        }
    }
}
