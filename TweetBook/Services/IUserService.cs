using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Common;
using TweetBook.Contract.V1.Models;
using TweetBook.Domain;

namespace TweetBook.Services
{
    public interface IUserService
    {
        Task<ActionResult<ReturnUser>> Authenticate(string username, string password);
        Task<ActionResult<IEnumerable<UserModel>>> GetAllAsync();
        Task<ActionResult<User>> GetByIdAsync(Guid id);
        Task<ActionResult<Guid>> Register(RegisterModel user, string password);
        Task<ActionResult> Update(UpdateUserModel user);
        Task<ActionResult> Delete(Guid Id);
        Task<ActionResult> ChangePassword(Guid Id, string password);
        Task<ActionResult> AddRole(Guid userId, Guid roleId);
    }
}
