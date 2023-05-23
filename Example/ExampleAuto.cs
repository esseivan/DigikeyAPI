using ApiClient;
using ApiClient.Models;
using DigikeyApiWrapper;
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
            Logger.Instance.Write("Application idle...");

#if DEBUG
            Process.Start(Logger.Instance.FileOutputPath);
            //Logger.Instance.Write(ApiClientSettings.GetInstance().ToString());
#endif

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
            var client = new ApiClientWrapper();
            var result = await client.GetAccess();

            if (result.Success)
            {
                SetLabel(true);
                if (result.NewService != null)
                {
                    service = result.NewService;
                }
            }
            else
            {
                if (!MiscTools.HasAdminPrivileges())
                {
                    AdminTools.RunAsAdmin(this);
                }
                else
                {
                    SetLabel(false);
                    var res = ESNLib.Controls.Dialog.ShowDialog(new ESNLib.Controls.Dialog.DialogConfig("Unable to get access token... Check the config and the logs", "Error")
                    {
                        Button1 = ESNLib.Controls.Dialog.ButtonType.Ignore,
                        Button2 = ESNLib.Controls.Dialog.ButtonType.Custom1,
                        CustomButton1Text = "Open log",
                        Icon = ESNLib.Controls.Dialog.DialogIcon.Error,
                    });
                    if (res.DialogResult == ESNLib.Controls.Dialog.DialogResult.Custom1)
                    {
                        Process.Start(Logger.Instance.FileOutputPath);
                    }
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ProductDetails();
        }

        private async void ProductDetails()
        {
            PartSearch ps = new PartSearch();
            string response = await ps.ProductDetails_Essentials(textBox1.Text);

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
            richTextBox1.Text = @"{""StandardPricing"":[{""BreakQuantity"":1,""UnitPrice"":0.86,""TotalPrice"":0.86}],""DetailedDescription"":""1000µF 35V Aluminum Electrolytic Capacitors Radial, Can  2000 Hrs @ 105°C"",""ManufacturerPartNumber"":""ECA-1VHG102"",""DigiKeyPartNumber"":""P5555-ND"",""Manufacturer"":{""ParameterId"":-1,""ValueId"":""10"",""Parameter"":""Manufacturer"",""Value"":""Panasonic Electronic Components""}}";


            DigikeyPart myPart = PartSearch.DeserializeProductDetails(richTextBox1.Text);
        }

    }
}
