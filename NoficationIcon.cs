using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CefSharp;

namespace WindowsFormsApp1
{
    class NoficationIcon : ApplicationContext
    {
        //Component declarations
        private NotifyIcon TrayIcon;
        private ContextMenuStrip TrayIconContextMenu;
        private ToolStripMenuItem SaladStore;
        private ToolStripMenuItem Settings;
        private ToolStripMenuItem Exit;
        public static Ao3Page ao3Page;
        public static Settings settings;

        public NoficationIcon()
        {
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            InitializeComponent();
            TrayIcon.Visible = true;
            TrayIcon.Text = "Ao3 Listener";
            TrayIcon.BalloonTipText = "Program has Started";
            TrayIcon.BalloonTipTitle = "Ao3 Listener";
            TrayIcon.ShowBalloonTip(5);
        }

        public void Notification(string work, string notifcation = "Error 404", string link = "Error 321")
        {
            TrayIcon.BalloonTipText = notifcation;
            TrayIcon.BalloonTipTitle = work;
            //TrayIcon.BalloonTipClicked += new EventHandler((sender, e) => OpenWebbrowser(link));
            TrayIcon.ShowBalloonTip(10);
        }

        private void InitializeComponent()
        {
            Program.StartChecking();
            TrayIcon = new NotifyIcon();
            TrayIcon.Icon = Properties.Resources.Icon1;
            TrayIconContextMenu = new ContextMenuStrip();
            TrayIconContextMenu.SuspendLayout();

            Exit = new ToolStripMenuItem();
            this.Exit.Text = "Exit";
            this.Exit.Size = new Size(152, 22);
            this.Exit.Click += new EventHandler(this.Exit_Click);

            Settings = new ToolStripMenuItem();
            this.Settings.Name = "Settings";
            this.Settings.Size = new Size(152, 22);
            this.Settings.Text = "Settings";
            this.Settings.Click += new EventHandler(this.Settings_Click);

            SaladStore = new ToolStripMenuItem();
            this.SaladStore.Name = "Ao3 Page";
            this.SaladStore.Size = new Size(152, 22);
            this.SaladStore.Text = "Ao3 Page";
            this.SaladStore.Click += new EventHandler(this.Store_Click);

            this.TrayIconContextMenu.Items.AddRange(new ToolStripItem[] { SaladStore, Settings, Exit});
            this.TrayIconContextMenu.Name = "TrayIconContextMenu";
            this.TrayIconContextMenu.Size = new Size(153, 70);

            TrayIconContextMenu.ResumeLayout(false);
            TrayIcon.ContextMenuStrip = TrayIconContextMenu;
        }

        public void OpenWebbrowser(string url)
        {
            Process.Start(url);
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            //Cleanup so that the icon will be removed when the application is closed
            TrayIcon.Visible = false;
        }

        private void Store_Click(object sender, EventArgs e)
        {
            ao3Page = new Ao3Page();
            ao3Page.Show();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            //CefSharp.Cef.Shutdown();
            Application.Exit();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            settings = new Settings();
            settings.Show();
        }
    }

    public class AutoClosingMessageBox
    {
        System.Threading.Timer _timeoutTimer;
        string _caption;
        AutoClosingMessageBox(string text, string caption, int timeout)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            using (_timeoutTimer)
                MessageBox.Show(text, caption);
        }
        public static void Show(string text, string caption, int timeout)
        {
            new AutoClosingMessageBox(text, caption, timeout);
        }
        void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
        }
        const int WM_CLOSE = 0x0010;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }
}
