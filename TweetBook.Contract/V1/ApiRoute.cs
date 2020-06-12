using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetBook.Contract.V1
{
    public static class ApiRoute
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = Root+"/"+Version;
        public static class Posts
        {
            public  const string GetAll = Base+"/posts";
            public const string Get = Base + "/posts{postId}";
            public const string Create = Base + "/posts/add";
            public const string Update = Base + "/posts/edit{postId}";
            public const string Delete = Base + "/posts/delete{postId}";
        }

        public static class Users
        {
            public const string GetAll = Base + "/users";
            public const string Get = Base + "/users{Id}";
            public const string Register = Base + "/users/register";
            public const string Update = Base + "/users/edit{Id}";
            public const string Delete = Base + "/users/delete{Id}";
            public const string AddRole = Base + "/users/addRole{userId}";
            public const string ChangePassword = Base + "/users/changepassword{userId}";
            public const string Authenticate = Base + "/users/authenticate";

        }
    }
}
