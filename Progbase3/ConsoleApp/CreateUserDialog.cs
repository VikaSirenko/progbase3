using Terminal.Gui;


public class CreateUserDialog : Dialog
{
    public bool canceled;
    protected TextField userNameInput;
    protected TextField passwordInput;
    protected TextField fullNameInput;
    protected CheckBox isModeratorCheck;
    private bool isRegistration;
    private Label isModeratorLbl;

    public CreateUserDialog()
    {

        this.Title = "Create user";
        Button okBtn = new Button("Ok");
        Button cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += OnCreateDialogCanceled;
        okBtn.Clicked += OnCreateButtonSubmit;
        this.AddButton(cancelBtn);
        this.AddButton(okBtn);
        int rightColumn = 25;
        int coordinateX = 2;

        Label userNameLbl = new Label(coordinateX, 2, "User name:");
        userNameInput = new TextField("")
        {
            X = rightColumn,
            Y = Pos.Top(userNameLbl),
            Width = 40,

        };
        this.Add(userNameLbl, userNameInput);

        Label passwordLbl = new Label(coordinateX, 4, "Password:");
        passwordInput = new TextField()
        {
            X = rightColumn,
            Y = Pos.Top(passwordLbl),
            Width = 40,
            Secret = true,
        };

        this.Add(passwordLbl, passwordInput);

        Label fullNameLbl = new Label(coordinateX, 6, "Full Name:");
        fullNameInput = new TextField()
        {
            X = rightColumn,
            Y = Pos.Top(fullNameLbl),
            Width = 40,
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

    public void SetRegistration(bool isRegistration)
    {
        this.isRegistration = isRegistration;
        if (isRegistration == true)
        {
            isModeratorCheck.Visible = false;
            isModeratorLbl.Visible = false;
        }
    }

    private void OnCreateDialogCanceled()
    {
        this.canceled = true;
        Application.RequestStop();
    }

    private void OnCreateButtonSubmit()
    {
        this.canceled = false;
        Application.RequestStop();
    }

    public User GetUserFromFields()
    {
        string[] fullName = this.fullNameInput.Text.ToString().Split(" ");
        if (!userNameInput.Text.IsEmpty && !passwordInput.Text.IsEmpty && !fullNameInput.Text.IsEmpty && fullName.Length == 2)
        {
            int moderatorNum = default;
            if (isRegistration == false)
            {
                if (isModeratorCheck.Checked == true)
                    moderatorNum = 1;
                else
                    moderatorNum = 0;
            }
            else
            {
                moderatorNum = 0;
            }

            User user = new User(userNameInput.Text.ToString(), passwordInput.Text.ToString(), fullNameInput.Text.ToString(), moderatorNum);
            return user;

        }

        return null;
    }
}
