using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileEncryptionServices
{
    /// <summary>
    /// Interaction logic for DecryptOnStartup.xaml
    /// </summary>
    public partial class DecryptOnStartup : Window
    {
        string FilePath;
        public DecryptOnStartup(string FP)
        {
            FilePath = FP;
            InitializeComponent();
            FileDec.IsChecked = true;
        }

        private void FolderDecryptionbgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            MessageBox.Show("Decrypting in background...\n Please wait","Processing");
            List<string> agms = e.Argument as List<string>;
            foreach (string i in Directory.GetFiles(agms[0], ".", SearchOption.AllDirectories))
            {
                if (i.Contains(".Crypt"))
                {
                    FileEncryption.DecryptFile(i, i.Replace(".Crypt", ""), agms[1]);


                    if (App.FileEncryptPassErr)
                    {
                        break;
                    }
                    else
                    {
                        File.Delete(i);
                    }
                }

            }
            if (!App.FileEncryptPassErr)
            {
                
                MainWindow.SuccessBox("Folder Decryption completed");
                Application.Current.Shutdown();
            }
            else
            {
                App.FileEncryptPassErr = false;
            }

        }

        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)FileDec.IsChecked)
            {
                FileEncryption.DecryptFile(FilePath, FilePath.Replace(".Crypt", ""),DecryptionPassword.Password);
                if (!App.FileEncryptPassErr)
                {
                    MainWindow.SuccessBox("File Decrypted Successfully");
                    File.Delete(FilePath);
                    Application.Current.Shutdown();
                }
            }
            if ((bool)FolderDec.IsChecked)
            {
                List<string> Agms = new List<string>();
                Agms.Add(FilePath.Replace(FilePath.Split('\\')[FilePath.Split('\\').Length-1],"").Replace(FilePath.Split('\\')[FilePath.Split('\\').Length - 1],""));
                Agms.Add(DecryptionPassword.Password);
                BackgroundWorker BW = new BackgroundWorker();
                BW.DoWork += FolderDecryptionbgWorker_DoWork;
                BW.RunWorkerAsync(Agms);

            }
        }

    }
}
