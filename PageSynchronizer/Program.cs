using FBSynch;
using KoobecaFeedController.DAL.Adapters;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace PageSynchronizer
{
    class Program
    {
        private static bool _Working = true;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            // do some work

            string fbToken = "EAAAAAYsX7TsBALOpEIIMWSDbIAfs9NkxRdhLDeRnS8EE8oUmQIn3CMCync754u11wlOmV7vsZAYoDfCwEHMbRnaZB3P1f7N9YDu6g5qtpLjXxf0RaLMmYnRe0VEVGVQ9LstqBVUByFCNGLZChaNW0MFyB0a50ZAG25rvucj6Ug852hesZBQWRLpSRwr7HMfVsCpIodZCUwGZAscv8UXgJHB";
            string koobuser = "pageadmin@koobeca.com";
            string koobpswd = "123456";
            var controller = new Controller();
            controller.Start(koobuser,koobpswd, fbToken);

            while (_Working)
            {
                Thread.Sleep(100);
            }
            controller.StopAll();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("Exiting...");
            _Working = false;
        }
    }
}
