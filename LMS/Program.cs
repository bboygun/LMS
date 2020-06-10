using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LMS
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Login login = new Login();
            //login.ShowDialog();

            //if(login.DialogResult==DialogResult.OK)
            //{
            //    login.Dispose();
            //    Application.Run(new Main());
            //}
            //else if(login.DialogResult==DialogResult.Cancel)
            //{
            //    login.Dispose();
            //    return;
            //}
            Application.Run(new Main());

        }
    }
}
