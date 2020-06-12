using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Domain;

namespace TweetBook
{
    public static class FakeData
    {
        private static List<Post> _posts;
        public static List<Post> GetAllPosts()
        {
            if (_posts == null)
            {
                _posts = new Faker<Post>("en")
                   .RuleFor(p => p.Id, f => Guid.NewGuid())
                   .RuleFor(p => p.Content, f => f.Lorem.Sentences(10)).Generate(10);
            }
            return _posts;
        }
    }
}
