using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using ApiClient.Models;
using ESNLib.Tools;

namespace Example
{
    public partial class ExampleUser : Form, ESNLib.Tools.WinForms.AdminTools.IAdminForm
    {
        private readonly ApiClientWrapper.ApiClientWrapper client = new ApiClientWrapper.ApiClientWrapper();

        public ExampleUser()
        {
            InitializeComponent();
        }

        private void SetState(bool enabled)
        {
            btnAuth.Enabled = btnRefresh.Enabled = enabled;
        }

        private async void StartAuth()
        {
            SetState(false);
            if (await client.GetAccessToken()) // Begin Token recuperation
            {
                var settings = ApiClientSettings.CreateFromConfigFile();
                MessageBox.Show(string.Format("Authorization successfull !\nRefresh Token : {0}\nAccess Token : {1}\nExpiration : {2}",
                    settings.RefreshToken,
                    settings.AccessToken,
                    settings.ExpirationDateTime),
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Unable to complete authorization. Check the logs",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            SetState(true);
        }

        private async void StartRefresh()
        {
            SetState(false);
            if (await client.RefreshAccessToken())
            {
                MessageBox.Show("Refresh successfull !",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Unable to refresh",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            SetState(true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartAuth();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            StartRefresh();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (MiscTools.HasAdminPrivileges())
            {
                if (client.RegisterListener())
                    MessageBox.Show("Registration successfull", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Failed\nIt may be already registered\nOtherwise you can manually register using \"netsh http add urlacl ...\"", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("Admin privileges required. The app will restart and you will have to click the button again.", "Admin required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                try
                {
                    ESNLib.Tools.WinForms.AdminTools.RunAsAdmin(this);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Write(ex.Message, Logger.LogLevels.Error);
                }
            }
            this.Focus();
        }

        private void btnUnregister_Click(object sender, EventArgs e)
        {
            if (MiscTools.HasAdminPrivileges())
            {
                if (client.UnregisterListener())
                    MessageBox.Show("Unregistration successfull", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Failed\nIt may be already unregistered\nOtherwise you can manually register using \"netsh http delete urlacl ...\"", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("Admin privileges required. The app will restart and you will have to click the button again.", "Admin required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                try
                {
                    ESNLib.Tools.WinForms.AdminTools.RunAsAdmin(this);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Write(ex.Message, Logger.LogLevels.Error);
                }
            }
            this.Focus();
        }

        public string GetAppPath()
        {
            return Application.ExecutablePath;
        }

        private void button1_Click_2(object sender, EventArgs e)
        {

        }
    }
}
