using ApiClient;
using ApiClient.Models;
using ESNLib.Tools;
using ESNLib.Tools.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Example
{
    public partial class ExampleAuto : Form, AdminTools.IAdminForm
    {
        private ApiClientService service = new ApiClientService(ApiClientSettings.GetInstance());

        public ExampleAuto()
        {
            InitializeComponent();
            Logger.Instance.Write(ApiClientSettings.GetInstance().ToString());
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
                var setting = ApiClientSettings.GetInstance();
                lblToken.Text = "Available";
                lblToken.BackColor = Color.LightGreen;

                Logger.Instance.Write(setting.ToString());
            }
            else
            {
                lblToken.Text = "Expired";
                lblToken.BackColor = Color.Crimson;
            }
            this.Focus();
        }

        private async void GetAccess()
        {
            var setting = ApiClientSettings.GetInstance();
            if (setting.ExpirationDateTime < DateTime.Now && setting.RefreshToken != null)
            {
                var client = new ApiClientWrapper();
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

                service = new ApiClientService(ApiClientSettings.GetInstance());
            }
            else
                SetLabel(true);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ProductDetails();
        }

        private async void ProductDetails()
        {
            //var response = await service.KeywordSearch("P5555-ND");
            
            // MPN, SPN, description, category, pricing. Maybe add obsolete, productStatus, PrimaryPhoto, QuantityAvailable, MinimumOrderQuantity
            // Category seems to give nothing ?
            var response = await service.ProductDetails(textBox1.Text, "ManufacturerPartNumber,DigiKeyPartNumber,DetailedDescription,StandardPricing");

            // Simulate search result :
            //string response = File.ReadAllText(@"C:\Workspace\01_Programming\DigikeyAPI\Example\bin\Debug\simulate.json");

            richTextBox1.Text = response;
            // Compress
            var compressed = Zip(response);
            File.WriteAllBytes("compressed.json", compressed);
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        private void lblToken_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(Logger.Instance.FileOutputPath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = Unzip(File.ReadAllBytes("compressed.json"));
        }
    }
}
