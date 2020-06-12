using System.ComponentModel.DataAnnotations;

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
