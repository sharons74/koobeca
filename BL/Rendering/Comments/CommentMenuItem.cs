using KoobecaFeedController.DAL.Models;

namespace KoobecaFeedController.BL.Rendering
{
    public class CommentMenuItem
    {
        public string name;
        public string label;
        public string url;
        public CommentMenuItemUrlParams urlParams;
        public uint? isLike;
        public uint? isReply;

        public CommentMenuItem()
        {

        }

        public CommentMenuItem(string itemName,Comment comment)
        {
            urlParams = new CommentMenuItemUrlParams(comment.comment_id, comment.resource_id);

            if (itemName == "delete"){
                name = "delete";
                label = "Delete";
                url = "comment-delete";
            }
            if (itemName == "comment_delete")
            {
                name = "comment_delete";
                label = "Delete";
                url = "comment-delete";
            }
            else if (itemName == "like")
            {
                name = url = itemName;
                label = "Like";
                isLike = 0;
            }
            else if (itemName == "unlike")
            {
                name = url = itemName;
                label = "Unlike";
                isLike = 1;
            }
            else if (itemName == "reply")
            {
                name = itemName;
                label = "Reply";
                url = "advancedcomments/reply";
                isReply = 0;
            }
            else if (itemName == "comment_edit")
            {
                name = itemName;
                label = "Edit";
                url = "advancedcomments/comment-edit";
            }
            else if (itemName == "comment_copy")
            {
                name = itemName;
                label = "Copy";
                urlParams = null;
            }
            else if (itemName == "comment_cancel")
            {
                name = itemName;
                label = "Cancel";
                urlParams = null;
            }
            
        }
    }
}