using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace aCis_Loader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("This is a public Beta. Report any issues to LabWare in discord!\n\nMake sure you have 7zip installed!!!", "aCis Loader");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (File.Exists("lineage2_interlude_client.7z"))
            {
                button1.Enabled = false;
                button1.Text = "Unpacking ...";
                label3.Text = "Unpacking ...";
                UnzipGameClient();
            }
            else
            {
                button1.Enabled = false;
                button1.Text = "Downloading ...";
                startDownload();
            }
        }

        private void startDownload()
        {
            Thread thread = new Thread(() =>
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri("http://www.anothercrappyinterludeserver.com/files/client/lineage2_interlude_client.7z"), @"lineage2_interlude_client.7z");
            });
            thread.Start();
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                label3.Text = "Downloaded " + e.BytesReceived + " of " + e.TotalBytesToReceive;
                progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
            });
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                label3.Text = "Completed";
                UnzipGameClient();
                
            });
        }

        private void UnzipGameClient()
        {
            Thread thread2 = new Thread(() =>
            {
                label3.Text = "Unpacking ...";

                string zPath = "C:\\Program Files\\7-Zip\\7z.exe"; //add to proj and set CopyToOuputDir
                try
                {
                    ProcessStartInfo pro = new ProcessStartInfo();
                    pro.WindowStyle = ProcessWindowStyle.Hidden;
                    pro.FileName = zPath;
                    pro.Arguments = string.Format("x \"{0}\" -y -o\"{1}\"", "lineage2_interlude_client.7z", "L2_Interlude");
                    Process x = Process.Start(pro);
                    x.WaitForExit();
                }
                catch (Exception Ex)
                {
                    //handle error
                }
                finally
                {
                    label3.Text = "Job Done!";
                    button1.Enabled = true;
                }

            });
            thread2.Start();
        }
    }
}
