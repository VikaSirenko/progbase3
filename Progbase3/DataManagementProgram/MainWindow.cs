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

        if (currentUser.userName == "ADMIN" && currentUser.passwordHash == "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824")
        {

            Button showAllcomments = new Button(2, 12, "Show all comments");
            showAllcomments.Clicked += OnShowCommentsClicked;
            this.Add(showAllcomments);

        }


        Button showUsersBtn = new Button(2, 8, "Show all users");
        showUsersBtn.Clicked += OnShowUsersClicked;
        this.Add(showUsersBtn);

        Button showPostsBtn = new Button(2, 10, "Show all posts");
        showPostsBtn.Clicked += OnShowPostsClicked;
        this.Add(showPostsBtn);




        Button showInformationBtn = new Button(2, 14, "Show my information");
        showInformationBtn.Clicked += OnShowInformationClicked;
        this.Add(showInformationBtn);

        MenuBar menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("File", new MenuItem [] {
                    new MenuItem ("Import posts","", OnImport),
                    new MenuItem("Export posts", "", OnExport),
                    new MenuItem("Exit", "", OnExit)
                }),
                new MenuBarItem("Help", new MenuItem[]{
                    new MenuItem ("About program","", OnAboutProgram)
                })
            });

        this.Add(menu);

        Button genereteReportBtn = new Button(2, 16, "Generete report");
        genereteReportBtn.Clicked += OnGenereteReportClicked;
        this.Add(genereteReportBtn);


    }

    private void OnAboutProgram()
    {
        AboutProgramDialog dialog = new AboutProgramDialog();
        Application.Run(dialog);
    }

    private void OnGenereteReportClicked()
    {
        GeneretionReportDialog dialog = new GeneretionReportDialog();
        dialog.SetData(currentUser, postRepository, commentRepository);
        Application.Run(dialog);

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
        dialog.SetData(postRepository, commentRepository, currentUser);
        Application.Run(dialog);
    }

    private void OnExit()
    {
        Application.RequestStop();
    }
    private void OnShowInformationClicked()
    {
        ShowUserDataDialog dialog = new ShowUserDataDialog();
        dialog.SetData(userRepository, postRepository, commentRepository, currentUser);
        Application.Run(dialog);
        if (dialog.isUserRemoved == true)
        {
            Application.RequestStop();
        }
    }


    public void SetData(UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    private void OnShowUsersClicked()
    {
        ShowAllUsersDialog dialog = new ShowAllUsersDialog(currentUser);
        dialog.SetData(userRepository, postRepository, commentRepository);
        Application.Run(dialog);
    }

    private void OnShowPostsClicked()
    {
        ShowAllPostsDialog dialog = new ShowAllPostsDialog(currentUser);
        dialog.SetData(postRepository, commentRepository);
        Application.Run(dialog);
    }

    private void OnShowCommentsClicked()
    {
        ShowAllCommentsDialog dialog = new ShowAllCommentsDialog(currentUser);
        dialog.SetData(commentRepository);
        Application.Run(dialog);
    }




}


