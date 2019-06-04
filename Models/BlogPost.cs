using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Automata_DTaylor_Blog.Models
{
    public class BlogPost
    {
        //Properties
        public int Id { get; set; }
        [StringLength(50, MinimumLength =4, ErrorMessage = "The title must be between 4 and 50 characters long.")]
        public string Title { get; set; }
        public string Slug { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public string Abstract { get; set; }
        public string QuoteInit { get; set; }
        public string Quote { get; set; }
        [Display(Name ="Header Image Path")]
        public string MediaUrl { get; set; }
        [Display(Name = "Thumbnail Image Path")]
        public string ThumbUrl { get; set; }
        public bool Published { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        //Virtuals used for navigation
        public virtual ICollection<Comment> Comments { get; set; }

        //Constructor code
        public BlogPost()
        {
            Comments = new HashSet<Comment>();
        }



    }
}