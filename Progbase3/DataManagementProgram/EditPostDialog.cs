using Terminal.Gui;


public class EditPostDialog : CreatePostDialog
{

    private CommentRepository commentRepository;
    private Post currentPost;
    public Comment selectedComment;

    public EditPostDialog()
    {
        this.Title = "Edit post";
        Button chooseCommentBtn = new Button(2, 7, "Pin comment");
        this.Add(chooseCommentBtn);
        chooseCommentBtn.Clicked += OnChooseComment;

    }

    public void SetData(Post post, CommentRepository commentRepository)
    {
        this.currentPost = post;
        this.commentRepository = commentRepository;
        this.publicationTextInput.Text = post.publicationText;
    }

    private void OnChooseComment()
    {
        PinCommentDialog dialog = new PinCommentDialog();
        dialog.SetData(commentRepository, currentPost.id);
        Application.Run(dialog);
        if (dialog.selectedComment != null)
        {
            currentPost.pinCommentId = dialog.selectedComment.id;
            selectedComment = dialog.selectedComment;
        }
        else
        {
            currentPost.pinCommentId = default;
        }


    }
}
