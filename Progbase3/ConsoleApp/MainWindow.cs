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

            Button createUserBtn = new Button(2, 4, "Create user");
            createUserBtn.Clicked += OnCreateUserClicked;
            this.Add(createUserBtn);

            Button createPostBtn = new Button(2, 6, "Create post");
            createPostBtn.Clicked += OnCreatePostClicked;
            this.Add(createPostBtn);

            Button createCommentBtn = new Button(2, 8, "Create comment");
            createCommentBtn.Clicked += OnCreateCommentClicked;
            this.Add(createCommentBtn);

            Button showUsersBtn = new Button(2, 10, "Show users");
            showUsersBtn.Clicked += OnShowUsersClicked;
            this.Add(showUsersBtn);

            Button showPostsBtn = new Button(2, 12, "Show posts");
            showPostsBtn.Clicked += OnShowPostsClicked;
            this.Add(showPostsBtn);

            Button showCommentsBtn = new Button(2, 14, "Show comments");
            showCommentsBtn.Clicked += OnShowCommentsClicked;
            this.Add(showCommentsBtn);


        }

        public void SetRepository(UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository)
        {
            this.userRepository = userRepository;
            this.postRepository = postRepository;
            this.commentRepository = commentRepository;
        }
        private void OnCreateUserClicked()
        {

            CreateUserDialog dialog = new CreateUserDialog();
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
                    long id = userRepository.Insert(user);
                    user.id = id;
                }
            }


        }

        private void OnCreatePostClicked()
        {

            CreatePostDialog dialog = new CreatePostDialog();
            Application.Run(dialog);

            if (dialog.canceled == false)
            {
                Post post = dialog.GetPostFromFields();
                if (post == null)
                {
                    MessageBox.ErrorQuery("Create post", "Can not create post.\nAll fields must be filled in the correct format", "OK");
                }

                else
                {
                    post.pinCommentId = default;          //TODO
                    post.userId = default;                //TODO

                    long id = postRepository.Insert(post);
                    post.id = id;
                }
            }

        }

        private void OnCreateCommentClicked()
        {

            CreateCommentDialog dialog = new CreateCommentDialog();
            Application.Run(dialog);

            if (dialog.canceled == false)
            {
                Comment comment = dialog.GetCommentFromFields();
                if (comment == null)
                {
                    MessageBox.ErrorQuery("Create comment", "Can not create comment.\nAll fields must be filled in the correct format", "OK");
                }

                else
                {
                    comment.userId = default; //TODO
                    comment.postId = default; //TODO

                    long id = commentRepository.Insert(comment);
                    comment.id = id;
                }
            }

        }

        private void OnShowUsersClicked()
        {

        }
        private void OnShowPostsClicked()
        {

        }

        private void OnShowCommentsClicked()
        {

        }




    }




}
