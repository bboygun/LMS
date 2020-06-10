using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;
using MySql.Data.MySqlClient;

namespace LMS
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void login_Load(object sender, EventArgs e)
        {

        }

        private void btnlogin_Click(object sender, EventArgs e)
        {
            string ID = txtID.Text;
            string password = txtPassWord.Text;

            if (ID == "" || password == "")
            {
                MessageBox.Show("用户名或密码不能为空！");
            }


            string connectStr = "server=127.0.0.1;port=3306;user=root;password=257248; database=librarymanagesystem;";
            MySqlConnection conn = new MySqlConnection(connectStr);

            try
            {
                conn.Open();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            //string sql = "select * from administrator where ID='" + ID + "' and password='" + password + "'";
            //MySqlCommand cmd = new MySqlCommand(sql, conn);
            //MySqlDataReader reader = cmd.ExecuteReader();

            //if (reader.Read())
            //{
            //    Pass.ID = ID;
            //    Pass.Name = (string)reader[1];
            //    reader.Close();
            //    this.DialogResult = DialogResult.OK;
            //    this.Dispose();
            //    this.Close();
            //}
            //else
            //{
            //    MessageBox.Show("用户名或密码错误！");
            //    txtPassWord.SelectAll();
            //}

            string sql = "select password,name from administrator where ID='" + ID + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                if (reader[0].Equals(password))
                {
                    Pass.ID = ID;
                    Pass.Name = (string)reader[1];
                    reader.Close();
                    this.DialogResult = DialogResult.OK;
                    Pass.login = true;
                    this.Dispose();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("密码错误！");
                    txtPassWord.SelectAll();
                }
            }
            else
            {
                MessageBox.Show("该管理员不存在！");
                txtPassWord.Clear();
                txtID.Clear();
                txtID.Focus();
            }
        }

        private void txtPassWord_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = btnlogin;
        }
    }


    static class Pass
    {
        public static string ID;
        public static string Name;
        public static string[] Books;
        public static bool login;
    }
}
    
    
