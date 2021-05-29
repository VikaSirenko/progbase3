using Terminal.Gui;
public class ShowUserDataDialog : Dialog
{
    private UserRepository userRepository;
    private PostRepository postRepository;
    private CommentRepository commentRepository;
    private User currentUser;

    public bool isUserRemoved;

    public ShowUserDataDialog()
    {
        this.Title = "My information";
        Button backBtn = new Button(25, 18, "Back");
        backBtn.Clicked += OnBackClicked;
        this.Add(backBtn);

        Button showAccountBtn = new Button(4, 4, "Account management");
        showAccountBtn.Clicked += OnAccountManagement;
        this.Add(showAccountBtn);
        Button showMyPostsBtn = new Button(4, 8, "Show my posts");
        showMyPostsBtn.Clicked += OnShowMyPostsClicked;
        this.Add(showMyPostsBtn);

        Button showMyCommentsBtn = new Button(4, 12, "Show my comments");
        showMyCommentsBtn.Clicked += OnShowMyCommentsClicked;
        this.Add(showMyCommentsBtn);
    }

    private void OnBackClicked()
    {
        Application.RequestStop();
    }

    private void OnShowMyCommentsClicked()
    {
        ShowUserCommentsDialog dialog = new ShowUserCommentsDialog();
        dialog.SetData(commentRepository, currentUser);
        Application.Run(dialog);
    }

    private void OnAccountManagement()
    {
        OpenUserDialog dialog = new OpenUserDialog(currentUser, currentUser);
        dialog.SetData(currentUser, postRepository, commentRepository);

        Application.Run(dialog);

        if (dialog.deleted == true)
        {
            ProcessDeleteUser(currentUser);

        }

        else if (dialog.updated == true)
        {
            ProcessEditUser(dialog, currentUser);
        }

    }

    private void ProcessDeleteUser(User user)
    {
        bool isDeleted = userRepository.Delete(user.id);
        if (isDeleted)
        {
            postRepository.DeleteAllByUserId(user.id);
            commentRepository.DeleteAllByUserId(user.id);
            isUserRemoved=true;
            Application.RequestStop();
        }

        else
        {
            MessageBox.ErrorQuery("Delete user", "Can not delete user.", "OK");
        }

    }

    private void ProcessEditUser(OpenUserDialog dialog, User user)
    {

        User updatedUser = dialog.GetUser();
        if (userRepository.Update(updatedUser, user.id))
        {
            if (user.passwordHash == Authentication.ConvertToHash(""))
            {
                updatedUser.passwordHash = user.passwordHash;
            }
        }

        else
        {
            MessageBox.ErrorQuery("Edit user", "Can not edit user.", "OK");
        }

    }


    public void SetData(UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository, User currentUser)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.currentUser = currentUser;
    }

    public void OnShowMyPostsClicked()
    {
        ShowUserPostsDialog dialog = new ShowUserPostsDialog();
        dialog.SetData(postRepository, commentRepository, currentUser);
        Application.Run(dialog);

    }
}
