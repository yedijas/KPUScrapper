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

namespace KPUScrapper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(DirectoryTextBox.Text) && !String.IsNullOrWhiteSpace(DirectoryTextBox.Text))
                {
                    if (!Directory.Exists(DirectoryTextBox.Text))
                    {
                        Directory.CreateDirectory(DirectoryTextBox.Text);
                    }
                    Helper.Downloader downloader = new Helper.Downloader();
                    downloader.Download(DirectoryTextBox.Text);
                    MessageBox.Show("Selamat! Download berhasil! Silakan cek folder " + DirectoryTextBox.Text + " untuk melihat hasil!");
                }
                else
                {
                    MessageBox.Show("Please fill the Save To path first!",
                            "Hey!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Close your opened file first.",
                    "Hey!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Access to file is denied. Please restart the program as Administrator.",
                    "Hey!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }
    }
}
