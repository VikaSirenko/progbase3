using Terminal.Gui;

namespace ConsoleApp
{
    public class OpenUserDialog : Dialog
    {
        public bool deleted;
        public bool updated;
        private User user;
        private Label userNameOutput;
        // private TextField passwordOutput;
        private Label fullNameOutput;
        private Label isModeratorOutput;

        public User GetTask()
        {
            return user;
        }

        public OpenUserDialog()
        {
            this.Title = "Open user";
            int rightColumn = 25;
            int coordinateX = 2;
            Button backBtn = new Button("Back");
            Button editBtn = new Button("Edit");
            Button deleteBtn = new Button("Delete");
            backBtn.Clicked += OnOpenDialogBack;
            editBtn.Clicked += OnOpenDialogEdit;
            deleteBtn.Clicked += OnOpenDialogDelete;
            this.AddButton(editBtn);
            this.AddButton(deleteBtn);
            this.AddButton(backBtn);

            Label userNameLbl = new Label(coordinateX, 2, "User name:");
            userNameOutput = new Label()
            {
                X = rightColumn,
                Y = Pos.Top(userNameLbl),
            };
            this.Add(userNameLbl, userNameOutput);

            //???????????????????????????
            /*
                        Label passwordLbl = new Label(coordinateX, 4, "Possword:");
                        passwordOutput = new TextField()
                        {
                            X = rightColumn,
                            Y = Pos.Top(passwordLbl),
                            Width = 40,
                            ReadOnly = true,
                            Secret = true,

                        };



                        this.Add(passwordLbl, passwordOutput);

                        */

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

        public void SetUser(User user)
        {
            this.user = user;
            this.userNameOutput.Text = user.userName;
            // this.passwordOutput.Text=user.passwordHash;
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
                    this.SetUser(updatedUser);
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
}
