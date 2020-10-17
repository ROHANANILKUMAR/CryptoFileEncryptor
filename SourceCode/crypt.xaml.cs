using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;
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

        public static bool Working = false;

        List<BackgroundWorker> BWs = new List<BackgroundWorker>();
        
        #region Miscellaneous

        public static void SuccessBox(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ErrorBox(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            Credits C = new Credits();
            C.Show();
        }
        #endregion

        #region UIGridButtons
        private void FileEnc_Click(object sender, RoutedEventArgs e)
        {
            FileEncryptionPassword.Password = "";
            if (App.DefaultPassword)
            {
                FileEncryptionPassword.Password = App.DefaultPas;
            }

            FileEncNameDisplay.Text = "Not selected";

            IntroGrid.Visibility = Visibility.Hidden;
            DriveEncGrid.Visibility = Visibility.Hidden;
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

        private void FileDec_Click(object sender, RoutedEventArgs e)
        {
            FileDecryptionPassword.Password = "";
            FileDecNameDisplay.Text = "Not selected";

            IntroGrid.Visibility = Visibility.Hidden;
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
                if (filename.Contains(".Crypt"))
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

            if (App.DefaultPassword)
            {
                FolderEncryptionPassword.Password = App.DefaultPas;
            }

            IntroGrid.Visibility = Visibility.Hidden;
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

            IntroGrid.Visibility = Visibility.Hidden;
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

            if (App.DefaultPassword)
            {
                DriveEncryptionPassword.Password = App.DefaultPas;
            }

            IntroGrid.Visibility = Visibility.Hidden;
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

            IntroGrid.Visibility = Visibility.Hidden;
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
        #endregion

        #region BackGroundTasks
        void FileEncryptionbgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> agms = e.Argument as List<string>;
            FileEncryption.EncryptFile(agms[0], agms[0] + ".Crypt", agms[1]);
            if (agms[2].Equals("Delete"))
            {
                File.Delete(agms[0]);
            }
            this.Dispatcher.Invoke(() =>
            {
                CryptStatus.Document.Blocks.Add(new Paragraph(new Run("File Encrypted :" + FileEncNameDisplay.Text + "\n")));
                SuccessBox("File Encrypted :" + FileEncNameDisplay.Text);
            });

        }

        void FileDecryptionbgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> agms = e.Argument as List<string>;
            FileEncryption.DecryptFile(agms[0], agms[0].Replace(".Crypt", ""), agms[1]);
            if (agms[2].Equals("Delete No Err")&&!App.FileEncryptPassErr)
            {
                File.Delete(agms[0]);
            }
            else
            {
                File.Delete(agms[0].Replace(".Crypt", ""));
            }
            if (!App.FileEncryptPassErr)
            {
                this.Dispatcher.Invoke(() =>
                {
                    CryptStatus.Document.Blocks.Add(new Paragraph(new Run("File Decrypted :" + FileDecNameDisplay.Text + "\n")));
                    SuccessBox("File Decrypted :" + FileDecNameDisplay.Text);
                });
                
            }
        }

        void NewLocFileDecryptionbgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> agms = e.Argument as List<string>;
            FileEncryption.DecryptFile(agms[0], agms[4]+"."+agms[0].Split('.')[agms[0].Split('.').Length-2], agms[1]);
            if (agms[2].Equals("Delete No Err") && !App.FileEncryptPassErr)
            {
                File.Delete(agms[0]);
            }
            else
            {
                File.Delete(agms[0].Replace(".Crypt", ""));
            }
            if (!App.FileEncryptPassErr)
            {
                this.Dispatcher.Invoke(() =>
                {
                    CryptStatus.Document.Blocks.Add(new Paragraph(new Run("File Decrypted :" + FileDecNameDisplay.Text + "\n")));
                    SuccessBox("File Decrypted :" + FileDecNameDisplay.Text);
                });

            }
        }

        void FolderEncryptionbgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> agms = e.Argument as List<string>;
            string[] Files = { };
            try
            {
                Files = Directory.GetFiles(agms[0], "*", SearchOption.AllDirectories);
            }
            catch
            {
                ErrorBox("No directory Selected");
                this.Dispatcher.Invoke(() =>
                {
                    CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Encryption Terminated\n")));
                });
                return;
            }
            bool check = true;

            foreach (string i in Files)
            {
                if (i.Contains(".Crypt"))
                {
                    ErrorBox("This Folder contains fils that are already encrypted.\nDecrypt those and retry Encryption.");
                    this.Dispatcher.Invoke(() =>
                    {
                        CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Encryption Terminated\n")));
                    });
                    check = false;
                    return;
                }
            }

            foreach (string i in Files)
            {
                if (i.Contains(".Crypt"))
                {
                    ErrorBox("This File is Already Encrypted");
                    this.Dispatcher.Invoke(() =>
                    {
                        CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Encryption Terminated\n")));
                    });
                    check = false;
                    break;
                }
                
                    try
                    {
                        FileEncryption.EncryptFile(i, i + ".Crypt", agms[1]);
                    this.Dispatcher.Invoke(() =>
                    {
                        CryptStatus.Document.Blocks.Add(new Paragraph(new Run("File Encrypted :" + i)));
                    });
                    File.Delete(i);

                    }
                    catch (Exception Ex)
                    {
                        ErrorBox(Ex.Message);
                    }
                
            }
            if (check)
            {
                this.Dispatcher.Invoke(() =>
                {
                    CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Folder Encryption Completed\n")));
                    SuccessBox("Folder Encryption Completed");
                });
            }
        }
        private void FolderDecryptionbgWorker_DoWork(object sender, DoWorkEventArgs e) {
            List<string> agms = e.Argument as List<string>;;
            try
            {
                Directory.GetFiles(agms[0], "*", SearchOption.AllDirectories);
            }
            catch
            {
                ErrorBox("No directory selected");
                this.Dispatcher.Invoke(() =>
                {
                    CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Decryption Terminated\n")));
                });
                return;
            }
            foreach (string i in Directory.GetFiles(agms[0], "*", SearchOption.AllDirectories))
            {

                try
                {
                    FileEncryption.DecryptFile(i, i.Replace(".Crypt", ""), agms[1]);
                    if (App.FileEncryptPassErr)
                    { 
                        this.Dispatcher.Invoke(() =>
                        {
                            CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Wrong Password... Decryption Terminated\n")));
                        });
                        break;
                    }
                    this.Dispatcher.Invoke(() =>
                    {
                        CryptStatus.Document.Blocks.Add(new Paragraph(new Run("File Decrypted :" + i)));
                    });
                    File.Delete(i);

                }
                catch (Exception Ex)
                {
                    ErrorBox(Ex.Message);
                }

            }
            if (!App.FileEncryptPassErr)
            {
                this.Dispatcher.Invoke(() =>
                {
                    CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Folder Decryption completed\n")));
                    SuccessBox("Folder Decryption completed");
                });
            }
            else
            {
                App.FileEncryptPassErr = false;
            }
            
        }

        private void DriveEncryptionbgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> agms = e.Argument as List<string>; ;
            string[] Files = { };
            try
            {
                Files = Directory.GetFiles(agms[0], "*", SearchOption.AllDirectories);
                this.Dispatcher.Invoke(() =>
                {
                    CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Encryption Terminated.\n")));
                });
            }
            catch
            {
                ErrorBox("No drive selected");
                return;
            }
            foreach(string i in Files)
            {
                if (i.Contains(".Crypt"))
                {
                    ErrorBox("This drive contains files that are already encrypted.\nDecrypt those and retry encryption");
                    this.Dispatcher.Invoke(() =>
                    {
                        CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Encryption Terminated.\n")));
                    });
                    return;
                }
            }
            foreach (string i in Files)
            { 
                    try
                    {
                        FileEncryption.EncryptFile(i, i + ".Crypt", agms[1]);
                        this.Dispatcher.Invoke(() =>
                        {
                            CryptStatus.Document.Blocks.Add(new Paragraph(new Run("File Encrypted :" + i)));
                        });
                        File.Delete(i);

                    }
                    catch (Exception Ex)
                    {
                        ErrorBox(Ex.Message);
                    }
                
            }
            this.Dispatcher.Invoke(() =>
            {
                CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Drive Encryption Completed.\n")));
                SuccessBox("Drive Encryption Completed.");
            });
        }


        void DriveDecryptionbgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> agms = e.Argument as List<string>; ;
            string[] Files = { };
            try
            {
                Files = Directory.GetFiles(agms[0], "*", SearchOption.AllDirectories);
            }
            catch
            {
                ErrorBox("No Drive selected");
                this.Dispatcher.Invoke(() =>
                {
                    CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Decryption Terminated\n")));
                });
                return;
            }

            foreach (string i in Files)
            {
                
                    if (i.Contains(".Crypt"))
                    {
                        try
                        {
                            
                            FileEncryption.DecryptFile(i, i.Replace(".Crypt", ""),agms[1]);
                            if (App.FileEncryptPassErr)
                            { 
                                this.Dispatcher.Invoke(() =>
                                {
                                    CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Wrong Password... Decryption Terminated\n")));
                                });
                                break;
                            }
                            this.Dispatcher.Invoke(() =>
                            {
                                CryptStatus.Document.Blocks.Add(new Paragraph(new Run("File Decrypted :" + i)));
                            });
                            File.Delete(i);
                        }
                        catch (Exception Ex)
                        {
                            ErrorBox(Ex.Message);
                        }
                    }
                
            }
            if (!App.FileEncryptPassErr)
            {
                this.Dispatcher.Invoke(() =>
                {
                    CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Folder Decryption completed\n")));
                    SuccessBox("Folder Decryption completed");
                });
            }
            else
            {
                App.FileEncryptPassErr = false;
            }
        }
        #endregion

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Working = false;
        }


        #region TaskHost
        private void EncryptFile_Click(object sender, RoutedEventArgs e)
        {
            if (FileEncNameDisplay.Text.Equals("Not selected"))
            {
                ErrorBox("No File Selected");
            }
            else
            {
                List<string> Agms = new List<string>();
                Agms.Add(FileEncNameDisplay.Text);
                Agms.Add(FileEncryptionPassword.Password);
                if (!(bool)CheckFileEnc.IsChecked)
                {
                    Agms.Add("Delete");
                }
                else
                {
                    Agms.Add("Nope");
                }

                if (Working)
                {
                    ErrorBox("A process is running in the background. Wait for it to finish");
                    return;
                }
                CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Encrypting... Please wait")));
                BackgroundWorker bgWorker;
                bgWorker = new BackgroundWorker();
                bgWorker.DoWork += new DoWorkEventHandler(FileEncryptionbgWorker_DoWork);
                bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
                bgWorker.RunWorkerAsync(Agms);
                BWs.Add(bgWorker);
                Working = true;
              
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
                    List<string> Agms = new List<string>();
                    Agms.Add(FileDecNameDisplay.Text);
                    Agms.Add(FileDecryptionPassword.Password);
                    
                    if (!(bool)CheckFileDec.IsChecked)
                    {
                        Agms.Add("Delete No Err");
                    }
                    else
                    {
                        Agms.Add("Err");
                    }
                    if (!App.FileEncryptPassErr)
                    {
                        Agms.Add("No Err");
                    }
                    else
                    {
                        Agms.Add("Err");
                    }
                    if (Working)
                    {
                        ErrorBox("A process is running in the background. Wait for it to finish");
                        return;
                    }
                    App.FileEncryptPassErr = false;
                    CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Decrypting... Please wait")));
                    BackgroundWorker bgWorker;
                    bgWorker = new BackgroundWorker();
                    bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
                    bgWorker.DoWork += new DoWorkEventHandler(FileDecryptionbgWorker_DoWork);
                    bgWorker.RunWorkerAsync(Agms);
                    BWs.Add(bgWorker);
                    Working = true;

                }
                catch (Exception ex)
                {
                    
                    if(ex.Message.Contains("Padding is invalid"))
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
        private void EncryptFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Working)
            {
                ErrorBox("A process is running in the background. Wait for it to finish");
                return;
            }
            List<string> Agms = new List<string>();
            Agms.Add(FolderEncNameDisplay.Text);
            Agms.Add(FolderEncryptionPassword.Password);
            CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Encrypting... Please wait")));
            BackgroundWorker bgWorker;
            bgWorker = new BackgroundWorker();
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            bgWorker.DoWork += new DoWorkEventHandler(FolderEncryptionbgWorker_DoWork);
            bgWorker.RunWorkerAsync(Agms);
            BWs.Add(bgWorker);
            Working = true;
        }
        private void DecryptFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Working)
            {
                ErrorBox("A process is running in the background. Wait for it to finish");
                return;
            }
            List<string> Agms = new List<string>();
            Agms.Add(FolderDecNameDisplay.Text);
            Agms.Add(FolderDecryptionPassword.Password);
            CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Decrypting folder... Please Wait...")));
            BackgroundWorker bgWorker;
            bgWorker = new BackgroundWorker();
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            bgWorker.DoWork += new DoWorkEventHandler(FolderDecryptionbgWorker_DoWork);
            bgWorker.RunWorkerAsync(Agms);
            BWs.Add(bgWorker);
            Working = true;
        }

        private void EncryptDrive_Click(object sender, RoutedEventArgs e)
        {
            if (Working)
            {
                ErrorBox("A process is running in the background. Wait for it to finish");
                return;
            }
            bool check = true;
            try { DriveEncryptionListBox.SelectedItem.ToString().Equals(""); }
            catch { check = false; }
            if (check) {
                List<string> Agms = new List<string>();
                Agms.Add(DriveEncryptionListBox.SelectedItem.ToString());
                Agms.Add(DriveEncryptionPassword.Password);
                CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Encrypting Drive... Please Wait")));
                BackgroundWorker bgWorker;
                bgWorker = new BackgroundWorker();
                bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
                bgWorker.DoWork += new DoWorkEventHandler(DriveEncryptionbgWorker_DoWork);
                bgWorker.RunWorkerAsync(Agms);
                BWs.Add(bgWorker);
                Working = true;
            }
            else
            {
                ErrorBox("No Drive Selected");
            }
        }

        private void DecryptDrive_Click(object sender, RoutedEventArgs e)
        {
            if (Working)
            {
                ErrorBox("A process is running in the background. Wait for it to finish");
                return;
            }
            bool check = true;
            try { DriveDecryptionListBox.SelectedItem.ToString().Equals(""); }
            catch { check = false; }
            if (check)
            {
                List<string> Agms = new List<string>();
                Agms.Add(DriveDecryptionListBox.SelectedItem.ToString());
                Agms.Add(DriveDecryptionPassword.Password);
                CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Decrypting Drive... Please wait")));
                BackgroundWorker bgWorker;
                bgWorker = new BackgroundWorker();
                bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
                bgWorker.DoWork += new DoWorkEventHandler(DriveDecryptionbgWorker_DoWork);
                bgWorker.RunWorkerAsync(Agms);
                BWs.Add(bgWorker);
                Working = true;

            }
            else
            {
                ErrorBox("No Drive Selected");
            }
        }
        private void NewLocation_Click(object sender, RoutedEventArgs e)
        {
            if (FileDecNameDisplay.Text.Equals("Not selected"))
            {
                ErrorBox("No File Selected");
            }
            else
            {
                try
                {
                    List<string> Agms = new List<string>();
                    Agms.Add(FileDecNameDisplay.Text);
                    Agms.Add(FileDecryptionPassword.Password);

                    if (!(bool)CheckFileDec.IsChecked)
                    {
                        Agms.Add("Delete No Err");
                    }
                    else
                    {
                        Agms.Add("Err");
                    }
                    if (!App.FileEncryptPassErr)
                    {
                        Agms.Add("No Err");
                    }
                    else
                    {
                        Agms.Add("Err");
                    }
                    if (Working)
                    {
                        ErrorBox("A process is running in the background. Wait for it to finish");
                        return;
                    }
                    App.FileEncryptPassErr = false;
                    SaveFileDialog SFD = new SaveFileDialog();
                    string fileloc= FileDecNameDisplay.Text;
                    SFD.FileName = fileloc.Replace(".Crypt", "");
                    SFD.AddExtension = true;
                    if ((bool)SFD.ShowDialog())
                    {
                        if (SFD.FileName.Equals(""))
                        {
                            ErrorBox("No location selected");
                        }
                        else
                        {
                            Agms.Add(SFD.FileName);
                        }
                    }
                    CryptStatus.Document.Blocks.Add(new Paragraph(new Run("Decrypting... Please wait")));
                    BackgroundWorker bgWorker;
                    bgWorker = new BackgroundWorker();
                    bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
                    bgWorker.DoWork += new DoWorkEventHandler(NewLocFileDecryptionbgWorker_DoWork);
                    bgWorker.RunWorkerAsync(Agms);
                    BWs.Add(bgWorker);
                    Working = true;

                }
                catch (Exception ex)
                {

                    if (ex.Message.Contains("Padding is invalid"))
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
        #endregion

        private void DefaultPassword_Click(object sender, RoutedEventArgs e)
        {
            ChangeDefaultPassword c = new ChangeDefaultPassword();
            c.Show();
        }

        private void Close(object sender, CancelEventArgs e)
        {
            if (Working)
            {
                if(MessageBox.Show("A process is running in the background. Do you want to close?", "Wait", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
                else
                {
                    e.Cancel=true;
                }
            }
        }

       
    }
}
