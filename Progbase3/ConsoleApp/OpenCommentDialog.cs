using Terminal.Gui;


public class OpenCommentDialog : Dialog
{
    public bool deleted;
    public bool updated;
    private Comment comment;
    private TextView commentTextOutput;
    private Label commentedAtOutput;
    private User currentUser;


    public Comment GetComment()
    {
        return comment;
    }

    public OpenCommentDialog(User currentUser, Comment comment)
    {
        this.currentUser = currentUser;
        this.comment = comment;
        this.Title = "Open comment";
        int rightColumn = 25;
        int coordinateX = 2;
        Button backBtn = new Button("Back");

        backBtn.Clicked += OnOpenDialogBack;


        if ((currentUser.userName == "ADMIN" && currentUser.passwordHash == "b756562aeca5d42be0705b993c861a473b1c2dbcb782fa730b89d38fd94572ac") || currentUser.id == comment.userId)
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

        Label commentTextLbl = new Label(coordinateX, 2, "Comment text:");
        commentTextOutput = new TextView()
        {
            X = rightColumn,
            Y = Pos.Top(commentTextLbl),
            Width = 40,
            Height = 8,
            ReadOnly = true,
        };
        this.Add(commentTextLbl, commentTextOutput);



        Label commentedAtLbl = new Label(coordinateX, 11, "Commented at:");
        commentedAtOutput = new Label()
        {
            X = rightColumn,
            Y = Pos.Top(commentedAtLbl),
        };
        this.Add(commentedAtLbl, commentedAtOutput);

    }

    public void SetData(Comment comment)
    {
        this.comment = comment;
        this.commentTextOutput.Text = comment.commentText;
        this.commentedAtOutput.Text = comment.commentedAt.ToString();
    }

    private void OnOpenDialogBack()
    {
        Application.RequestStop();
    }

    private void OnOpenDialogEdit()
    {
        EditCommentDialog dialog = new EditCommentDialog();
        dialog.SetComment(this.comment);
        Application.Run(dialog);
        if (dialog.canceled == false)
        {
            Comment updatedComment = dialog.GetCommentFromFields();
            if (updatedComment == null)
            {
                this.updated = false;
            }
            else
            {
                updatedComment.commentedAt = comment.commentedAt;
                updatedComment.userId = comment.userId;
                updatedComment.postId = comment.postId;
                this.SetData(updatedComment);
                this.updated = true;
            }
        }
    }

    private void OnOpenDialogDelete()
    {
        int index = MessageBox.Query("Delete comment", "Are you sure?", "No", "Yes");
        if (index == 1)
        {
            Application.RequestStop();
            deleted = true;
        }
    }


}
