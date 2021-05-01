using System;
using System.Collections.Generic;


public class Post
{
    public long id;
    public string publicationText;
    public DateTime publishedAt;
    public long userId;
    public long pinCommentId;

    public List<Comment> comments;

    public Post()
    {
        this.id= default;
        this.publicationText = default;
        this.publishedAt = default;
        this.userId = default;
        this.pinCommentId = default;
    }


    
}
