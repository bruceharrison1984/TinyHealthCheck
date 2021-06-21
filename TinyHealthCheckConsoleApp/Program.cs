using System;

namespace TinyHealthCheckConsoleApp
{
    class Program
    {
        static async void Main(string[] args)
        {
            var h = new TinyHealthCheck.TinyHealthCheck();
            await h.Start();

            while (true)
            {

            }
        }
    }
}
