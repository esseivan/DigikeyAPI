using Microsoft.VisualStudio.TestTools.UnitTesting;
using ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiClient.Models;

namespace ApiClient.Tests
{
    [TestClass()]
    public class ApiClientWrapperTests
    {
        [TestMethod()]
        public void RefreshAccessTokenTest_Valid()
        {
            // Clear settings
            (ApiClientSettings.GetInstance()).ClearAndSave();

            ApiClientWrapper client = new ApiClientWrapper();
            bool result = client.RefreshAccessToken().Wait(60000);

            Assert.IsTrue(result);
        }
    }
}
