using ApiClient.Models;
using ApiClient;
using ESNLib.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESNLib.Tools.WinForms;

namespace DigikeyApiWrapper
{
    public class Access
    {
        private ApiClientService service;

        public event EventHandler<StateChangedEventArgs> OnStateChanged;

        private async void GetAccess(AdminTools.IAdminForm form)
        {
            var setting = ApiClientSettings.GetInstance();
            bool IsExpired = (setting.ExpirationDateTime < DateTime.Now)
                && (setting.RefreshToken != null);

            if (!IsExpired)
            {
                OnStateChanged?.Invoke(this, new StateChangedEventArgs(true));
                service = new ApiClientService(ApiClientSettings.GetInstance());
                return;
            }

            var client = new ApiClientWrapper();
            bool GotAccessToken = await client.RefreshAccessToken();

            if (GotAccessToken)
            {
                OnStateChanged?.Invoke(this, new StateChangedEventArgs(true));
                service = new ApiClientService(ApiClientSettings.GetInstance());
                return;
            }

            bool hasAdminPrivileges = MiscTools.HasAdminPrivileges();

            if (!hasAdminPrivileges)
            {
                AdminTools.RunAsAdmin(form);
                throw new ApplicationException("Application should have exited on it's own...");
            }


            OnStateChanged?.Invoke(this, new StateChangedEventArgs(false));
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

        public class StateChangedEventArgs : EventArgs
        {
            public bool State { get; set; }

            public StateChangedEventArgs(bool state)
            {
                State = state;
            }
        }
    }
}
