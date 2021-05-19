namespace ConsoleApp
{
    public class EditPostDialog:CreatePostDialog
    {
         public EditPostDialog()
        {
            this.Title = "Edit post";
        }

        public void SetPost(Post post)
        {
            this.publicationTextInput.Text=post.publicationText;
        }
    }
}