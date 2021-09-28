using CefSharp;
using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Ao3Page : Form
    {
        public Ao3Page()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            toolStripButton1.Enabled = false;
            toolStripLabel1.Text = "0 Categories Tracked";
            chromiumWebBrowser1.Load("https://archiveofourown.org/tags/Fleur%20Delacour*s*Harry%20Potter/works");
            chromiumWebBrowser1.MenuHandler = new ConextMenuHandler();
            for (int i = 0; i < Program.Track.Count; i++)
            {
                toolStripDropDownButton1.DropDownItems.Add(Program.Track[i].Name);
            }
        }

        private void chromiumWebBrowser1_AddressChanged(object sender, CefSharp.AddressChangedEventArgs e)
        {
            //UpdateButton();
            this.Invoke((MethodInvoker)delegate
            {
                toolStripDropDownButton1.DropDownItems.Clear();
                toolStripButton1.Text = "Add To Tracking";
                for (int i = 0; i < Program.Track.Count; i++)
                {
                    toolStripDropDownButton1.DropDownItems.Add(Program.Track[i].Name);
                }
                for (int i = 0; i < Program.Track.Count; i++)
                {
                    if (chromiumWebBrowser1.Address == Program.Track[i].Link)
                    {
                        toolStripButton1.Text = "Remove From Tracking";
                        toolStripLabel1.Text = Program.Track.Count + " Categories Tracked";
                    }
                }
                if ((chromiumWebBrowser1.Address.Contains("https://archiveofourown.org/tags") || chromiumWebBrowser1.Address.Contains("https://archiveofourown.org/works?") || chromiumWebBrowser1.Address.Contains("https://archiveofourown.org/collections") || chromiumWebBrowser1.Address.Contains("https://archiveofourown.org/users/")) && !chromiumWebBrowser1.Address.Contains("?page=") && chromiumWebBrowser1.Address.Contains("/works"))
                {
                    toolStripButton1.Enabled = true;
                }
                else
                {
                    toolStripButton1.Enabled = false;
                }
            });
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (toolStripButton1.Text == "Add To Tracking")
            {
                toolStripButton1.Text = "Remove From Tracking";
                Program.RemoveAddTracker(chromiumWebBrowser1.Address, true);
            }
            else
            {
                toolStripButton1.Text = "Add To Tracking";
                Program.RemoveAddTracker(chromiumWebBrowser1.Address, false);
            }
        }

        public void UpdateButton()
        {
            this.Invoke((MethodInvoker)delegate
            {
                toolStripButton1.Text = "Add To Tracking";
                toolStripDropDownButton1.DropDownItems.Clear();
            });
            for (int i = 0; i < Program.Track.Count; i++)
            {
                if (chromiumWebBrowser1.Address == Program.Track[i].Link)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        toolStripButton1.Text = "Remove From Tracking";
                        toolStripLabel1.Text = Program.Track.Count + " Categories Tracked";
                    });
                }
            }
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            UpdateButton();
        }

        private void toolStripDropDownButton1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            for (int i = 0; i < Program.Track.Count; i++)
            {
                if (e.ClickedItem.Text == Program.Track[i].Name)
                {
                    chromiumWebBrowser1.Load(Program.Track[i].Link);
                }
            }
        }


        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }
    }

    public class ConextMenuHandler : IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();
            model.AddItem(CefMenuCommand.Find, "Add To Tracking");
            model.SetEnabledAt(0, !string.IsNullOrEmpty(parameters.LinkUrl)&&parameters.LinkUrl.Contains("https://app.salad.io/rewards/"));
        }

        public bool OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if (commandId == CefMenuCommand.Find)
            {
                string Address = "https://app-api.salad.io/api/v1/rewards/" + parameters.LinkUrl.Substring(29);
                return true;
            }
            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
        {
        }

        public bool RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
}
