using DBeaverAutoUpdater.Core.BLL;
using GenericCore.Support;
using GenericCore.Support.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBeaverAutoUpdater.GUI
{
    public partial class UpdateForm : Form
    {
        IConfigBLL configBLL = new ConfigBLL();
        IUpdateBLL updateBLL = new UpdateBLL();

        public UpdateForm()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            IProgress<double> progress =
                new Progress<double>
                (n =>
                {
                    //if (progressBar1.Value == progressBar1.Maximum)
                    //{
                    //    MessageBox.Show("Completed");
                    //    return;
                    //}

                    progressBar1.Value = Math.Truncate(n).ConvertTo<int>();
                });

            IProgress<double> progress2 =
                new Progress<double>
                (n =>
                {
                    //if (progressBar2.Value == progressBar1.Maximum)
                    //{
                    //    MessageBox.Show("Completed");
                    //    return;
                    //}

                    progressBar2.Value = Math.Truncate(n).ConvertTo<int>();
                });

            //byte[] file = await WebDataRetriever.DownloadFileAsync("https://dbeaver.io/files/dbeaver-ce-latest-win32.win32.x86_64.zip", progress);
            byte[] file = File.ReadAllBytes(@"C:\temp\dbeaver.zip");
            updateBLL.UnzipArchive(file, @"C:\temp", progress2);
        }
    }
}
