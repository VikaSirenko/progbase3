using System;
using System.Xml.Serialization;

public class Comment
{
    public long id;
    public string commentText;
    public DateTime commentedAt;
    public long userId;
    public long postId;

    [XmlIgnore]
    public bool imported;

    public Comment()
    {
        this.id = default;
        this.commentText = default;
        this.commentedAt = default;
        this.userId = default;
        this.postId = default;
        this.imported = false;

    }

    public override string ToString()
    {
        return $"[{id}] | Comment:'{commentText}'";
    }

}
