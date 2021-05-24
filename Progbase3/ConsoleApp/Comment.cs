using System;

public class Comment
{
    public long id;
    public string commentText;
    public DateTime commentedAt;
    public long userId;
    public long postId;

    public Comment()
    {
        this.id = default;
        this.commentText = default;
        this.commentedAt = default;
        this.userId = default;
        this.postId = default;
    }

    public override string ToString()
    {
        return $"[{id}] | Comment:'{commentText}'";
    }

}
