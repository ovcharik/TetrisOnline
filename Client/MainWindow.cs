using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void UpdateUserId(object sender, String s)
        {
            MessageBox.Show(s);
        }



        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Owner != null) this.Owner.Show();
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            if (this.Owner != null) this.Owner.Hide();
        }
    }
}
