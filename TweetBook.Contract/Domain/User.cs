using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TweetBook.Domain  
{
    public class User
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
        }
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }

    }
}
