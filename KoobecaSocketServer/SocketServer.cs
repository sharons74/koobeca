using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace KoobecaSocketServer {
    internal class SocketServer : IDisposable {
        private readonly int _port;
        public int ConcurrentConnections { get; set; } = 100;
        public CancellationTokenSource TokenSource { get; set; }
        public TcpListener Socket { get; set; }
        public Task Task { get; set; }

        public event EventHandler<RequestEventArgs> Request;

        public SocketServer(int port) {
            _port = port;
        }

        public Task Run() {
            var server = new TcpListener(IPAddress.Any, _port);
            server.Start(ConcurrentConnections);
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var rc = Task.Run(async () => {
                while (!token.IsCancellationRequested) {
                    Console.WriteLine("Awaiting");
                    using (var client = await server.AcceptTcpClientAsync())
                    using (var strm = client.GetStream())
                    using (var sr = new StreamReader(strm))
                    using (var sw = new StreamWriter(strm)) {
                        var request = sr.ReadToEnd();
                        Console.WriteLine("Responding");
                        OnRequest(request, sw);
                    }

                    Console.WriteLine("Closed");
                }
            }, token);
            Socket = server;
            TokenSource = tokenSource;
            Task = rc;
            return rc;
        }

        public void Stop() {
            TokenSource?.Cancel();
            Socket?.Stop();
            TokenSource = null;
            Socket = null;
        }

        public void Dispose() {
            Stop();
            TokenSource?.Dispose();
        }

        protected virtual void OnRequest(string request, StreamWriter stream) {
            Request?.Invoke(this, new RequestEventArgs(request, stream));
        }
    }
}