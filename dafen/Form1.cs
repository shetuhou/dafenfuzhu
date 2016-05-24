using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dafen
{
    using System.Resources;
    using MSExcel = Microsoft.Office.Interop.Excel;
    enum Weidu {P, O, F, S, T }


    
    

    public partial class Form1 : Form
    {
        const string fileName = "scores.xlsx";
        string fullFileName;
        MSExcel.Workbook excelWb;
        MSExcel.Application appExcel = new MSExcel.Application();
        MSExcel.Worksheet excelWs;

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
        int photoOffset;



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
                        Regex reg = new Regex("([0-9]+)[^0-9]+?([0-9]+)[^0-9]+?([0-9]+)\\.JPG");
                        Match match = reg.Match(NextFile.Name);
                        if (match.Success)
                        {
                            string userId = int.Parse(match.Groups[2].Value).ToString();
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
                    {
                        Console.WriteLine(ex);
                    }
                }

            }
            
            InitializeComponent();
            currentPosition = 0;
            currentUserId = 0;
            currentUser = userList[currentUserId];
            currentWeidu = Weidu.P;
            
            comboBox1.SelectedIndex = 0;
            comboBox2.DataSource = userList;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {          
            if (appExcel == null)
            {
                MessageBox.Show("Excel没有安装，结果无法保存!!");
                this.Close();
                return;
            }
            appExcel.DisplayAlerts = false;

            fullFileName = foldPath + "/" + fileName;
            Object nothing = Missing.Value;
            if (File.Exists(fullFileName))
            {
                excelWb = appExcel.Workbooks.Open(fullFileName);

                excelWs = excelWb.Worksheets[1];

                MSExcel.Range range = excelWs.UsedRange;

                for (int rCnt = 2; rCnt <= range.Rows.Count; rCnt++)
                {
                    for (int cCnt = 2; cCnt < 2 + 10; cCnt++)
                    {
                        string dataStr = ((MSExcel.Range)excelWs.Cells[rCnt, cCnt]).Text.ToString();

                        int offset;
                        int score;

                        for (Weidu cWd = Weidu.P; cWd < Weidu.T; cWd = cWd + 1)
                        {
                            offset = dataStr.ToUpper().IndexOf(cWd.ToString());
                            if (offset < 0)
                                continue;
                            score = int.Parse(dataStr.Substring(offset + 1, 1));
                            scoreList[((MSExcel.Range)excelWs.Cells[rCnt, 1]).Text.ToString()][(int)cWd, cCnt - 2] = score;
                        }
                        
                    }
                }
                //excelWb.Save();
                
            }
            else
            {              
                excelWb = appExcel.Workbooks.Add(nothing);
                Object format = MSExcel.XlFileFormat.xlWorkbookDefault;

                excelWs = excelWb.Worksheets[1];
                excelWs.Cells[1, 1] = "用户编号";

                for (int i = 0; i < 10; i++)
                    excelWs.Cells[1, i+2] = "位置" + (i+1);

                for (int i = 0; i < userList.Count; i++)
                {
                    excelWs.Cells[i + 2, 1] = userList[i];
                }

                for (int i = 1; i < 10; i++)
                    for (int j = 1; j < userList.Count; j++)
                    {
                        excelWs.Cells[j + 1, i + 1] = "";
                    }
                excelWb.SaveAs(fullFileName, nothing, nothing, nothing, nothing, nothing, MSExcel.XlSaveAsAccessMode.xlExclusive, nothing, nothing, nothing, nothing, nothing);
            }
            switchWeidu();
            switchPhoto();

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
            if (comboBox4.SelectedIndex < 0)
                return;
            scoreList[userList[currentUserId]][(int)currentWeidu,currentPosition] = comboBox4.SelectedIndex;
            string tmpStr;

            try
            {
                tmpStr = ((MSExcel.Range)excelWs.Cells[currentUserId + 2, currentPosition + 2]).Text.ToString();
            }
            catch (Exception ex)
            {
                excelWb = appExcel.Workbooks.Open(fullFileName);

                excelWs = excelWb.Worksheets[1];
                tmpStr = ((MSExcel.Range)excelWs.Cells[currentUserId + 2, currentPosition + 2]).Text.ToString();
            }
            
            
            string resStr;
            int offset = tmpStr.IndexOf(currentWeidu.ToString());
            if (offset < 0)
            {
                resStr = tmpStr + currentWeidu.ToString() + comboBox4.SelectedIndex;
            }
            else
            {
                resStr = tmpStr.Substring(0, offset) + currentWeidu.ToString() + comboBox4.SelectedIndex + tmpStr.Substring(offset + 2);
            }
            excelWs.Cells[currentUserId + 2, currentPosition + 2] = resStr;
            excelWb.Save();
        }   

        private void switchWeidu()
        {

            label5.Text = desc[(int)currentWeidu];
            if (currentWeidu == Weidu.T)
                button2.Visible = true;
            else
                button2.Visible = false;

            ResourceManager manager = new ResourceManager(typeof(Resource1));
            string resName;

            resName = currentWeidu.ToString() + 0;
            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();
            pictureBox1.Image = (Image)manager.GetObject(resName);

            resName = currentWeidu.ToString() + 1;
            if (pictureBox2.Image != null)
                pictureBox2.Image.Dispose();
            pictureBox2.Image = (Image)manager.GetObject(resName);

            resName = currentWeidu.ToString() + 2;
            if (pictureBox3.Image != null)
                pictureBox3.Image.Dispose();
            pictureBox3.Image = (Image)manager.GetObject(resName);

            resName = currentWeidu.ToString() + 3;
            if (pictureBox4.Image != null)
                pictureBox4.Image.Dispose();
            pictureBox4.Image = (Image)manager.GetObject(resName);

            resName = currentWeidu.ToString() + 4;
            if (pictureBox5.Image != null)
                pictureBox5.Image.Dispose();
            pictureBox5.Image = (Image)manager.GetObject(resName);
        }

        private void switchPhoto()
        {
            photoOffset = 0;

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
            if (pictureBox6.Image != null)
                pictureBox6.Image.Dispose();
            try
            {
                pictureBox6.Image = Image.FromFile(foldPath + "/" + fileList[userList[currentUserId]][photoOffset]);
            }
            catch (Exception ex)
            {
                pictureBox6.Image = null;
                Console.WriteLine(ex);
            }
            
            if (scoreList[userList[currentUserId]][(int)currentWeidu,currentPosition] > 4)
                comboBox4.SelectedIndex = -1;
            else
                comboBox4.SelectedIndex = scoreList[userList[currentUserId]][(int)currentWeidu, currentPosition];

            try {
                label11.Text = fileList[userList[currentUserId]][photoOffset];
            }
            catch (Exception ex)
            {
                label11.Text = "图片不存在";
                Console.WriteLine(ex);
            }

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
            switchPhoto();
            switchWeidu();
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
                        MessageBox.Show("已经是最后一张");
                    }
                    switchWeidu();
                }
            }
            switchPhoto();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                excelWb.Save();
                excelWb.Close();
            }
            catch (Exception ex)
            {

            }
            appExcel.Quit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox6.Image.Dispose();
            photoOffset = photoOffset - 1 + (photoOffset % 2) * 2;
            pictureBox6.Image = Image.FromFile(foldPath + "/" + fileList[currentUser][photoOffset]);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //get next
            if (currentPosition > 0)
                currentPosition--;
            else
            {
                currentPosition = 9;
                if (currentUserId > 0)
                {
                    currentUserId--;
                }
                else
                {
                    currentUserId = userList.Count - 1;
                    if (currentWeidu > Weidu.P)
                    {
                        currentWeidu = currentWeidu - 1;
                    }
                    else
                    {
                        //end!!!!
                        currentWeidu = Weidu.T;
                    }
                    switchWeidu();
                }
            }
            switchPhoto();
        }

        private void pictureBox6_DoubleClick(object sender, EventArgs e)
        {
            ////建立新的系统进程  
            //System.Diagnostics.Process process = new System.Diagnostics.Process();
            ////设置文件名，此处为图片的真实路径+文件名  
            //process.StartInfo.FileName = foldPath + "/" + fileList[currentUser][photoOffset];
            ////此为关键部分。设置进程运行参数，此时为最大化窗口显示图片。  
            //process.StartInfo.Arguments = "rundll32.exe C://WINDOWS//system32//shimgvw.dll,ImageView_Fullscreen";
            ////此项为是否使用Shell执行程序，因系统默认为true，此项也可不设，但若设置必须为true  
            //process.StartInfo.UseShellExecute = true;
            ////此处可以更改进程所打开窗体的显示样式，可以不设  
            //process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //process.Start();
            //process.Close();
            Form3 f3 = new Form3(pictureBox6.Image);
            this.Hide();
            f3.ShowDialog();
            this.Show();
        }

        private void small_DoubleClick(object sender, EventArgs e)
        {
            Form3 f3 = new Form3(((PictureBox)sender).Image);
            //this.Hide();
            f3.ShowDialog();
            this.Show();
        }
    }
}
