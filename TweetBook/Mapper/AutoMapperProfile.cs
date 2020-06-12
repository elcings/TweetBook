using AutoMapper;
using TweetBook.Contract.V1.Models;
using TweetBook.Domain;

namespace TweetBook.Mapper
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UpdateUserModel, User>();
            CreateMap<RegisterModel, User>();
            CreateMap<User,UserModel>();

            CreateMap<UpdatePostModel, Post>();
            CreateMap<CreatePostModel, Post>();
            CreateMap<Post, ResponsePost>();
        }
    }
}
