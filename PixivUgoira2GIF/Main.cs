using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gif.Components;
using System.IO;
using System.Threading;

namespace Ugoira2GIF
{
    public partial class Ugoira2GIF : Form
    {
        int repeat = 0;
        int qua = 10;
        int delay = 100;
        public Ugoira2GIF()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Move;
            else e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            Thread workThread = new Thread(delegate()
            {
                string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                string dir = System.IO.Path.GetDirectoryName(path);//目錄名稱
                string ext = System.IO.Path.GetExtension(path);
                string nam = System.IO.Path.GetFileNameWithoutExtension(path);
                if (ext != ".zip")
                {
                    MessageBox.Show("請拖入zip格式檔案");
                }
                else
                {

                    this.label1.Text = "正在解壓";
                    System.Threading.Thread.Sleep(500);
                    ZIP.UnpackFiles(path, dir + "/" + nam);
                    this.label1.Text = "解壓完畢";
                    System.Threading.Thread.Sleep(500);
                    this.label1.Text = "正在生成GIF";
                    System.Threading.Thread.Sleep(500);
                    try
                    {
                        string[] jpgImgNames = Directory.GetFiles(dir + "/" + nam); // 获取各帧图片的文件名
                        string dtStr = DateTime.Now.ToString("yyyyMMddhhmmss");
                        string outPutPath = string.Format(dir + "/" + nam + ".gif", dtStr); // 生成要保存成的gif图片的文件名
                        // 创建动画（gif）
                        AnimatedGifEncoder animate = new AnimatedGifEncoder();
                        animate.Start(outPutPath);
                        animate.SetDelay(delay);
                        animate.SetRepeat(repeat); // -1：不重复，0：无限循环
                        animate.SetQuality(qua);
                        int filecount=0;
                        foreach (var item in jpgImgNames)
                        {
                            filecount++;
                            Image imgFrame = Image.FromFile(item);
                            this.label1.Text = "正在添加幀：\r\n" + System.IO.Path.GetFileNameWithoutExtension(item) + "\r\n" + filecount.ToString() + "/" + jpgImgNames.Length.ToString();
                            animate.AddFrame(imgFrame); // 添加帧
                            imgFrame.Dispose();
                        }
                        animate.Finish();
                        this.label1.Text = "生成成功";

                    }
                    catch (Exception ex)
                    {
                        this.label1.Text = "出錯啦：" + ex.Message;
                    }
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(dir + "/" + nam);
                        di.Delete(true);
                    }
                    catch (Exception ex)
                    {
                        this.label1.Text = "出錯啦：" + ex.Message;
                    }
                }
            });
            workThread.Start();
        }

        private void loop_CheckedChanged(object sender, EventArgs e)
        {
            if (loop.Checked)
            {
                repeat = 0;
            }
            else {
                repeat = -1;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                delay = int.Parse(textBox1.Text);
            }
            catch {
                textBox1.Text = "100";
                delay = 100;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                qua = int.Parse(comboBox1.Text);
            }
            catch
            {
                comboBox1.Text = "10";
                qua = 10;
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                qua = int.Parse(comboBox1.Text);
            }
            catch
            {
                comboBox1.Text = "10";
                qua = 10;
            }
        }
    }
}
