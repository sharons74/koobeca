namespace KoobecaSync.APIs.FB
{
    public class FBVideo
    {
        public string source;
        public string title;
        public string description;
        public string picture;
        public Source from;

        public class Source
        {
            public string name;
        }
    }
}