using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace KoobecaSync.APIs.Koobeca.Requests {
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public abstract class KoobecaApiRequest {
        public abstract string Endpoint { get; }
        public string body { get; set; }

        public string auth_view { get; set; } = "everyone";

        public Stream[] photos { get; set; }
    }
}
