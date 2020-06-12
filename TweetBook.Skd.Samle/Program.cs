using Refit;
using System.Threading.Tasks;
using TweetBook.Sdk;

namespace TweetBook.Skd.Samle
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cachedToken = string.Empty;
            var identityApi = RestService.For<IIdentityApi>("http://localhost:5000/");
            var tweetBookApi = RestService.For<ITweetBokkApi>("http://localhost:5000/", new RefitSettings
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(cachedToken)
            });
            //var registerResponse = await identityApi.Register(new Contract.V1.Models.RegisterModel()
            //{
            //    FirstName = "Elcin",
            //    LastName = "Aliyev",
            //    Email = "elcinaliyevgs@gmail.com",
            //    Password = "elcings85"
            //});
            var login =await identityApi.Login(new Contract.V1.Models.AuthenticateModel
            {
                Username = "elcinaliyevgs@gmail.com",
                Password = "elcings85"
            });

            cachedToken = login.Token;
            var createPost =await tweetBookApi.CreatePost(new Contract.V1.Models.CreatePostModel
            {
                Title = "Elcin Refit",
                Content = "jhdgadgajahgdjagajdgajdahgdhghagdajdgakdjaada;d;aakj;adajkjkjkjkjkjkk"
            });
        }
    }
}
