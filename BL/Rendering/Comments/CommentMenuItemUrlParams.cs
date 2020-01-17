namespace KoobecaFeedController.BL.Rendering
{
    public class CommentMenuItemUrlParams
    {
        public ulong action_id;
        public string subject_type;
        public uint subject_id;
        public ulong comment_id;
        

        public CommentMenuItemUrlParams(uint comment_id, uint action_id)
        {
            this.comment_id = comment_id;
            this.action_id = this.subject_id = action_id;
            //this.action_id = action_id.ToString();
            this.subject_type = "activity_action";
        }
    }
}