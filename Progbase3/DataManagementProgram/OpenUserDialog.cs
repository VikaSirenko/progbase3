using Terminal.Gui;


public class OpenUserDialog : Dialog
{
    public bool deleted;
    public bool updated;
    private User user;
    private Label userNameOutput;
    private Label fullNameOutput;
    private Label isModeratorOutput;
    private User currentUser;
    private PostRepository postRepository;
    private CommentRepository commentRepository;


    public User GetUser()
    {
        return user;
    }

    public OpenUserDialog(User currentUser, User user)
    {
        this.currentUser = currentUser;
        this.user = user;
        this.Title = "Open user";
        int rightColumn = 25;
        int coordinateX = 2;
        Button backBtn = new Button("Back");
        backBtn.Clicked += OnOpenDialogBack;

        if ((currentUser.userName == "ADMIN" && currentUser.passwordHash == "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824") || currentUser.id == user.id)
        {
            Button editBtn = new Button("Edit");
            Button deleteBtn = new Button("Delete");

            editBtn.Clicked += OnOpenDialogEdit;
            deleteBtn.Clicked += OnOpenDialogDelete;
            this.AddButton(editBtn);
            this.AddButton(deleteBtn);
        }

        Button showUserPosts = new Button(coordinateX, 10, "View user posts");
        showUserPosts.Clicked += OnShowUserPosts;
        this.Add(showUserPosts);

        this.AddButton(backBtn);

        Label userNameLbl = new Label(coordinateX, 4, "User name:");
        userNameOutput = new Label()
        {
            X = rightColumn,
            Y = Pos.Top(userNameLbl),
        };
        this.Add(userNameLbl, userNameOutput);

        Label fullNameLbl = new Label(coordinateX, 6, "Full name:");
        fullNameOutput = new Label()
        {
            X = rightColumn,
            Y = Pos.Top(fullNameLbl),
        };
        this.Add(fullNameLbl, fullNameOutput);

        Label isModeratorLbl = new Label(coordinateX, 8, "Is moderator:");
        isModeratorOutput = new Label()
        {
            X = rightColumn,
            Y = Pos.Top(isModeratorLbl),

        };
        this.Add(isModeratorLbl, isModeratorOutput);

    }

    private void OnShowUserPosts()
    {
        ShowUserPostsDialog dialog = new ShowUserPostsDialog();
        dialog.SetData(postRepository, commentRepository, user);
        Application.Run(dialog);
    }

    public void SetData(User user, PostRepository postRepository, CommentRepository commentRepository)
    {
        this.user = user;
        this.userNameOutput.Text = user.userName;
        this.fullNameOutput.Text = user.fullname;
        this.isModeratorOutput.Text = user.isModerator.ToString();
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    private void OnOpenDialogBack()
    {
        Application.RequestStop();
    }

    private void OnOpenDialogEdit()
    {
        EditUserDialog dialog = new EditUserDialog();
        dialog.SetUser(this.user);
        Application.Run(dialog);
        if (dialog.canceled == false)
        {
            User updatedUser = dialog.GetUserFromFields();
            if (updatedUser == null)
            {
                this.updated = false;
                MessageBox.ErrorQuery("Edit user", "Can not edit user. Fields are filled incorrectly.", "OK");
            }
            else
            {
                this.SetData(updatedUser, postRepository, commentRepository);
                this.updated = true;
            }
        }
    }

    private void OnOpenDialogDelete()
    {
        int index = MessageBox.Query("Delete user", "Are you sure?", "No", "Yes");
        if (index == 1)
        {
            Application.RequestStop();
            deleted = true;
        }
    }

}

