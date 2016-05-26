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
    public partial class Form4 : Form
    {
        private double rate = 1;
        Point? lastPoint;
        bool fullFlag = false;
        public Form4(Image img)
        {
            InitializeComponent();
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.mouseWheel);
            pictureBox1.Image = img;
            this.WindowState = FormWindowState.Maximized;
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            //panel1.Dock = DockStyle.None;
            //panel1.AutoSize = true;
            if (fullFlag)
            {
                fullFlag = false;
                pictureBox1.Dock = DockStyle.Fill;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                pictureBox1.Dock = DockStyle.None;
                pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
                pictureBox1.Width = pictureBox1.Image.Width;
                pictureBox1.Height = pictureBox1.Image.Height;
                fullFlag = true;
            }
            

        }

        private void mouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) 
        {
            if (e.Delta < 0)
                rate *= 1.2;
            else
                rate /= 1.2;
            if (rate < 1)
                rate = 1;
            pictureBox1.Width = (int)(pictureBox1.Image.Width/rate);
            pictureBox1.Height = (int)(pictureBox1.Image.Height/rate);
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (lastPoint == null)
                return;
            int tmpValue;

            tmpValue = panel1.VerticalScroll.Value;
            tmpValue -= (Control.MousePosition.Y - lastPoint.Value.Y);
            if (tmpValue > panel1.VerticalScroll.Maximum)
                tmpValue = panel1.VerticalScroll.Maximum;

            if (tmpValue < panel1.VerticalScroll.Minimum)
                tmpValue = panel1.VerticalScroll.Minimum;
            panel1.VerticalScroll.Value = tmpValue;

            tmpValue = panel1.HorizontalScroll.Value;
            tmpValue -= (Control.MousePosition.X - lastPoint.Value.X);
            if (tmpValue > panel1.HorizontalScroll.Maximum)
                tmpValue = panel1.HorizontalScroll.Maximum;
            if (tmpValue < panel1.HorizontalScroll.Minimum)
                tmpValue = panel1.HorizontalScroll.Minimum;
            panel1.HorizontalScroll.Value = tmpValue;


            lastPoint = Control.MousePosition;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            lastPoint = null;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = e.Location;
        }

        private void pictureBox1_MouseDown_1(object sender, MouseEventArgs e)
        {
            lastPoint = Control.MousePosition;
        }
    }
}
