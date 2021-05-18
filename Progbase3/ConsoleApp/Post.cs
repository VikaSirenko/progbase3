using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlType(TypeName = "post")]
public class Post
{
    public long id;
    public string publicationText;
    public DateTime publishedAt;
    public long userId;
    public long pinCommentId;

    [XmlElement("comment")]
    public List<Comment> comments;

    public Post()
    {
        this.id = default;
        this.publicationText = default;
        this.publishedAt = default;
        this.userId = default;
        this.pinCommentId = default;
    }



}
