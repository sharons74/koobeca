namespace KoobecaFeedController.DAL.Models {
    public interface IActivityObject {
        string GetObjectTitle();
        uint GetStorageFileId();
        string GetObjectDesctiption();
    }

    public class DummeyActivityObject : IActivityObject
    {
        public string GetObjectTitle()
        {
            return string.Empty;
        }

        public string GetObjectDesctiption()
        {
            return string.Empty;
        }

        public uint GetStorageFileId()
        {
            return 0;
        }
    }
}