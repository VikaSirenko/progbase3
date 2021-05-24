namespace ConsoleApp
{
    public class EditUserDialog :CreateUserDialog
    {
        public EditUserDialog()
        {
            this.Title = "Edit user";
        }

        public void SetUser(User user)
        {
            this.userNameInput.Text=user.userName;
            this.passwordInput.Text="";
            this.fullNameInput.Text=user.fullname;
            this.isModeratorCheck.Checked=user.isModerator;

        }
        
    }
}