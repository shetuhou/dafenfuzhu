using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dafen
{
    enum Weidu {P, O, F, S, T }

    

    public partial class Form1 : Form
    {
        string[] desc = new string[5]{
            "色素\r\n0 无可辨色素斑\r\n1 可见1-2个点状色素斑\r\n2 可见较多点状色素斑或小片状色素网\r\n3 可见大片色素网，存在小片无色素沉着区域\r\n4 可见粗大色素带，或弥漫色素网",
            "油光\r\n0 可见较多环形鳞屑(白色圆形)\r\n1 可见少量环形鳞屑或色泽暗淡\r\n2 可见细小油光闪烁(白色小点)\r\n3 可见较多油光闪烁(白色不规则片)\r\n4 较多油脂浸润区",
            "毛孔\r\n0 未见扩张毛孔\r\n1 可见1-2个扩张毛孔\r\n2 可见较多扩张毛孔\r\n3 弥漫扩张毛孔，或毛孔直径较大\r\n4 弥漫扩张毛孔及毛孔直径较大\r\n",
            "炎症\r\n0 未见红斑及血管扩张\r\n1 轻微带淡红色\r\n2 可见淡红色背景\r\n3 可见深红背景，或者可见扩张血管\r\n4 可见深红色背景及扩张的血管\r\n",
            "纹理\r\n0 未见可辨认纹理\r\n1 非偏振光边缘可见刚可辨认的皮沟皮脊\r\n2 非偏振光全视野可见细小皮沟皮脊\r\n3 非偏振光可见粗大皮沟皮脊\r\n4 偏振光照片下可辨认粗大的皮沟皮脊\r\n"
};
        Dictionary<String, Dictionary<int, String>> fileList = new Dictionary<String, Dictionary<int, String>>();
        Dictionary<String, int[,]> scoreList = new Dictionary<String, int[,]>();

        List<String> userList = new List<string>();
        String currentUser;
        int currentUserId;
        int currentPosition;
        Weidu currentWeidu;
        string foldPath;

        
        public Form1()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择打分图片路径";
            while (true)
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foldPath = dialog.SelectedPath;
                    DirectoryInfo TheFolder = new DirectoryInfo(foldPath);
                    foreach (FileInfo NextFile in TheFolder.GetFiles())
                    {
                        Regex reg = new Regex("([0-9]+).*?([0-9]+).*?([0-9]+).JPG");
                        Match match = reg.Match(NextFile.Name);
                        if (match.Success)
                        {
                            string userId = match.Groups[2].Value;
                            string photoId = match.Groups[3].Value;
                            if (userList.Contains(userId) == false)
                            {
                                userList.Add(userId);
                                scoreList.Add(userId, new int[5, 10]);
                                for (int i = 0; i < 5; i++)
                                    for (int j = 0; j < 10; j++)
                                    {
                                        scoreList[userId][i, j] = 0xff;
                                    }
                            }

                            if (fileList.ContainsKey(userId) == false)
                                fileList.Add(userId, new Dictionary<int, String>());
                            fileList[userId].Add(int.Parse(photoId), match.Groups[0].Value);
                        }

                    }
                    if (fileList.Count != 0)
                        break;
                    else
                        MessageBox.Show("未找到图片文件");

                }
                else
                {
                    try
                    {
                        System.Environment.Exit(0);
                    }
                    catch (Exception ex)
                    { }
                }

            }
            
            InitializeComponent();
            currentPosition = 0;
            currentUserId = 0;
            currentUser = userList[currentUserId];
            currentWeidu = Weidu.P;
            pictureBox6.Image = Image.FromFile(foldPath + "/" + fileList[currentUser][currentPosition + 1]);
            comboBox1.SelectedIndex = 0;
            comboBox2.DataSource = userList;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            label5.Text = desc[(int)currentWeidu];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            tableLayoutPanel1.Height = this.Width * 9 / 16;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            scoreList[userList[currentUserId]][(int)currentWeidu,currentPosition] = comboBox4.SelectedIndex;
            

        }

        private void switchPhoto()
        {
            int photoOffset = 0;

            comboBox1.SelectedIndex = (int)currentWeidu;
            comboBox2.SelectedIndex = currentUserId;
            comboBox3.SelectedIndex = currentPosition;
            switch (currentWeidu)
            {
                case Weidu.P:
                case Weidu.F:
                case Weidu.S:
                    photoOffset = currentPosition * 2 + 1;
                    break;
                case Weidu.O:
                case Weidu.T:
                    photoOffset = currentPosition * 2 + 2;
                    break;
            }
            pictureBox6.Image.Dispose();
            pictureBox6.Image = Image.FromFile(foldPath + "/" + fileList[currentUser][photoOffset]);
            if (scoreList[userList[currentUserId]][(int)currentWeidu,currentPosition] > 4)
                comboBox4.Text = "";
            else
                comboBox4.Text = scoreList[userList[currentUserId]][(int)currentWeidu, currentPosition].ToString();


        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPosition = comboBox3.SelectedIndex;
            switchPhoto();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentUserId = comboBox2.SelectedIndex;
            switchPhoto();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentWeidu = (Weidu)comboBox1.SelectedIndex;
            label5.Text = desc[(int)currentWeidu];
            switchPhoto();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //get next
            if (currentPosition < 9)
                currentPosition++;
            else
            {
                currentPosition = 0;
                if (currentUserId < (userList.Count - 1))
                {
                    currentUserId++;
                }
                else
                {
                    currentUserId = 0;
                    if (currentWeidu < Weidu.T)
                    {
                        currentWeidu = currentWeidu + 1;
                    }
                    else
                    {
                        //end!!!!
                        currentWeidu = Weidu.P;
                    }
                    label5.Text = desc[(int)currentWeidu];
                }
            }
            switchPhoto();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
