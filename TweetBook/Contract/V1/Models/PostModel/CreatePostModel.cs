using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TweetBook.Contract.V1.Models
{
    public class CreatePostModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

    }
}
