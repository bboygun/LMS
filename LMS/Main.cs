using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;
using Microsoft.Win32.SafeHandles;
using MySql.Data.MySqlClient;

namespace LMS
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.Text = "图书馆管理系统";
            Pass.login = false;

        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            string[] strs = txtFormat.Text.Split(new string[] { ", " }, StringSplitOptions.None);
            try
            {
                txtBookNumber.Text = strs[0];
                txtType.Text = strs[1];
                txtBookName.Text = strs[2];
                txtPublishing.Text = strs[3];
                txtYear.Text = strs[4];
                txtAuthor.Text = strs[5];
                txtPrice.Text = strs[6];
                txtAmount.Text = strs[7];
            }
            catch(IndexOutOfRangeException)
            {
                MessageBox.Show("格式不正确！请检查后再试。");
            }
        }

        private bool PutIn(string[] values)
        {
            bool flag = false;

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
            string sql = "select * from book where booknumber='" + values[0] + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                int amount = Convert.ToInt32(reader[7].ToString());
                amount += Convert.ToInt32(values[7]);
                string newAmount = amount.ToString();
                int stock = Convert.ToInt32(reader[8].ToString());
                stock += Convert.ToInt32(values[7]);
                reader.Close();
                string newStock = stock.ToString();
                string sql2 = "update book set amount=" + newAmount + ",stock=" + newStock + " where booknumber='" + values[0] + "'";
                MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
                int s = cmd2.ExecuteNonQuery();
                if (s == 0)
                    flag = false;
                else
                    flag = true;
                conn.Close();
            }
            else
            {
                reader.Close();
                string value = "('" + values[0] + "','" + values[1] + "','" + values[2] + "','" + values[3] + "','" + values[4] + "','" + values[5] + "','" + values[6] + "','" + values[7] + "','" + values[7] + "')";
                string sql3 = "insert into book (booknumber,type,bookname,publishing,year,author,price,amount,stock) values " + value;
                MySqlCommand cmd3 = new MySqlCommand(sql3, conn);
                int s = cmd3.ExecuteNonQuery();
                if (s == 0)
                    flag = false;
                else
                    flag = true;
                conn.Close();
            }
            return flag;
        }

        private void btnPutIn_Click(object sender, EventArgs e)
        {
            string[] values = new string[8];
            values[0] = txtBookNumber.Text;
            values[1] = txtType.Text;
            values[2] = txtBookName.Text;
            values[3] = txtPublishing.Text;
            values[4] = txtYear.Text;
            values[5] = txtAuthor.Text;
            values[6] = txtPrice.Text;
            values[7] = txtAmount.Text;

            if (PutIn(values))
                MessageBox.Show("入库成功！");
            else
                MessageBox.Show("发生错误！");
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            txtMessage.Clear();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            string path = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog1.FileName;     //显示文件路径 
            }

            StreamReader sr = new StreamReader(path, Encoding.GetEncoding("utf-8"));
            string line = "";
            ArrayList books = new ArrayList();
            int n = 0;
            while (((line = sr.ReadLine())!= null)&&(line!=""))
            {
                txtMessage.Text += line + "\r\n";
                books.Add(line);
                n++;
            }
            sr.Close();
            Pass.Books = (string[])books.ToArray(typeof(string));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int n = Pass.Books.Length;
            int cnt = 0;
            for (int i = 0; i <= n - 1; i++)
            {
                string[] values = Pass.Books[i].Split(new string[] { ", " }, StringSplitOptions.None);
                if (!PutIn(values)) cnt++;
            }
            if (cnt == 0) MessageBox.Show("入库成功！");
            else
            {
                MessageBox.Show("发生{0}条错误", cnt.ToString());
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            this.AcceptButton = btnQuery;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            listQueryResult.Items.Clear();

            string sql = "select * from book where 1=1";

            if (txtQueryType.Text != "")
                sql += " and type='" + txtQueryType.Text + "'";

            if (txtQueryBookName.Text != "")
                sql += " and bookname='" + txtQueryBookName.Text + "'";

            if (txtPublishing.Text != "")
                sql += " and publishing='" + txtQueryPublishing.Text + "'";

            if (txtQueryYear.Text != "")
            {
                string year = txtQueryYear.Text;
                string[] years = year.Split(new string[] { "-" }, StringSplitOptions.None);
                if (years.Length == 1)
                    sql += " and year='" + txtQueryYear.Text + "'";
                else if (years.Length == 2)
                    sql += " and year>=" + years[0] + " and year<=" + years[1] ;
            }

            if (txtQueryAuthor.Text != "")
                sql += " and author='" + txtQueryAuthor.Text + "'";
            if (txtQueryPrice.Text != "")
            {
                string price = txtQueryPrice.Text;
                string[] prices = price.Split(new string[] { "-" }, StringSplitOptions.None);
                if (prices.Length == 1)
                    sql += " and price=" + prices[0];
                else if (prices.Length == 2)
                    sql += " and price>=" + prices[0] + " and price<=" + prices[1];
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
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            //bool flag = false;
            int cnt = 0;
            while (reader.Read())
            {
                cnt++;
                ListViewItem tmp = new ListViewItem();
                tmp.Text = cnt.ToString();
                for (int i = 0; i <= 8; i++)
                    tmp.SubItems.Add(reader[i].ToString());
                listQueryResult.Items.Add(tmp);
            }
            if (cnt == 0) MessageBox.Show("找不到相关记录！");
            reader.Close();
            conn.Close();
        }

        private void txtQueryType_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = btnQuery;
        }

        private void listQueryResult_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnConfirmUserID_Click(object sender, EventArgs e)
        {
            listBorrowedBook.Items.Clear();
            string ID = txtLibCardID.Text;
            string sql = "select * from librarycard where cardnumber='" + ID + "'";
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
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read())
            {
                labUserInformation.Text = "姓名："+reader[1]+" 单位:"+reader[2]+" 类别: "+reader[3];
                reader.Close();

                string sql2 = "select booknumber from record where cardnumber='" + ID + "'";

                MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
                MySqlDataReader reader2 = cmd2.ExecuteReader();
                int cnt = 0;

                Queue q = new Queue();
                while(reader2.Read())
                {
                    q.Enqueue(reader2[0].ToString());
                }
                reader2.Close();
                while(q.Count!=0)
                {
                    string bookNumber = q.Dequeue().ToString();
                    string sqltmp = "select * from book where booknumber='" + bookNumber + "'";
                    MySqlCommand cmdTmp = new MySqlCommand(sqltmp, conn);
                    MySqlDataReader bookReader = cmdTmp.ExecuteReader();
                    bookReader.Read();
                    ListViewItem tmp = new ListViewItem();
                    cnt++;
                    tmp.Text = cnt.ToString();
                    for (int i = 0; i <= 8; i++)
                        tmp.SubItems.Add(bookReader.GetString(i));
                    listBorrowedBook.Items.Add(tmp);
                    bookReader.Close();
                }
                conn.Close();
            }
            else
            {
                labUserInformation.Text = "卡号不存在！";
                labUserInformation.ForeColor = Color.Red;
                conn.Close();
            }
        }

        private void btnBorrow_Click(object sender, EventArgs e)
        {
            string ISBN = txtBorrowBookNumber.Text;
            string cardNumber = txtLibCardID.Text;
            if(ISBN==""||cardNumber=="")
            {
                MessageBox.Show("请先输入借书卡号和书号");
                return;
            }
            string sql = "select stock from book where booknumber='" + ISBN + "'";
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
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read())
            {
                int stock = Convert.ToInt32(reader[0]);
                if(stock>0)
                {
                    stock--;
                    reader.Close();
                    string update = "update book set stock=" + stock + " where booknumber='" + ISBN + "'";
                    MySqlCommand cmd2 = new MySqlCommand(update, conn);
                    int s1 = cmd2.ExecuteNonQuery();
                    //string sql3 = "insert into book (booknumber,type,bookname,publishing,year,author,price,amount,stock) values " + value;
                    string borrowDate = DateTime.Now.ToString("yyyy-MM-dd");
                    string returnDate = DateTime.Now.AddDays(31).ToString("yyyy-MM-dd");
                    string record = @"insert into record (booknumber,cardnumber,borrowdate,returndate,handler) 
                        values('" + ISBN + "','" + cardNumber + "','" + borrowDate + "','" + returnDate + "','" + Pass.ID + "')";
                    MySqlCommand addRecord = new MySqlCommand(record, conn);
                    int s2 = addRecord.ExecuteNonQuery();
                    if (s1 == 1 && s2 == 1)
                    {
                        labBorrowStatus.ForeColor = Color.Green;
                        labBorrowStatus.Text = "借书成功";
                    }
                    else
                    {
                        labBorrowStatus.ForeColor = Color.Red;
                        labBorrowStatus.Text = "借书失败";
                    }
                    conn.Close();
                }
                else
                {
                    reader.Close();
                    string findReturnDate = "select returndate from record where booknumber='" + ISBN + "' order by returndate ASC";
                    MySqlCommand findDate = new MySqlCommand(findReturnDate, conn);
                    MySqlDataReader dates = findDate.ExecuteReader();
                    while (dates.Read()) { }
                    string date = ((DateTime)dates[0]).ToString("yyyy-MM-dd");
                    labBorrowStatus.ForeColor = Color.Red;
                    labBorrowStatus.Text = "该书库存不足，最近归还时间为"+date;
                }
            }
            else
            {
                labBorrowStatus.Text = "该书不存在！";
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            string bookNumber = txtBorrowBookNumber.Text;
            string borrower = txtLibCardID.Text;
            string sql = "select * from record where booknumber='" + bookNumber + "'and cardnumber='" + borrower+"'";
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
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read())
            {
                reader.Close();
                string sqlDelete = "delete from record where booknumber='" + bookNumber + "'and cardnumber='" + borrower+"'";
                MySqlCommand cmdDelete = new MySqlCommand(sqlDelete, conn);
                int s1 = cmdDelete.ExecuteNonQuery();
                string sqlQueryStock = "select stock from book where booknumber='" + bookNumber+"'";
                MySqlCommand cmdQueryStock = new MySqlCommand(sqlQueryStock, conn);
                MySqlDataReader readerStock = cmdQueryStock.ExecuteReader();
 
                if(readerStock.Read())
                {
                    
                    int stock = Convert.ToInt32(readerStock[0]);
                    readerStock.Close();
                    stock +=s1;
                    string sqlUpdate = "update book set stock=" + stock + " where booknumber='" + bookNumber+"'";
                    MySqlCommand cmdUpdate = new MySqlCommand(sqlUpdate, conn);
                    int s2 = cmdUpdate.ExecuteNonQuery();
                    if (s2 == 1)
                    {
                        labBorrowStatus.ForeColor = Color.Green;
                        labBorrowStatus.Text = "还书成功！";
                    }
                    else
                    {
                        labBorrowStatus.ForeColor = Color.Red;
                        labBorrowStatus.Text = "还书出错！";
                    }
                }
                else
                {
                    labBorrowStatus.Text = "该用户没有借阅该书！";
                    labBorrowStatus.ForeColor = Color.Red;
                }

            }
        }

        private void btnAddLibCard_Click(object sender, EventArgs e)
        {
            string addCardID = txtAddCardID.Text;
            string sqlQuery = "select * from librarycard where cardnumber='" + addCardID + "'";
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
            MySqlCommand cmd = new MySqlCommand(sqlQuery, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read())
            {
                MessageBox.Show("该借书证号已存在！");
                txtAddCardID.SelectAll();
                reader.Close();
                conn.Close();
            }
            else
            {
                reader.Close();
                string addName = txtAddName.Text;
                string addDepartment = txtAddDepartment.Text;
                string addType = txtAddType.Text;
                string values = "('" + addCardID + "','" + addName + "','" + addDepartment + "','" + addType + "')";
                string sqlAddCard = "insert into librarycard (cardnumber, name, department, type) values " + values;
                MySqlCommand cmdAdd = new MySqlCommand(sqlAddCard, conn);
                int s = cmdAdd.ExecuteNonQuery();
                if (s == 1)
                    MessageBox.Show("添加成功！");
                else
                    MessageBox.Show("添加出错！");
                conn.Close();
            }

        }

        private void btnDeleteLibCard_Click(object sender, EventArgs e)
        {
            string deleteID = txtDeleteCardID.Text;
            string sqlDelete = "delete from librarycard where cardnumber='"+deleteID+"'";
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
            MySqlCommand cmd = new MySqlCommand(sqlDelete, conn);
            int s = cmd.ExecuteNonQuery();
            if(s==0)
            {
                MessageBox.Show("该借书证不存在！");
                conn.Close();
            }
            else
            {
                MessageBox.Show("成功删除"+s+"个借书证！");
                conn.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string addAdministratorID = txtAddAdministrator.Text;
            string sqlQuery = "select * from librarycard where cardnumber='" + addAdministratorID + "'";
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
            MySqlCommand cmd = new MySqlCommand(sqlQuery, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            string password = txtAddPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            if (password.Equals(confirmPassword))
            {
                if (reader.Read())
                {
                    MessageBox.Show("该管理员ID已存在！");
                    txtAddAdministrator.SelectAll();
                    reader.Close();
                    conn.Close();
                }
                else
                {
                    reader.Close();
                    string addAdministratorName = txtAddAdministratorName.Text;
                    string addContact = txtAddContact.Text;
                    string values = "('" + addAdministratorID + "','" + password + "','" + addAdministratorName + "','" + addContact + "')";
                    string sqlAddAdministrator = "insert into librarycard (ID, password, name, contact) values " + values;
                    MySqlCommand cmdAdd = new MySqlCommand(sqlAddAdministrator, conn);
                    int s = cmdAdd.ExecuteNonQuery();
                    if (s == 1)
                        MessageBox.Show("添加成功！");
                    else
                        MessageBox.Show("添加出错！");
                    conn.Close();
                }
            }
        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            if(!Pass.login)
            {
                Login newLogin = new Login();
                if(newLogin.ShowDialog()==DialogResult.OK)
                {
                    labCurrentAdministrator.Text = Pass.ID + " " + Pass.Name;
                    return;
                }
                else
                {
                    this.tabControl1.SelectedTab = tabPage2;
                }
            }
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            Pass.ID = "";
            Pass.Name = "";
            Pass.login = false;
            tabControl1.SelectedTab = tabPage2;
        }
    }
    
}
