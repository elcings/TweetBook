﻿using System;
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
        Task<ActionResult<AuthenticateResponse>> Authenticate(AuthenticateModel model,string ipaddress);
        Task<ActionResult<AuthenticateResponse>> Register(RegisterModel user, string password);
        Task<ActionResult> Update(UpdateUserModel user);
        Task<ActionResult> Delete(Guid Id);
        Task<ActionResult> ChangePassword(Guid Id, string password);
        Task<ActionResult> AddRole(Guid userId, Guid roleId);
        Task<AuthenticateResponse> RefreshToken(string token, string ipaddress);
        Task<bool> RevokeToken(string token, string ipAddress);
        Task<ActionResult<IEnumerable<UserModel>>> GetAllAsync();
        Task<ActionResult<UserModel>> GetByIdAsync(Guid id);
    }
}
