namespace ConsoleApp
{
    public class EditCommentDialog:CreateCommentDialog
    {
        public EditCommentDialog()
        {
            this.Title = "Edit comment";
        }

        public void SetComment(Comment comment)
        {
            this.commentTextInput.Text = comment.commentText;
        }
    }
}