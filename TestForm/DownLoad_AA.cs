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
    public partial class DownLoad_AA : Form
    {
        public int DownloadSize = 0;
        public string downloadPath = null;
        long totalSize = 0;
        const int BufferSize = 2048;
        byte[] BufferRead = new byte[BufferSize];
        FileStream fileStream = null;

        CancellationTokenSource cts = null;


        public DownLoad_AA()
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
        }

        private async void buttonDownload_Click(object sender, EventArgs e)
        {
            fileStream = new FileStream(downloadPath, FileMode.OpenOrCreate);

            fileStream.Seek(DownloadSize, SeekOrigin.Begin);
            buttonDownload.Enabled = false;
            buttonPause.Enabled = true;

            cts = new CancellationTokenSource();

            await DownloadFileAsync(textBoxAddress.Text.Trim(), cts.Token,
                new Progress<int>(p => progressBar1.Value = p));

        }

        private async Task DownloadFileAsync(string url, CancellationToken token, IProgress<int> progress)
        {
            try
            {
                var myWebRequest = (HttpWebRequest)WebRequest.Create(url);
                if (DownloadSize != 0)
                {
                    myWebRequest.AddRange(DownloadSize);
                }
                var myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                var responseStream = myWebResponse.GetResponseStream();

                int readSize = 0;
                while (true)
                {
                    if (cts.IsCancellationRequested)
                    {
                        MessageBox.Show($"下载暂停，下载的文件地址为：{downloadPath}，" +
                            $"\n已下载的字节数为：{DownloadSize}字节");
                        myWebResponse.Close();
                        fileStream.Close();

                        buttonDownload.Enabled = true;
                        buttonPause.Enabled = false;
                        break;
                    }

                    readSize = await responseStream.ReadAsync(BufferRead, 0, BufferRead.Length);
                    if (readSize > 0)
                    {
                        DownloadSize += readSize;
                        int percentComplete = (int)((float)DownloadSize / (float)totalSize * 100);
                        fileStream.Write(BufferRead, 0, readSize);

                        progress.Report(percentComplete);
                    }
                    else
                    {
                        MessageBox.Show($"下载已完成，下载地址为：{downloadPath}，文件的总字节数为：{totalSize}");
                        fileStream.Close();

                        buttonPause.Enabled = false;
                        buttonDownload.Enabled = false;

                        myWebResponse.Close();
                        fileStream.Close();
                        break;
                    }
                }
            }
            catch (AggregateException ex)
            {
                ex.Handle(e => e is OperationCanceledException);
            }
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            cts.Cancel();
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
