using Terminal.Gui;
using System.Collections.Generic;


public class ShowUsersDialog : Dialog
{
    private ListView allUsersListView;

    private int pageLength = 10;
    private int currentPage = 1;
    private Label currentPageLbl;
    private Label allPagesLbl;
    private Button prevPageButton;
    private Button nextPageButton;
    private UserRepository userRepository;
    private PostRepository postRepository;
    private CommentRepository commentRepository;
    private Label isEmptyListLbl;
    private User currentUser;

    public ShowUsersDialog(User currentUser)
    {
        this.currentUser = currentUser;
        this.Title = "Show users";
        Button backBtn = new Button(30, 21, "Back");
        backBtn.Clicked += OnShowDialogBack;
        this.Add(backBtn);
        allUsersListView = new ListView(new List<User>())
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),

        };

        if (currentUser.userName == "ADMIN" && currentUser.passwordHash == "b756562aeca5d42be0705b993c861a473b1c2dbcb782fa730b89d38fd94572ac")
        {
            Button createUserBtn = new Button(4, 18, "Create user");
            createUserBtn.Clicked += OnCreateUserClicked;
            this.Add(createUserBtn);
        }

        allUsersListView.OpenSelectedItem += OnOpenUser;
        prevPageButton = new Button(22, 14, "<");
        nextPageButton = new Button(38, 14, ">");
        this.currentPageLbl = new Label(30, 14, "?");
        Label slash = new Label(32, 14, "/");
        this.allPagesLbl = new Label(34, 14, "?");


        nextPageButton.Clicked += OnNextButtonClicked;
        prevPageButton.Clicked += OnPrevButtonClicked;
        this.Add(prevPageButton, nextPageButton, currentPageLbl, allPagesLbl, slash);

        FrameView frameView = new FrameView("Users")
        {
            X = 4,
            Y = 2,
            Width = Dim.Fill() - 4,
            Height = pageLength + 2,
        };

        frameView.Add(allUsersListView);
        this.Add(frameView);

        isEmptyListLbl = new Label("There is no users.");
        frameView.Add(isEmptyListLbl);
        isEmptyListLbl.Visible = false;




    }


    private void OnCreateUserClicked()
    {
        CreateUserDialog dialog = new CreateUserDialog();
        dialog.SetRegistration(false);
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
                }
                else
                {
                    MessageBox.ErrorQuery("ERROR", "User already exists", "OK");
                }

                ShowCurrentPage();
            }
        }

    }

    private void OnShowDialogBack()
    {
        Application.RequestStop();
    }

    public void SetData(UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        ShowCurrentPage();
    }

    private void OnOpenUser(ListViewItemEventArgs args)
    {
        User user = (User)args.Value;
        OpenUserDialog dialog = new OpenUserDialog(currentUser, user);
        dialog.SetData(user);

        Application.Run(dialog);

        if (dialog.deleted == true)
        {
            ProcessDeleteUser(user);

        }

        else if (dialog.updated == true)
        {
            ProcessEditUser(dialog, user);
        }

    }

    private void OnNextButtonClicked()
    {
        int totalPages = userRepository.GetTotalPages(pageLength);
        if (currentPage >= totalPages)
        {
            return;
        }

        this.currentPage++;
        ShowCurrentPage();

    }

    private void OnPrevButtonClicked()
    {

        int totalPages = userRepository.GetTotalPages(pageLength);
        if (currentPage <= 1)
        {
            return;
        }

        this.currentPage--;
        ShowCurrentPage();
    }

    private void ShowCurrentPage()
    {
        this.currentPageLbl.Text = currentPage.ToString();
        int totalPages = userRepository.GetTotalPages(pageLength);

        if (totalPages == 0)
        {
            totalPages = 1;
        }

        this.allPagesLbl.Text = totalPages.ToString();

        this.allUsersListView.SetSource(userRepository.GetPageOfUsers(currentPage, pageLength));

        prevPageButton.Visible = (currentPage != 1);
        nextPageButton.Visible = (currentPage != int.Parse(this.allPagesLbl.Text.ToString()));

        if (userRepository.GetPageOfUsers(currentPage, pageLength).Count == 0)
        {
            isEmptyListLbl.Visible = true;
        }

        else
        {
            isEmptyListLbl.Visible = false;
        }

    }

    private void ProcessDeleteUser(User user)
    {
        bool isDeleted = userRepository.Delete(user.id);
        if (isDeleted)
        {
            postRepository.DeleteAllByUserId(user.id);
            commentRepository.DeleteAllByUserId(user.id);
            int countOfPages = userRepository.GetTotalPages(pageLength);
            if (currentPage > countOfPages && currentPage > 1)
            {
                currentPage--;
            }
            ShowCurrentPage();
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
            ShowCurrentPage();
        }

        else
        {
            MessageBox.ErrorQuery("Edit user", "Can not edit user.", "OK");
        }

    }

}
