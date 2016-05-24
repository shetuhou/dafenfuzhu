using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dafen
{
    public partial class Form3 : Form
    {
        public Form3(Image img)
        {
            InitializeComponent();
            pictureBox1.Image = img;
            this.WindowState = FormWindowState.Maximized;
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
