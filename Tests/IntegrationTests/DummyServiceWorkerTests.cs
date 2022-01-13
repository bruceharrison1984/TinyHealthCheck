using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests.IntegrationTests
{
    /// <summary>
    /// The tests here run against the DummyServiceWorker to ensure that the contract put forth by TinyHealthCheck hasn't been broken
    /// </summary>
    [TestFixture]
    public class DummyServiceWorkerTests
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            var hostProcess = DummyServiceWorker.Program.CreateHostBuilder(new string[] { "localhost" });
            await hostProcess.Build().StartAsync();
        }

        [Test]
        public async Task BasicHealthCheck_ReturnsSuccess()
        {
            var result = await QueryHealthCheckEndpoint<Dictionary<string, string>>("http://localhost:8080/healthz");

            Assert.IsTrue(result.response.IsSuccessStatusCode);
            Assert.AreEqual(result.json["Status"], "Healthy!");
        }

        [Test]
        public async Task BasicHealthCheckWithUptime_ReturnsSuccess()
        {
            var result = await QueryHealthCheckEndpoint<Dictionary<string, string>>("http://localhost:8081/healthz");

            Assert.IsTrue(result.response.IsSuccessStatusCode);
            Assert.AreEqual(result.json["Status"], "Healthy!");
            Assert.DoesNotThrow(() => TimeSpan.Parse(result.json["Uptime"]));
        }

        /// <summary>
        /// This test runs two requests to make sure that the waiting/iterating is appropriately changing the health-check output
        /// </summary>
        [Test]
        public async Task CustomHealthCheck_RespondsAsExpected()
        {
            var result = await QueryHealthCheckEndpoint<CustomHealthCheckResponse>("http://localhost:8082/healthz");

            Assert.IsTrue(result.response.IsSuccessStatusCode);
            Assert.That(result.json.Status, Is.EqualTo("Healthy!"));
            Assert.That(result.json.IsServiceRunning, Is.EqualTo(true));
            Assert.That(result.json.Iteration, Is.EqualTo(0));

            //run on another thread so sleeping doesn't also sleep the DummyHealthCheck process
            await Task.Run(async () =>
            {
                Thread.Sleep(new TimeSpan(0,0,26));
                var secondResult = await QueryHealthCheckEndpoint<CustomHealthCheckResponse>("http://localhost:8082/healthz");

                Assert.IsFalse(secondResult.response.IsSuccessStatusCode);
                Assert.AreEqual(secondResult.json.Status, "Unhealthy!");
                Assert.AreEqual(secondResult.json.Iteration, 10);
                Assert.IsFalse(secondResult.json.IsServiceRunning);
            });
        }

        private async Task<(T json, HttpResponseMessage response)> QueryHealthCheckEndpoint<T>(string url) where T : class
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(new Uri(url));
            var responseObject = System.Text.Json.JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
            if (responseObject == null) throw new Exception("Response was null. Make sure the target project is running before executing tests");
            return (responseObject, response);
        }

        private class CustomHealthCheckResponse
        {
            public string? Status { get; set; }
            public int Iteration { get; set; }
            public bool IsServiceRunning { get; set; }
        }
    }
}