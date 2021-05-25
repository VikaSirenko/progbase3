using Terminal.Gui;

public class OpenPostDialog : Dialog
{
    public bool deleted;
    public bool updated;
    private Post post;
    private TextView publicationTextOutput;
    private Label publishedAtOutput;
    private TextField pinnedComment;
    private CommentRepository commentRepository;
    private Comment comment;
    private User currentUser;


    public Post GetPost()
    {
        return post;
    }

    public OpenPostDialog(User currentUser, Post post)
    {
        this.currentUser = currentUser;
        this.post = post;
        this.Title = "Open post";
        int rightColumn = 25;
        int coordinateX = 2;
        Button backBtn = new Button("Back");
        backBtn.Clicked += OnOpenDialogBack;

        if ((currentUser.userName == "ADMIN" && currentUser.passwordHash == "b756562aeca5d42be0705b993c861a473b1c2dbcb782fa730b89d38fd94572ac") || currentUser.id == post.userId)
        {
            Button editBtn = new Button("Edit");
            Button deleteBtn = new Button("Delete");
            editBtn.Clicked += OnOpenDialogEdit;
            deleteBtn.Clicked += OnOpenDialogDelete;
            this.AddButton(editBtn);
            this.AddButton(deleteBtn);
        }
        else if (currentUser.isModerator == true)
        {
            Button deleteBtn = new Button("Delete");
            deleteBtn.Clicked += OnOpenDialogDelete;
            this.AddButton(deleteBtn);
        }


        this.AddButton(backBtn);

        Label publicationTextLbl = new Label(coordinateX, 2, "Publicatiion text:");
        publicationTextOutput = new TextView()
        {
            X = rightColumn,
            Y = Pos.Top(publicationTextLbl),
            Width = 40,
            Height = 8,
            ReadOnly = true,
        };
        this.Add(publicationTextLbl, publicationTextOutput);



        Label publisherAtLbl = new Label(coordinateX, 10, "Published at:");
        publishedAtOutput = new Label()
        {
            X = rightColumn,
            Y = Pos.Top(publisherAtLbl),
        };
        this.Add(publisherAtLbl, publishedAtOutput);

        Label pinnedComLbl = new Label(coordinateX, 14, "Pin comment");
        pinnedComment = new TextField()
        {
            X = rightColumn,
            Y = Pos.Top(pinnedComLbl),
            Width = 40,
            Height = 8,
            ReadOnly = true,
        };
        this.Add(pinnedComLbl, pinnedComment);

        Button showCommentsBtn = new Button(coordinateX, 18, "Show comments");
        showCommentsBtn.Clicked += OnShowCommentsClicked;
        this.Add(showCommentsBtn);

    }

    private void OnShowCommentsClicked()
    {
        DialogOfSelectedPostComments dialog = new DialogOfSelectedPostComments();
        dialog.SetData(commentRepository, post.id, currentUser);
        Application.Run(dialog);
    }

    public void SetData(Post post, CommentRepository commentRepository)
    {
        this.post = post;
        this.commentRepository = commentRepository;
        this.publicationTextOutput.Text = post.publicationText;
        this.publishedAtOutput.Text = post.publishedAt.ToString();
        if (post.pinCommentId != default)
        {
            this.comment = commentRepository.GetByCommentId(post.pinCommentId);
            this.pinnedComment.Text = this.comment.commentText;
        }
        else
        {
            pinnedComment.Text = "None";
        }
    }

    private void OnOpenDialogBack()
    {
        Application.RequestStop();
    }

    private void OnOpenDialogEdit()
    {
        EditPostDialog dialog = new EditPostDialog();
        dialog.SetData(this.post, commentRepository);
        Application.Run(dialog);
        if (dialog.canceled == false)
        {
            Post updatedPost = dialog.GetPostFromFields();
            if (dialog.selectedComment != null)
            {
                updatedPost.pinCommentId = dialog.selectedComment.id;
                this.comment = dialog.selectedComment;
            }

            if (updatedPost == null)
            {
                this.updated = false;
            }
            else
            {
                updatedPost.publishedAt = post.publishedAt;
                updatedPost.userId = post.userId;
                updatedPost.id = post.id;
                this.SetData(updatedPost, commentRepository);
                this.updated = true;
            }
        }
    }

    private void OnOpenDialogDelete()
    {
        int index = MessageBox.Query("Delete post", "Are you sure?", "No", "Yes");
        if (index == 1)
        {
            Application.RequestStop();
            deleted = true;
        }
    }

}
