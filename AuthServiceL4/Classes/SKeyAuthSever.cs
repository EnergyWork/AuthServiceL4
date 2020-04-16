using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceL4.Classes
{
    class SKeyAuthSever
    {
        readonly IHash hash;
        //uint N;
        readonly string database;
        public uint N { get; set; }
        public SKeyAuthSever()
        {
            hash = new MD5hash();
            N = 10;
            database = ".\\DataBaseSKey.txt";
        }
        public int GetUserN(string login)
        {
            using (StreamReader sr = new StreamReader(database))
            {
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    string[] tmpStr = str.Split(' ');
                    if (tmpStr[0] == login)
                    {
                        return Convert.ToInt32(tmpStr[2]);
                    }
                }
            }
            return 1;
        }
        public bool SetNewUser(string login, string newHash)
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
                sw.WriteLine(login + ' ' + newHash + ' ' + N);
                return true;
            }
        }
        public uint Verify(string login, string keyHash)
        {
            int n = 0;
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
                        if (!hash.VerifyHash(keyHash, tmp[1]))
                        {
                            return 1;
                        }
                        else
                        {
                            sr.Close();
                            var file = new List<string>(File.ReadAllLines(database));
                            string[] tmpStr = file[n].Split(' ');
                            tmpStr[1] = keyHash;
                            tmpStr[2] = (Convert.ToInt32(tmpStr[2]) - 1).ToString();
                            file.RemoveAt(n);
                            File.WriteAllLines(database, file.ToArray());
                            if ((Convert.ToInt32(tmpStr[2]) != 0))
                            {
                                StreamWriter sw = new StreamWriter(database, true);
                                string newStr = string.Join(" ", tmpStr);
                                sw.WriteLine(newStr);
                                sw.Close();
                            } 
                            return 2;
                        }
                    }
                    n++;
                    Array.Clear(tmp, 0, tmp.Length);
                }
                return 0;
            }
        }
    }
}
