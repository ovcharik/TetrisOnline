using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Pages
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainWindow Owner { get; set; }
        public MainPage(MainWindow owner)
        {
            this.Owner = owner;
            InitializeComponent();
        }

        public void OnRaiseUpdateId(object sender, String json)
        {
            MessageBox.Show(json);
        }
    }
}
