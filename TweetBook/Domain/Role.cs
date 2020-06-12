﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetBook.Domain
{
    public class Role
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
