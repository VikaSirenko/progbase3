using Terminal.Gui;

public class EditUserDialog : Dialog
{
    public bool canceled;
    protected TextField userNameInput;
    protected TextField passwordInput;
    protected TextField fullNameInput;
    protected CheckBox isModeratorCheck;
    private Label isModeratorLbl;
    private User selectedUser;

    public EditUserDialog()
    {

        this.Title = "Edit user";
        Button okBtn = new Button("Ok");
        Button cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += OnEditDialogCanceled;
        okBtn.Clicked += OnEditButtonSubmit;
        this.AddButton(cancelBtn);
        this.AddButton(okBtn);
        int rightColumn = 25;
        int coordinateX = 2;

        Label userNameLbl = new Label(coordinateX, 2, "User name:");
        userNameInput = new TextField("")
        {
            X = rightColumn,
            Y = Pos.Top(userNameLbl),
            Width = 50,

        };
        this.Add(userNameLbl, userNameInput);

        Label passwordLbl = new Label(coordinateX, 4, "Password:");
        passwordInput = new TextField()
        {
            X = rightColumn,
            Y = Pos.Top(passwordLbl),
            Width = 50,
            Secret = true,
        };

        this.Add(passwordLbl, passwordInput);

        Label fullNameLbl = new Label(coordinateX, 6, "Full Name:");
        fullNameInput = new TextField()
        {
            X = rightColumn,
            Y = Pos.Top(fullNameLbl),
            Width = 50,
        };
        this.Add(fullNameLbl, fullNameInput);

        isModeratorLbl = new Label(2, 8, "Is moderator:");
        isModeratorCheck = new CheckBox()
        {
            X = 25,
            Y = Pos.Top(isModeratorLbl),
            Width = 40,

        };
        this.Add(isModeratorLbl, isModeratorCheck);


    }

    private void OnEditDialogCanceled()
    {
        this.canceled = true;
        Application.RequestStop();
    }

    private void OnEditButtonSubmit()
    {
        this.canceled = false;
        Application.RequestStop();
    }

    public User GetUserFromFields()
    {
        string[] fullName = this.fullNameInput.Text.ToString().Split(" ");
        if (!userNameInput.Text.IsEmpty && !fullNameInput.Text.IsEmpty && fullName.Length == 2)
        {
            int moderatorNum = default;
            if (isModeratorCheck.Checked == true)
                moderatorNum = 1;
            else
                moderatorNum = 0;

            if (passwordInput.Text.IsEmpty)
            {
                User user = new User();
                user.userName = userNameInput.Text.ToString();
                user.passwordHash = selectedUser.passwordHash;
                user.fullname = fullNameInput.Text.ToString();
                user.isModerator = isModeratorCheck.Checked;
                return user;
            }
            else
            {
                User user = new User(userNameInput.Text.ToString(), passwordInput.Text.ToString(), fullNameInput.Text.ToString(), moderatorNum);
                return user;
            }

        }

        return null;
    }

    public void SetUser(User selectedUser)
    {
        this.selectedUser = selectedUser;
        this.userNameInput.Text = selectedUser.userName;
        this.passwordInput.Text = "";
        this.fullNameInput.Text = selectedUser.fullname;
        this.isModeratorCheck.Checked = selectedUser.isModerator;

    }

}
