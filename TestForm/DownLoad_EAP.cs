using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class DownLoad_EAP : Form
    {
        public int DownloadSize = 0;
        public string downloadPath = null;
        long totalSize = 0;
        const int BufferSize = 2048;
        byte[] BufferRead = new byte[BufferSize];
        FileStream fileStream = null;
        HttpWebResponse myWebResponse = null;

        public DownLoad_EAP()
        {
            InitializeComponent();
            string url = "https://dldir1.qq.com/qqfile/qq/PCTIM/TIM3.3.5/TIM3.3.5.22018.exe";

            textBoxAddress.Text = url;
            buttonPause.Enabled = false;

            GetTotalSzie();
            downloadPath = Path.Combine(@"D:\soft", Path.GetFileName(textBoxAddress.Text.Trim()));
            if (File.Exists(downloadPath))
            {
                FileInfo fileInfo = new FileInfo(downloadPath);
                DownloadSize = (int)fileInfo.Length;
                progressBar1.Value = (int)((float)DownloadSize / (float)totalSize * 100);
            }

            bgwDownload.WorkerReportsProgress = true;
            bgwDownload.WorkerSupportsCancellation = true;
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            if (!bgwDownload.IsBusy) 
            {
                bgwDownload.RunWorkerAsync();

                fileStream = new FileStream(downloadPath, FileMode.OpenOrCreate);

                fileStream.Seek(DownloadSize, SeekOrigin.Begin);
                buttonDownload.Enabled = false;
                buttonPause.Enabled = true;
            }
            else
            {
                MessageBox.Show("正在执行操作，请稍后");
            }
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (bgwDownload.IsBusy && bgwDownload.WorkerSupportsCancellation)
            {
                bgwDownload.CancelAsync();
            }
        }

        private void bgwDownload_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = sender as BackgroundWorker;
            try
            {
                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(textBoxAddress.Text.Trim());
                if (DownloadSize != 0)
                {
                    myHttpWebRequest.AddRange(DownloadSize);
                }
                myWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                Stream responseStream = myWebResponse.GetResponseStream();
                int readSize = 0;
                while (true)
                {
                    if (bgWorker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }

                    readSize = responseStream.Read(BufferRead, 0, BufferRead.Length);
                    if (readSize > 0)
                    {
                        DownloadSize += readSize;
                        int percentComplete = (int)((float)DownloadSize / (float)totalSize * 100);
                        fileStream.Write(BufferRead, 0, readSize);

                        bgWorker.ReportProgress(percentComplete);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void bgwDownload_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void bgwDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                MessageBox.Show($"下载暂停，下载的文件地址为{downloadPath}，已下载的字节数为{DownloadSize}");
                fileStream.Close();

                buttonDownload.Enabled = true;
                buttonPause.Enabled = false;
            }
            else
            {
                MessageBox.Show($"下载已完成，下载地址为：{downloadPath}，文件的总字节数为：{totalSize}");
                fileStream.Close();

                buttonPause.Enabled = false;
                buttonDownload.Enabled = false;
            }
            myWebResponse.Close();
        }

        private void GetTotalSzie()
        {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(textBoxAddress.Text.Trim());
            HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse();
            totalSize = response.ContentLength;
            response.Close();
        }
    }
}
