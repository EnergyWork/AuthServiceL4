using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using AuthServiceL4.Classes;

namespace AuthServiceL4
{
    public partial class RegistrationForm : Form
    {
        string password, login;
        //readonly string file;
        readonly IHash hash;
        AuthForm aform;
        SKeyAuthSever skey;
        StandartAuthServer standart;
        readonly AuthType atype;
        public RegistrationForm(AuthForm af, AuthType at)
        {
            InitializeComponent();
            tbPassword.UseSystemPasswordChar = true;
            tbPassword.MaxLength = 16;
            tbLogin.MaxLength = 16;
            //file = ".\\DataBase.txt";
            hash = new MD5hash();
            skey = new SKeyAuthSever();
            standart = new StandartAuthServer();
            aform = af;
            atype = at;
            lbPassword.Text = (at == AuthType.STANDART) ? "Password" : "Key";
        }
        private string keyHash(uint N, string key)
        {
            string tmpHash = key;
            for (uint i = 0; i < N; i++)
                tmpHash = hash.GetHash(tmpHash);
            return tmpHash;
        }
        private void Message(string text, string title, MessageBoxIcon icon)
        {
            DialogResult dr = MessageBox.Show(
                text,
                title,
                MessageBoxButtons.OK,
                icon,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);
            if (dr == DialogResult.OK)
                return;
        }
        private void cbShow_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShow.Checked)
            {
                tbPassword.UseSystemPasswordChar = false;
            }
            else
            {
                tbPassword.UseSystemPasswordChar = true;
            }
        }
        private void tbPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8 && !string.IsNullOrEmpty(password))
            {
                if (password != null)
                {
                    password = password.Substring(0, password.Length - 1);
                }
            }
            else
            {
                password += e.KeyChar;
            }
        }
        private void tbLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
            else
            {
                if (e.KeyChar == 8 && !string.IsNullOrEmpty(login))
                {
                    login = login.Substring(0, login.Length - 1);
                }
                else
                {
                    login += e.KeyChar;
                }
            }
        }
        private void RegistrationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            aform.Visible = true;
        }
        private void bRegister_Click(object sender, EventArgs e)
        {
            if (atype == AuthType.STANDART 
                ? standart.SetNewUser(login, password) 
                : skey.SetNewUser(login, keyHash(skey.N, password)))
            {
                Message("Registration completed", "Done", MessageBoxIcon.Information);
                Close();
            }
            else
            {
                Message("This login already exist!", "Error", MessageBoxIcon.Error);
                return;
            }
        }
    }
}
