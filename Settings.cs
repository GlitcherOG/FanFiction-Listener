using System;
using System.Windows.Forms;
using IWshRuntimeLibrary;

namespace WindowsFormsApp1
{
    public partial class Settings : Form
    {
        string file = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\Ao3Listener.lnk";
        public Settings()
        {
            InitializeComponent();
            numericUpDown1.Value = Program.waittime;
            textBox1.Text = Program.Webhook;
            checkBox2.Checked = Program.pageCheck;
            checkBox4.Checked = Program.discordWebhook;
            if (System.IO.File.Exists(file))
            {
                checkBox1.Checked = true;
            }
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            Program.waittime = (int)numericUpDown1.Value;
            Program.Webhook = textBox1.Text;
            Program.pageCheck = checkBox2.Checked;
            Program.discordWebhook = checkBox4.Checked;
            Program.Saving.Save();
            if (checkBox1.Checked)
            {
                WshShell wsh = new WshShell();
                IWshShortcut shortcut = wsh.CreateShortcut(file) as IWshRuntimeLibrary.IWshShortcut;
                shortcut.TargetPath = Application.ExecutablePath;
                shortcut.Save();
            }
            else
            {
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }
            this.Close();
        }
    }
}
