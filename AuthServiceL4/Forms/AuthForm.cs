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
    public enum AuthType
    {
        STANDART, SKEY
    }
    public partial class AuthForm : Form
    {
        string login, password;
        string file; // ".\\DataBase.txt" ".\\DataBaseSKey.txt"
        readonly IHash hash;
        readonly SKeyAuthSever skey;
        readonly StandartAuthServer standart;
        AuthType authType;
        uint attempts;
        public AuthForm()
        {
            InitializeComponent();
            cbMethod.SelectedIndex = 0;
            hash = new MD5hash();
            skey = new SKeyAuthSever();
            standart = new StandartAuthServer();
            authType = AuthType.STANDART;
            attempts = 0;
            tbPassword.UseSystemPasswordChar = true;
            file = ".\\DataBase.txt";
        }
        private string keyHash(int N, string key)
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
        private void ToLockTheForm()
        {
            tbLogin.Enabled = false;
            tbPassword.Enabled = false;
            bEnter.Enabled = false;
            llReg.Enabled = false;
            cbMethod.Enabled = false;
            Text = "Locked";
        }
        private void bEnter_Click(object sender, EventArgs e)
        {
            uint callback;
            if (authType == AuthType.STANDART)
                callback = standart.Verify(login, password);
            else
                callback = skey.Verify(login, keyHash(skey.GetUserN(login) - 1, password));

            switch(callback)
            {
                case 0: // "The user is not found!"
                    Message("The user is not found!", "Error", MessageBoxIcon.Warning);
                    break;
                case 1: // "Wrong password!"
                    Message("Wrong password!", "Warning", MessageBoxIcon.Warning);
                    attempts++;
                    if (attempts == 3)
                    {
                        ToLockTheForm();
                        Message("LOCKED!", "Error", MessageBoxIcon.Error);
                    }
                    break;
                case 2: // "Login completed!"
                    Message("Login completed!!", "Complete", MessageBoxIcon.Information);
                    attempts = 0;
                    break;
            }  
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegistrationForm registrationForm = new RegistrationForm(this, authType);
            Visible = false;
            registrationForm.ShowDialog();
        }
        private void tbPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8 && !string.IsNullOrEmpty(password))
            {
                password = password.Substring(0, password.Length - 1);
            }
            else
            {
                password += e.KeyChar;
            }
        }
        private void UdpateForm()
        {
            lbPassword.Text = (authType == AuthType.STANDART) ? "Password" : "Key";
            file = (authType == AuthType.STANDART) ? ".\\DataBase.txt" : ".\\DataBaseSKey.txt";
        }
        private void cbMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(cbMethod.SelectedIndex)
            {
                case 0: //Standart method
                    authType = AuthType.STANDART;
                    UdpateForm();
                    break;
                case 1: //SKey method
                    authType = AuthType.SKEY;
                    UdpateForm();
                    break;
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
    }
}
