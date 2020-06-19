using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Contract.V1.Models;

namespace TweetBook.Sdk
{
    public interface IIdentityApi
    {
        [Post("/api/v1/users/register")]
        Task<AuthenticateResponse> Register([Body]RegisterModel model);

        [Post("/api/v1/users/authenticate")]
        Task<AuthenticateResponse> Login([Body]AuthenticateModel model);

        [Get("/api/v1/users{Id}")]
        Task<UserModel> GetById(Guid Id);

        [Get("/api/v1/users")]
        Task<IEnumerable<UserModel>> GetAll();
    }
}
