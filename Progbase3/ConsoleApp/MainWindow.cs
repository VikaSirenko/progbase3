using Terminal.Gui;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class MainWindow : Window
    {

        private UserRepository userRepository;
        private PostRepository postRepository;
        private CommentRepository commentRepository;

        public MainWindow()
        {
            this.Title = "Task Room";

            Button showUsersBtn = new Button(2, 4, "Show users");
            showUsersBtn.Clicked += OnShowUsersClicked;
            this.Add(showUsersBtn);

            Button showPostsBtn = new Button(2, 8, "Show posts");
            showPostsBtn.Clicked += OnShowPostsClicked;
            this.Add(showPostsBtn);

            Button showCommentsBtn = new Button(2, 12, "Show comments");
            showCommentsBtn.Clicked += OnShowCommentsClicked;
            this.Add(showCommentsBtn);


        }

        public void SetRepository(UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository)
        {
            this.userRepository = userRepository;
            this.postRepository = postRepository;
            this.commentRepository = commentRepository;
        }




        private void OnShowUsersClicked()
        {
            ShowUsersDialog dialog = new ShowUsersDialog();
            dialog.SetRepository(userRepository);
            Application.Run(dialog);
        }
        private void OnShowPostsClicked()
        {
            ShowPostsDialog dialog = new ShowPostsDialog();
            dialog.SetRepository(postRepository);
            Application.Run(dialog);
        }

        private void OnShowCommentsClicked()
        {
            ShowCommentsDialog dialog = new ShowCommentsDialog();
            dialog.SetRepository(commentRepository);
            Application.Run(dialog);
        }




    }




}
