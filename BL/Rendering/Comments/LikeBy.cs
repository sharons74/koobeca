using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using System;

namespace KoobecaFeedController.BL.Rendering.Comments
{
    public class LikeBy
    {
  
        public ulong user_id;
        public string displayname;
        public string image_profile;
        public string friendship_type;
        public string reaction_image_icon;
        public int isVerified = 1;
        public string image;
        public string image_normal;
        public string  image_icon;

        public LikeBy(Like like)
        {
            user_id = like.poster_id;
            var user = Users.GetById((uint)user_id);
            displayname = user.displayname;
            image_profile = ServiceUrlPrefixes.CouldFront + Storages.GetByFileId((uint)user.photo_id).storage_path;
            reaction_image_icon = like.reaction;
            image = image_normal = image_icon = image_profile;
         }
    }
}