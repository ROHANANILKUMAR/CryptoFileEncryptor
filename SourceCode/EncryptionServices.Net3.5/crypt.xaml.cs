using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileEncryptionServices
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static bool FileEncryptPassErr = false;

        private void FileEnc_Click(object sender, RoutedEventArgs e)
        {
            FileEncryptionPassword.Password = "";
            FileEncNameDisplay.Text = "Not selected";

            DriveDecGrid.Visibility = Visibility.Hidden;
            FileEncGrid.Visibility = Visibility.Visible;
            FolderEncGrid.Visibility = Visibility.Hidden;
            FolderDecGrid.Visibility = Visibility.Hidden;
            FileDecGrid.Visibility = Visibility.Hidden;

            string filename;
            OpenFileDialog OFD = new OpenFileDialog();
            if (OFD.ShowDialog() == true)
            {
                filename = OFD.FileName;
                FileEncNameDisplay.Text = filename;
            }
            
            }

        public static void ErrorBox(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void FileDec_Click(object sender, RoutedEventArgs e)
        {
            FileDecryptionPassword.Password = "";
            FileDecNameDisplay.Text = "Not selected";

            DriveDecGrid.Visibility = Visibility.Hidden;
            FileDecGrid.Visibility = Visibility.Visible;
            DriveEncGrid.Visibility = Visibility.Hidden;
            FolderDecGrid.Visibility = Visibility.Hidden;
            FolderEncGrid.Visibility = Visibility.Hidden;
            FileEncGrid.Visibility = Visibility.Hidden;

            string filename;
            OpenFileDialog OFD = new OpenFileDialog();
            if (OFD.ShowDialog() == true)
            {              
                filename = OFD.FileName;
                if (filename.Contains("Crypt"))
                {
                    FileDecNameDisplay.Text = filename;
                }
                else
                {
                    ErrorBox("Not a valid encrypted file");
                }
            }

        }
    

        private void FolderEnc_Click(object sender, RoutedEventArgs e)
        {
            FolderEncryptionPassword.Password = "";
            FolderEncNameDisplay.Text = "Not selected";

            DriveDecGrid.Visibility = Visibility.Hidden;
            DriveEncGrid.Visibility = Visibility.Hidden;
            FolderEncGrid.Visibility = Visibility.Visible;
            FolderDecGrid.Visibility = Visibility.Hidden;
            FileDecGrid.Visibility = Visibility.Hidden;
            FileEncGrid.Visibility = Visibility.Hidden;

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if(dialog.ShowDialog()== System.Windows.Forms.DialogResult.OK)
                {
                    FolderEncNameDisplay.Text = dialog.SelectedPath;
                }
            }
        }

        private void FolderDec_Click(object sender, RoutedEventArgs e)
        {
            FolderDecryptionPassword.Password = "";
            FolderDecNameDisplay.Text = "Not selected";

            DriveDecGrid.Visibility = Visibility.Hidden;
            DriveEncGrid.Visibility = Visibility.Hidden;
            FolderDecGrid.Visibility = Visibility.Visible;
            FolderEncGrid.Visibility = Visibility.Hidden;
            FileDecGrid.Visibility = Visibility.Hidden;
            FileEncGrid.Visibility = Visibility.Hidden;

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FolderDecNameDisplay.Text = dialog.SelectedPath;
                }
            }
        }

        private void DriveEnc_Click(object sender, RoutedEventArgs e)
        {
            DriveEncryptionPassword.Password = "";

            DriveDecGrid.Visibility = Visibility.Hidden;
            DriveEncGrid.Visibility = Visibility.Visible;
            FolderDecGrid.Visibility = Visibility.Hidden;
            FolderEncGrid.Visibility = Visibility.Hidden;
            FileDecGrid.Visibility = Visibility.Hidden;
            FileEncGrid.Visibility = Visibility.Hidden;

            foreach(string i in Directory.GetLogicalDrives())
            {
                DriveEncryptionListBox.Items.Add(i);
            }
        }

        private void DriveDec_Click(object sender, RoutedEventArgs e)
        {
            DriveDecryptionPassword.Password = "";
            
            DriveDecGrid.Visibility = Visibility.Visible;
            DriveEncGrid.Visibility = Visibility.Hidden;
            FolderDecGrid.Visibility = Visibility.Hidden;
            FolderEncGrid.Visibility = Visibility.Hidden;
            FileDecGrid.Visibility = Visibility.Hidden;
            FileEncGrid.Visibility = Visibility.Hidden;
            foreach (string i in Directory.GetLogicalDrives())
            {
                DriveDecryptionListBox.Items.Add(i);
            }
        }

        private void EncryptFile_Click(object sender, RoutedEventArgs e)
        {
            if (FileEncNameDisplay.Text.Equals("Not selected"))
            {
                ErrorBox("No File Selected");
            }
            else
            {
                CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Encrypting... Please wait")));
                FileEncryption.EncryptFile(FileEncNameDisplay.Text, FileEncNameDisplay.Text + "Crypt", FileEncryptionPassword.Password);
                if (!(bool)CheckFileEnc.IsChecked)
                {
                    File.Delete(FileEncNameDisplay.Text);
                }
                CryptStatus.Document.Blocks.Add(new Paragraph(new Run("File Encrypted :"+ FileEncNameDisplay.Text+"\n")));
            }
        }

        private void DecryptFile_Click(object sender, RoutedEventArgs e)
        {
            if (FileDecNameDisplay.Text.Equals("Not selected"))
            {
                ErrorBox("No File Selected");
            }
            else
            {
                try
                {
                    CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Decrypting... Please wait")));
                    FileEncryption.DecryptFile(FileDecNameDisplay.Text, FileDecNameDisplay.Text.Replace("Crypt", ""), FileDecryptionPassword.Password);
                    if (!(bool)CheckFileDec.IsChecked&&!FileEncryptPassErr)
                    {
                        File.Delete(FileDecNameDisplay.Text);
                    }
                    if (!FileEncryptPassErr)
                    {
                        CryptStatus.Document.Blocks.Add(new Paragraph(new Run("File Decrypted :" + FileDecNameDisplay.Text+"\n")));
                    }
                    FileEncryptPassErr = false;

                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("The file") && ex.Message.Contains("already exists"))
                    {
                        ErrorBox("A file of the same name exists in that folder!");
                    }

                    else if(ex.Message.Contains("Padding is invalid"))
                    {
                        ErrorBox("Wrong password");
                    }
                    else
                    {
                        ErrorBox(ex.Message);
                    }

                    
                }
            }
        }



        private void EncryptionFolderThread()
        {
            string[] Files= {};
            this.Dispatcher.Invoke(() =>
            {
                Files = Directory.GetFiles(FolderEncNameDisplay.Text, "*", SearchOption.AllDirectories);
            });
            

            foreach (string i in Files)
            {
                this.Dispatcher.Invoke(() =>
                {
                        try
                    {
                        FileEncryption.EncryptFile(i, i + "Crypt", FolderEncryptionPassword.Password);
                        CryptStatus.Document.Blocks.Add(new Paragraph(new Run("File Encrypted :" + i)));
                        File.Delete(i);

                    }
                    catch (Exception Ex)
                    {
                        ErrorBox(Ex.Message);
                    }
                });
            }
            this.Dispatcher.Invoke(() =>
            {
                CryptStatus.Document.Blocks.Add(new Paragraph(new Run("\n")));
            });
        }

        private void EncryptionDriveThread()
        {
            string[] Files = { };
            this.Dispatcher.Invoke(() =>
            {
                Files = Directory.GetFiles(DriveEncryptionListBox.SelectedItem.ToString(), "*", SearchOption.AllDirectories);
            });


            foreach (string i in Files)
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        FileEncryption.EncryptFile(i, i + "Crypt", FolderEncryptionPassword.Password);
                        CryptStatus.Document.Blocks.Add(new Paragraph(new Run("File Encrypted :" + i)));
                        File.Delete(i);

                    }
                    catch (Exception Ex)
                    {
                        ErrorBox(Ex.Message);
                    }
                });
            }
            this.Dispatcher.Invoke(() =>
            {
                CryptStatus.Document.Blocks.Add(new Paragraph(new Run("\n")));
            });
        }

        private void DecryptionDriveThread()
        {
            string[] Files = { };
            this.Dispatcher.Invoke(() =>
            {
                Files = Directory.GetFiles(DriveDecryptionListBox.SelectedItem.ToString(), "*", SearchOption.AllDirectories);
            });


            foreach (string i in Files)
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (i.Contains("Crypt"))
                    {
                        try
                        {
                            FileEncryption.DecryptFile(i, i.Replace("Crypt", ""), FolderDecryptionPassword.Password);
                            CryptStatus.Document.Blocks.Add(new Paragraph(new Run("File Decrypted :" + i)));
                            File.Delete(i);

                        }
                        catch (Exception Ex)
                        {
                            ErrorBox(Ex.Message);
                        }
                    }
                });
            }
            this.Dispatcher.Invoke(() =>
            {
                CryptStatus.Document.Blocks.Add(new Paragraph(new Run("\n")));
            });
        }

        private void EncryptFolder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Encrypting Folder... The screen may freeze.", "Running", MessageBoxButton.OK, MessageBoxImage.Information);
            CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Encrypting folder... This screen might freeze as the process is running in background")));
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(EncryptionFolderThread));
            t.Start();       
        }

        private void DecryptFolder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Decrypting Folder... The screen may freeze.", "Running", MessageBoxButton.OK, MessageBoxImage.Information);
            CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Decrypting folder... This screen might freeze as the process is running in background")));

            foreach (string i in Directory.GetFiles(FolderDecNameDisplay.Text, "*", SearchOption.AllDirectories))
            {

                try
                {
                    FileEncryption.DecryptFile(i, i.Replace("Crypt", ""), FolderDecryptionPassword.Password);
                    CryptStatus.Document.Blocks.Add(new Paragraph(new Run("File Decrypted :" + i)));
                    File.Delete(i);

                }
                catch (Exception Ex)
                {
                    ErrorBox(Ex.Message);
                }
           
            }
            CryptStatus.Document.Blocks.Add(new Paragraph(new Run("\n")));
        }

        private void EncryptDrive_Click(object sender, RoutedEventArgs e)
        {
            if (DriveEncryptionListBox.SelectedItem.ToString().Equals(""))
            {
                ErrorBox("No Drive selected");
            }
            else
            {

                MessageBox.Show("Encrypting Drive... The screen may freeze.", "Running", MessageBoxButton.OK, MessageBoxImage.Information);
                CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Encrypting Drive... This screen might freeze as the process is running in background")));
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(EncryptionDriveThread));
                t.Start();
            }
        }

        private void DecryptDrive_Click(object sender, RoutedEventArgs e)
        {
            if (DriveDecryptionListBox.SelectedItem.ToString().Equals(""))
            {
                ErrorBox("No Drive selected");
            }
            else
            {

                MessageBox.Show("Decrypting Drive... The screen may freeze.", "Running", MessageBoxButton.OK, MessageBoxImage.Information);
                CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Decrypting Drive... This screen might freeze as the process is running in background")));
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(DecryptionDriveThread));
                t.Start();
            }
        }
    }
}
