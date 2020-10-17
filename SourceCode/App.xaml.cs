using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.IO;

namespace FileEncryptionServices
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string DefaultPas = "";
        public static bool DefaultPassword = false;
        public static string Set = @"Settings.cpt";
        public static void ErrBox(string mess)
        {
            MessageBox.Show(mess, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static bool FileEncryptPassErr = false;
        void App_Startup(object sender, StartupEventArgs e)
        {
           
            try 
            {
                int name = e.Args.Length;
                if (e.Args[0].Contains(".Crypt"))
                {
                    DecryptOnStartup D = new DecryptOnStartup(e.Args[0]);
                    ///DecryptOnStartup D = new DecryptOnStartup(@"C:\Users\user\Desktop\blah\000.mp3.Crypt");
                    D.Show();
                }
                else
                {
                    ErrBox("Not a valid Encrypted file :"+e.Args[0]);
                }
            }
            catch
            {
                string udata = File.ReadAllText(App.Set);
                if (udata.Split(':')[0].Equals(Encryption.Encrypt("ON")))
                {
                    DefaultPassword = true;
                    DefaultPas = Encryption.Decrypt(udata.Split(':')[1]);
                }
                FileEncryptionServices.MainWindow M = new FileEncryptionServices.MainWindow();
                M.Show();
            }
        }



    }
}
