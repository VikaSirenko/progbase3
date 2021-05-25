using Terminal.Gui;


public class MainWindow : Window
{
    private UserRepository userRepository;
    private PostRepository postRepository;
    private CommentRepository commentRepository;
    private User currentUser;

    public MainWindow(User currentUser)
    {
        this.currentUser = currentUser;
        this.Title = "My social network";

        Label greetingLbl = new Label(2, 4, $"Hi, {currentUser.fullname}");
        this.Add(greetingLbl);

        if (currentUser.userName == "ADMIN" && currentUser.passwordHash == "b756562aeca5d42be0705b993c861a473b1c2dbcb782fa730b89d38fd94572ac")
        {
            Button showUsersBtn = new Button(2, 8, "Show users");
            showUsersBtn.Clicked += OnShowUsersClicked;
            this.Add(showUsersBtn);

            Button showPostsBtn = new Button(2, 10, "Show posts");
            showPostsBtn.Clicked += OnShowPostsClicked;
            this.Add(showPostsBtn);

            Button showCommentsBtn = new Button(2, 12, "Show comments");
            showCommentsBtn.Clicked += OnShowCommentsClicked;
            this.Add(showCommentsBtn);

            Button showMyPostsBtn = new Button(2, 14, "Show my posts");
            showMyPostsBtn.Clicked += OnShowMyPostsClicked;
            this.Add(showMyPostsBtn);

        }
        else
        {

            Button showUsersBtn = new Button(2, 8, "Show all users");
            showUsersBtn.Clicked += OnShowUsersClicked;
            this.Add(showUsersBtn);

            Button showPostsBtn = new Button(2, 10, "Show all posts");
            showPostsBtn.Clicked += OnShowPostsClicked;
            this.Add(showPostsBtn);

            Button showMyPostsBtn = new Button(2, 12, "Show my posts");
            showMyPostsBtn.Clicked += OnShowMyPostsClicked;
            this.Add(showMyPostsBtn);

        }

        MenuBar menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("File", new MenuItem [] {
                    new MenuItem ("Import posts","", OnImport),
                    new MenuItem("Export posts", "", OnExport),
                    new MenuItem("Exit", "", OnExit)
                }),
                new MenuBarItem("Help", new MenuItem[]{
                })
            });

        this.Add(menu);


    }

    private void OnImport()
    {
        ImportPostDialog dialog = new ImportPostDialog();
        dialog.SetData(postRepository, commentRepository);
        Application.Run(dialog);

    }

    private void OnExport()
    {
        ExportPostDialog dialog = new ExportPostDialog();
        dialog.SetData(postRepository);
        Application.Run(dialog);
    }

    private void OnExit()
    {
        Application.RequestStop();
    }

    public void OnShowMyPostsClicked()
    {
        ShowMyPostsDialog dialog = new ShowMyPostsDialog();
        dialog.SetData(postRepository, commentRepository, currentUser);
        Application.Run(dialog);

    }

    public void SetData(UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    private void OnShowUsersClicked()
    {
        ShowUsersDialog dialog = new ShowUsersDialog(currentUser);
        dialog.SetData(userRepository, postRepository, commentRepository);
        Application.Run(dialog);
    }

    private void OnShowPostsClicked()
    {
        ShowPostsDialog dialog = new ShowPostsDialog(currentUser);
        dialog.SetData(postRepository, commentRepository);
        Application.Run(dialog);
    }

    private void OnShowCommentsClicked()
    {
        ShowCommentsDialog dialog = new ShowCommentsDialog(currentUser);
        dialog.SetData(commentRepository);
        Application.Run(dialog);
    }




}


