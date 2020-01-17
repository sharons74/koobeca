using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.BL.Rendering
{
    public class FeedReaction
    {
        public FeedReaction(Reaction reaction,uint count)
        {
            caption = reaction.title;
            if(count > 0) reaction_count = count;
            reactionicon_id = reaction.reactionicon_id;
            reaction_image_icon = ServiceUrlPrefixes.CouldFront + Storages.GetByFileId(reaction.photo_id).storage_path;
        }

        public string caption;
        public uint? reaction_count;
        public string reaction_image_icon;
        public uint reactionicon_id; 
    }
}
