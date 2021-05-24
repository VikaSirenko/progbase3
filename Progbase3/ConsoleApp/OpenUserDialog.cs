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

        if ((currentUser.userName == "ADMIN" && currentUser.passwordHash == "b756562aeca5d42be0705b993c861a473b1c2dbcb782fa730b89d38fd94572ac") || currentUser.id == user.id)
        {
            Button editBtn = new Button("Edit");
            Button deleteBtn = new Button("Delete");

            editBtn.Clicked += OnOpenDialogEdit;
            deleteBtn.Clicked += OnOpenDialogDelete;
            this.AddButton(editBtn);
            this.AddButton(deleteBtn);
        }

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

    public void SetData(User user)
    {
        this.user = user;
        this.userNameOutput.Text = user.userName;
        this.fullNameOutput.Text = user.fullname;
        this.isModeratorOutput.Text = user.isModerator.ToString();
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
            }
            else
            {
                this.SetData(updatedUser);
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

