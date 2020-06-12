using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TweetBook.Common;
using TweetBook.Contract.V1;
using TweetBook.Contract.V1.Models;
using TweetBook.Services;

namespace TweetBook.Controllers.V1
{
    public class UserController : MainController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.Users.Authenticate)]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateModel model)
        {
            var guid = Guid.NewGuid();
            var token = await _userService.Authenticate(model.Username, model.Password);

            if (token.IsSucceed)
                return Ok(token.Data);
            return BadRequest(token.FailureResult);
        }

        [HttpGet(ApiRoute.Users.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            if (users.IsSucceed)
                return Ok(users);
            else
                return BadRequest(users.FailureResult);
        }

        [Route("users/getuser")]
        [HttpGet]
        public async Task<IActionResult> GetUserID()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("Id", StringComparison.InvariantCultureIgnoreCase));
            return Ok(userId.Value);
        }
        [HttpGet(ApiRoute.Users.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid Id)
        {
            var user = await _userService.GetByIdAsync(Id);
            if (user.IsSucceed)
                return Ok(user);
            return BadRequest(user.FailureResult);
        }



        [HttpPost(ApiRoute.Users.AddRole)]
        public async Task<IActionResult> AddRole([FromRoute]Guid userId,[FromBody]URole role)
        {
            var user = await _userService.AddRole(userId, role.RoleId);
            if (user.IsSucceed)
                return Ok(user);
            return BadRequest(user.FailureResult);
        }

        [HttpPut(ApiRoute.Users.ChangePassword)]
        public async Task<IActionResult> ChangePassword([FromRoute]Guid userId, [FromBody]string password)
        {
            var user = await _userService.ChangePassword(userId, password);
            if (user.IsSucceed)
                return Ok(user);
            return BadRequest(user.FailureResult);
        }

        
       
        [HttpPost(ApiRoute.Users.Register)]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
        {
            var created = await _userService.Register(model, model.Password);
            if (created.IsSucceed)
            {
                string baseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
                string createdUri = baseUri + "/" + ApiRoute.Users.Get.Replace("{Id}", created.Data.ToString());
                return Created(createdUri, model);
            }
            else
                return BadRequest(created.FailureResult);
        }


        [HttpPut(ApiRoute.Users.Update)]
        public async Task<IActionResult> Update([FromRoute]Guid Id, [FromBody]UpdateUserModel model)
        {
            model.Id = Id;
            var updated = await _userService.Update(model);
            if (updated.IsSucceed)
                return Ok();
            return BadRequest(updated.FailureResult);

        }

        [HttpDelete(ApiRoute.Users.Delete)]
        public async Task<IActionResult> Delete([FromRoute]Guid Id)
        {
            var deleted = await _userService.Delete(Id);
            if (deleted.IsSucceed)
                return Ok();
            return BadRequest(deleted.FailureResult);

        }


    }
}
