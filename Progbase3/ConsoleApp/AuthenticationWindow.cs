using Terminal.Gui;


public class AuthenticationWindow : Window
{
    private UserRepository userRepository;

    private PostRepository postRepository;
    private CommentRepository commentRepository;
    private TextField userNameInput;
    private TextField passwordInput;

    public AuthenticationWindow()
    {
        int rightColumn = 25;
        int coordinateX = 2;
        this.Title = "Authentication";

        Label userNameLbl = new Label(coordinateX, 4, "User name");
        userNameInput = new TextField()
        {
            X = rightColumn,
            Y = Pos.Top(userNameLbl),
            Width = 20,
            Height = 8,
        };
        this.Add(userNameLbl, userNameInput);

        Label passwordLbl = new Label(coordinateX, 8, "Password");
        passwordInput = new TextField()
        {
            X = rightColumn,
            Y = Pos.Top(passwordLbl),
            Width = 20,
            Height = 8,
            Secret = true,
        };
        this.Add(passwordLbl, passwordInput);

        Button logInBtn = new Button(coordinateX, 10, "LOG IN");
        logInBtn.Clicked += OnLogInClicked;
        this.Add(logInBtn);

        Button registerBtn = new Button(coordinateX, 14, "Register");
        registerBtn.Clicked += OnRegisterClicked;
        this.Add(registerBtn);


    }

    private void OnRegisterClicked()
    {
        CreateUserDialog dialog = new CreateUserDialog();
        dialog.SetRegistration(true);
        Application.Run(dialog);

        if (dialog.canceled == false)
        {
            User user = dialog.GetUserFromFields();
            if (user == null)
            {
                MessageBox.ErrorQuery("Create user", "Can not create user.\nAll fields must be filled in the correct format", "OK");
            }

            else
            {
                if (!userRepository.UserExists(user.userName, user.passwordHash))
                {
                    long id = userRepository.Insert(user);
                    user.id = id;
                    Toplevel top = Application.Top;
                    MainWindow window = new MainWindow(user);
                    window.SetData(userRepository, postRepository, commentRepository);
                    top.Add(window);
                    Application.Run();
                }
                else
                {
                    MessageBox.ErrorQuery("ERROR", "User already exists", "OK");
                }
            }
        }
    }

    public void SetRepository(UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    private void OnLogInClicked()
    {
        if (userNameInput.Text != "" && passwordInput.Text != "")
        {
            string passwordHash = Authentication.ConvertToHash(passwordInput.ToString());
            if (userRepository.UserExists(userNameInput.Text.ToString(), passwordHash))
            {
                Application.Init();
                User currentUser = userRepository.GetUser(userNameInput.Text.ToString(), passwordHash);
                if (currentUser == null)
                {
                    MessageBox.ErrorQuery("Incorrect information", "Can't log in. Try again.", "OK");
                }
                else
                {
                    Toplevel top = Application.Top;
                    MainWindow window = new MainWindow(currentUser);
                    window.SetData(userRepository, postRepository, commentRepository);
                    top.Add(window);
                    Application.Run();
                }
            }
            else
            {
                MessageBox.ErrorQuery("Incorrect information", "Can't log in. Try again.", "OK");
            }


        }
        else
        {
            MessageBox.ErrorQuery("Incorrect information", "You have not filled in the fields", "OK");
        }

    }

}
