using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Example
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable logger
            var log = ESNLib.Tools.Logger.Instance;
            log.FilePath = ESNLib.Tools.Logger.GetDefaultLogPath("ESN", "ApiClientWrapper", "log.txt");
            log.Enable();

            // Sandbox mode API
            ApiClient.Models.ApiClientSettings.SetSandboxMode();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ExampleAuto());
        }
    }
}
