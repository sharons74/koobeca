using System;
using System.IO;

namespace KoobecaSocketServer {
    internal class RequestEventArgs : EventArgs {
        public RequestEventArgs(string request, StreamWriter stream) {
            Request = request;
            Stream = stream;
        }

        public string Request { get; set; }
        public StreamWriter Stream { get; set; }
    }
}