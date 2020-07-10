using System;
using System.Collections.Generic;
using System.Text;

namespace TweetBook.Contract.V1.Models
{
    public class ResponsePost
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
