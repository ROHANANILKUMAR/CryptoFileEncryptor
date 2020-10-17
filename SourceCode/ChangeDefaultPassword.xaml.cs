using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileEncryptionServices
{
    /// <summary>
    /// Interaction logic for ChangeDefaultPassword.xaml
    /// </summary>
    public partial class ChangeDefaultPassword : Window
    {
        public ChangeDefaultPassword()
        {
            InitializeComponent();
            string udata = File.ReadAllText(App.Set);
            if (udata.Split(':')[0].Equals(Encryption.Encrypt("OFF")))
            {
                StateBtn.Content = "OFF";
                OnGrid.Visibility = Visibility.Hidden;
            }
            if (udata.Split(':')[0].Equals(Encryption.Encrypt("ON")))
            {
                StateBtn.Content = "ON";
                Password.Password = Encryption.Decrypt(udata.Split(':')[1]);
                OnGrid.Visibility = Visibility.Visible;
            }
        }

        private void StateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StateBtn.Content.Equals("OFF"))
            {
                string udata = File.ReadAllText(App.Set);
                string[] sdata = udata.Split(':');
                sdata[0] = Encryption.Encrypt("ON");
                File.WriteAllText(App.Set,String.Join(":",sdata));
                StateBtn.Content = "ON";
                Password.Password = Encryption.Decrypt(udata.Split(':')[1]);
                App.DefaultPassword = true;
                OnGrid.Visibility = Visibility.Visible;
            }
            else if (StateBtn.Content.Equals("ON"))
            {
                string udata = File.ReadAllText(App.Set);
                string[] sdata = udata.Split(':');
                sdata[0] = Encryption.Encrypt("OFF");
                File.WriteAllText(App.Set, String.Join(":", sdata));
                StateBtn.Content = "OFF";
                App.DefaultPassword = false;
                OnGrid.Visibility = Visibility.Hidden;
            }
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            string udata = File.ReadAllText(App.Set);
            string[] sdata = udata.Split(':');
            sdata[1] = Encryption.Encrypt(Password.Password);
            File.WriteAllText(App.Set, String.Join(":", sdata));
            App.DefaultPas = Password.Password;
            MainWindow.SuccessBox("Default password is set");
        }
    }
}
