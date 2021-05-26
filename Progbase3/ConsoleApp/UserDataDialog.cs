using Terminal.Gui;
public class UserDataDialog : Dialog
{
    private UserRepository userRepository;
    private PostRepository postRepository;
    private CommentRepository commentRepository;
    private User currentUser;

    public UserDataDialog()
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
        dialog.SetData(currentUser);
        Application.Run(dialog);
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
