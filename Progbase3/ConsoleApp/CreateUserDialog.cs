using Terminal.Gui;

namespace ConsoleApp
{
    public class CreateUserDialog : Dialog
    {
        public bool canceled;
        protected TextField userNameInput;
        protected TextField passwordInput;
        protected TextField fullNameInput;
        protected CheckBox isModeratorCheck;

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

            Label fullNameLbl = new Label(coordinateX, 11, "Full Name:");
            fullNameInput = new TextField()
            {
                X = rightColumn,
                Y = Pos.Top(fullNameLbl),
                Width = 40,
            };
            this.Add(fullNameLbl, fullNameInput);

            Label isModeratorLbl = new Label(coordinateX, 13, "Is moderator:");
            isModeratorCheck = new CheckBox()
            {
                X = rightColumn,
                Y = Pos.Top(isModeratorLbl),
                Width = 40,

            };
            this.Add(isModeratorLbl, isModeratorCheck);

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
            User user = new User();
            string[] fullName = this.fullNameInput.Text.ToString().Split("");
            if (!userNameInput.Text.IsEmpty && !passwordInput.Text.IsEmpty && !fullNameInput.Text.IsEmpty && fullName.Length == 2) //??
            {
                user.userName = this.userNameInput.Text.ToString();
                user.passwordHash = this.passwordInput.Text.ToString();
                user.fullname = this.fullNameInput.Text.ToString();
                user.isModerator = this.isModeratorCheck.Checked;
                return user;
            }

            return null;
        }
    }
}