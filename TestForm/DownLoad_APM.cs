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
    public partial class DownLoad_APM : Form
    {
        public DownLoad_APM()
        {
            InitializeComponent();
            textBoxAddress.Text = "https://dldir1.qq.com/qqfile/qq/PCTIM/TIM3.3.5/TIM3.3.5.22018.exe";
        }

        private delegate string AsyncMethodCaller(string fileurl);

        SynchronizationContext sc;

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            richTextBoxMessage.Text = "下载中……";
            buttonDownload.Enabled = false;
            if (textBoxAddress.Text == string.Empty)
            {
                MessageBox.Show("请先输入地址！");
                return;
            }
            //获得调用线程的同步上下文对象
            sc = SynchronizationContext.Current;
            AsyncMethodCaller mc = new AsyncMethodCaller(DownLoadFileAsync);
            mc.BeginInvoke(textBoxAddress.Text.Trim(), GetResult, null);
        }

        private string DownLoadFileAsync(string fileurl)
        {
            int BufferSize = 2048;
            byte[] BufferRead = new byte[BufferSize];
            string savePath = Path.Combine(@"D:\soft", Path.GetFileName(fileurl));
            HttpWebResponse myWebResponse = null;
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate);
            try
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(fileurl);
                if (myHttpWebRequest != null)
                {
                    myWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                    Stream responseStream = myWebResponse.GetResponseStream();
                    int readSize = responseStream.Read(BufferRead, 0, BufferSize);
                    while (readSize > 0)
                    {
                        fileStream.Write(BufferRead, 0, readSize);
                        readSize = responseStream.Read(BufferRead, 0, BufferSize);
                    }
                }
                return $"文件下载完成，文件大小为:{fileStream.Length}, 下载路径为:{savePath}";
            }
            catch(Exception ex)
            {
                return $"下载过程中发生异常，异常信息为：{ex.Message}";
            }
            finally
            {
                if (myWebResponse != null)
                {
                    myWebResponse.Close();
                }
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        private void GetResult(IAsyncResult result)
        {
            AsyncMethodCaller caller = (AsyncMethodCaller)((AsyncResult)result).AsyncDelegate;
            string returnString = caller.EndInvoke(result);
            sc.Post(ShowState, returnString);
        }

        private void ShowState(object state)
        {
            richTextBoxMessage.Text = state.ToString();
            buttonDownload.Enabled = true;
        }
    }
}
