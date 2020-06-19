using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Common;
using TweetBook.Contract.V1.Models;
using TweetBook.Data;
using TweetBook.Domain;
using TweetBook.Options;

namespace TweetBook.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;
        JwtSettings _jwtSettings;
        private IMapper _mapper;
        private ICacheService _cacheService;
        public UserService(DataContext dataContext, JwtSettings jwtSettings, IMapper mapper,ICacheService cacheService)
        {
            _dataContext = dataContext;
            _jwtSettings = jwtSettings;
            _mapper = mapper;
            _cacheService = cacheService;
        }
        public async Task<ActionResult<AuthenticateResponse>> Authenticate(AuthenticateModel model, string ipAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                    return ActionResult<AuthenticateResponse>.Failure("Email is empty");

                var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.Email == model.Email);

                if (user == null)
                    return ActionResult<AuthenticateResponse>.Failure("User does not exist");

                if (!VerifyPasswordHash(model.Password, Convert.FromBase64String(user.PasswordHash), Convert.FromBase64String(user.PasswordSalt)))
                    return ActionResult<AuthenticateResponse>.Failure("Password is not correct");

                var jwtToken = generateJwtToken(user);
                var refreshToken = generateRefreshToken(ipAddress);
                return ActionResult<AuthenticateResponse>.Succeed(new AuthenticateResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Token = jwtToken,
                    RefreshToken=refreshToken.Token
                });
            }
            catch (Exception ex)
            {
                return ActionResult<AuthenticateResponse>.Failure(ex.Message);
            }
           
        }

        public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            var user = await _dataContext.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            // return null if no user found with token
            if (user == null) return null;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            _dataContext.Update(user);
            _dataContext.SaveChanges();

            // generate new jwt
            var jwtToken = generateJwtToken(user);

            return new AuthenticateResponse 
            { 
                Token=jwtToken,
                RefreshToken=newRefreshToken.Token,
                FirstName=user.FirstName,
                LastName=user.LastName,
                Email=user.Email
            };
        }

        public async Task<bool> RevokeToken(string token, string ipAddress)
        {
            var user = _dataContext.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = user.Result.RefreshTokens.Single(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _dataContext.Update(user);
            _dataContext.SaveChanges();

            return true;
        }


        public async Task<ActionResult<IEnumerable<UserModel>>> GetAllAsync()
        {
            try
            {
                 IEnumerable<UserModel> modelList;
                 var cachekey = "GetAllAsync" + nameof(UserService);
                if (_cacheService.TryGet<IEnumerable<UserModel>>(cachekey, out modelList))
                {
                    return ActionResult<IEnumerable<UserModel>>.Succeed(modelList);
                }
                else
                {
                    var users = await _dataContext.Users.ToListAsync();
                    modelList = _mapper.Map<IEnumerable<UserModel>>(users);
                    _cacheService.Set<IEnumerable<UserModel>>(cachekey, modelList);
                    return ActionResult<IEnumerable<UserModel>>.Succeed(modelList);
                }
            }
            catch (Exception ex)
            {
                return ActionResult<IEnumerable<UserModel>>.Failure(ex.Message);
            }
        }

        public async Task<ActionResult<UserModel>> GetByIdAsync(Guid id)
        {
            try
            {
                var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.Id == id);
                var model = _mapper.Map<UserModel>(user);
                if (user != null)
                    return ActionResult<UserModel>.Succeed(model);
                else
                    return ActionResult<UserModel>.Failure("User not found");
            }
            catch (Exception ex)
            {
                return ActionResult<UserModel>.Failure(ex.Message);
            }
            
        }

        public async Task<ActionResult<Guid>> Register(RegisterModel model, string password)
        {
            try
            {
                var dto = _mapper.Map<User>(model);
                dto.Id = Guid.NewGuid();
                if (_dataContext.Users.Any(x => x.Email == dto.Email))
                    return ActionResult<Guid>.Failure("Username \"" + dto.Email + "\" is already taken");
                if (string.IsNullOrWhiteSpace(password))
                    return ActionResult<Guid>.Failure("Password is required");

                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);
                dto.PasswordHash = Convert.ToBase64String(passwordHash);
                dto.PasswordSalt = Convert.ToBase64String(passwordSalt);

                await _dataContext.Users.AddAsync(dto);
                var added = await _dataContext.SaveChangesAsync();
                if (added > 0)
                    return ActionResult<Guid>.Succeed(dto.Id);
                else
                    return ActionResult<Guid>.Failure("Problem  occured");

            }
            catch (Exception ex)
            {
                return ActionResult<Guid>.Failure(ex.Message);
            }

        }

        public async Task<ActionResult> Update(UpdateUserModel updatedUser)
        {
            try
            {
                var user = _dataContext.Users.Find(updatedUser.Id);
                if (user == null)
                    return ActionResult.Failure("User not found");

                if (!string.IsNullOrWhiteSpace(updatedUser.Email) && updatedUser.Email != user.Email)
                {
                    if (_dataContext.Users.Any(x => x.Email == updatedUser.Email))
                        return ActionResult.Failure("Username " + updatedUser.Email + " is already taken");
                    user.Email = updatedUser.Email;
                }

                if (!string.IsNullOrWhiteSpace(updatedUser.FirstName))
                    user.FirstName = updatedUser.FirstName;

                if (!string.IsNullOrWhiteSpace(updatedUser.LastName))
                    user.LastName = updatedUser.LastName;

                _dataContext.Users.Update(user);
                var updated = await _dataContext.SaveChangesAsync();
                if (updated > 0)
                    return ActionResult.Succeed();
                else
                    return ActionResult.Failure("Problem occured");

            }
            catch (Exception ex)
            {
                return ActionResult.Failure(ex.Message);
            }

        }

        public async Task<ActionResult> ChangePassword(Guid Id, string password)
        {
            try
            {
                var user  = await _dataContext.Users.SingleOrDefaultAsync(x => x.Id == Id);
                if (user!=null)
                {
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        byte[] passwordHash, passwordSalt;
                        CreatePasswordHash(password, out passwordHash, out passwordSalt);
                        user.PasswordHash = Convert.ToBase64String(passwordHash);
                        user.PasswordSalt = Convert.ToBase64String(passwordSalt);

                        _dataContext.Users.Update(user);
                        var changed = await _dataContext.SaveChangesAsync();
                        if (changed > 0)
                            return ActionResult.Succeed();

                    }
                    return ActionResult.Failure("Password is empty");
                }
                return ActionResult.Failure("");
            }
            catch (Exception ex)
            {
                return ActionResult.Failure(ex.Message);
            }
        }

        public async Task<ActionResult> Delete(Guid Id)
        {
            try
            {
                 var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.Id == Id);
                if (user!=null)
                {
                    _dataContext.Users.Remove(user);
                    var deleted = await _dataContext.SaveChangesAsync();
                    if (deleted > 0)
                        return ActionResult.Succeed();
                    else
                        return ActionResult.Failure("Problem occured");
                }
                return ActionResult.Failure("User not found");

            }
            catch (Exception ex)
            {
                return ActionResult<bool>.Failure(ex.Message);

            }
        }

        public async Task<ActionResult> AddRole(Guid userId, Guid roleId)
        {
            try
            {
                var user = _dataContext.Users.FirstOrDefault(i => i.Id == userId);
                var role = _dataContext.Roles.FirstOrDefault(i => i.Id == roleId);
                var userRoles = new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                };

                user.UserRoles.Add(userRoles);
                var added = await _dataContext.SaveChangesAsync();

                if (added > 0)
                    return ActionResult.Succeed();
                return ActionResult.Failure("Problem occured");
            }
            catch (Exception ex)
            {
                return ActionResult.Failure(ex.Message);
            }
        }

        #region helper
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        private string generateJwtToken(User user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.FirstName+" "+user.LastName),
                    new Claim("Id", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }

       

        #endregion


    }
}
