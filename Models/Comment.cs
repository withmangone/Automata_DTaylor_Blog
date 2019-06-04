using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_Blog.Models
{
    public class Comment
    {
        //primary key
        public int Id { get; set; }
        //foreign keys
        public int BlogPostId { get; set; }
        public string AuthorId { get; set; }

        public string CommentBody { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public string UpdateBy { get; set; }
        public string UpdateReason { get; set; }

        //navigational properties
        public virtual BlogPost BlogPost { get; set; }
        public virtual ApplicationUser Author { get; set; }

    }
}