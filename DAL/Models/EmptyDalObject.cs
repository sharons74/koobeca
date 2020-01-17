namespace KoobecaFeedController.DAL.Models {
    public class EmptyDalObject : IActivityObject {
        public string GetObjectDesctiption()
        {
            return "";
        }

        string IActivityObject.GetObjectTitle() {
            return "";
        }

        uint IActivityObject.GetStorageFileId() {
            return 0;
        }
    }
}