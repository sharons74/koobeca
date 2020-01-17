namespace KoobecaFeedController.DAL.Models
{
    public interface IFollowedSource
    {
        void IncreasFollow();
        void DecreaseFollow();
    }
}