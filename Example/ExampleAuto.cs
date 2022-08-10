using ESNLib.Tools;
using ESNLib.Tools.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Example
{
    public partial class ExampleAuto : Form, AdminTools.IAdminForm
    {
        public ExampleAuto()
        {
            InitializeComponent();
            Logger.Instance.Write(ApiClient.Models.ApiClientSettings.CreateFromConfigFile().ToString());
            GetAccess();
        }

        public string GetAppPath()
        {
            return Application.ExecutablePath;
        }

        private void SetLabel(bool state)
        {
            if (state)
            {
                var setting = ApiClient.Models.ApiClientSettings.CreateFromConfigFile();
                lblToken.Text = string.Format("Expiring {0}", setting.ExpirationDateTime);
                lblToken.BackColor = Color.LightGreen;

                Logger.Instance.Write(setting.ToString());
            }
            else
            {
                lblToken.Text = "Expired";
                lblToken.BackColor = SystemColors.Control;
            }
            this.Focus();
        }

        private async void GetAccess()
        {
            var client = new ApiClientWrapper.ApiClientWrapper();
            if (!await client.RefreshAccessToken())
            {
                // Refresh not working
                if (!await client.GetAccessToken())
                {
                    if (!MiscTools.HasAdminPrivileges())
                        AdminTools.RunAsAdmin(this);
                    else
                    {
                        SetLabel(false);
                        var result = ESNLib.Controls.Dialog.ShowDialog(new ESNLib.Controls.Dialog.DialogConfig("Unable to get access token... Check the config and the logs", "Error")
                        {
                            Button1 = ESNLib.Controls.Dialog.ButtonType.Ignore,
                            Button2 = ESNLib.Controls.Dialog.ButtonType.Custom1,
                            CustomButton1Text = "Open log",
                            Icon = ESNLib.Controls.Dialog.DialogIcon.Error,
                        });
                        if (result.DialogResult == ESNLib.Controls.Dialog.DialogResult.Custom1)
                            Process.Start(Logger.Instance.FileOutputPath);
                    }
                }
                else
                    SetLabel(true);
            }
            else
                SetLabel(true);
        }
    }
}
