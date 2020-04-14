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
    public partial class AuthForm : Form
    {
        string login, password;
        readonly string file;
        readonly IHash hash;
        uint attempts;
        public AuthForm()
        {
            InitializeComponent();
            hash = new MD5hash();
            attempts = 0;
            tbPassword.UseSystemPasswordChar = true;
            file = ".\\DataBase.txt";
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
        private void LockedForm()
        {
            tbLogin.Enabled = false;
            tbPassword.Enabled = false;
            bEnter.Enabled = false;
            llReg.Enabled = false;
            Text = "Locked";
        }
        private void bEnter_Click(object sender, EventArgs e)
        {
            if (!File.Exists(file))
            {
                using (FileStream fs = File.Create(file)) { }
                Message("The user is not found!", "Error", MessageBoxIcon.Warning);
                return;
            }
            using (StreamReader sr = new StreamReader(file))
            {
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    string[] tmp = str.Split(' ');
                    if (tmp[0] == login)
                    {
                        if (!hash.VerifyHash(password, tmp[1]))
                        {
                            Message("Wrong password!", "Error", MessageBoxIcon.Warning);
                            attempts++;
                            if (attempts == 3)
                            {
                                LockedForm();
                                Message("LOCKED!", "Error", MessageBoxIcon.Error);
                                return;
                            }
                            return;
                        }
                        else
                        {
                            Message("Login completed!", "Complete", MessageBoxIcon.Information);
                            attempts = 0;
                            return;
                        }
                    }
                    Array.Clear(tmp, 0, tmp.Length);
                }
                Message("The user is not found!", "Error", MessageBoxIcon.Warning);
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegistrationForm registrationForm = new RegistrationForm(this);
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
