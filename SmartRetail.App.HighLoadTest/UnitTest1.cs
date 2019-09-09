using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartRetail.App.HighLoadTest
{
    /// <summary>
    /// Сводное описание для UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async void TestMethod1()
        {
            var httpClient = new HttpClient();
            var t = await httpClient.GetAsync(@"http://localhost:5000/units");
        }
    }
}
