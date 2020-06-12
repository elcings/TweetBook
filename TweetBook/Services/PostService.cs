using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Common;
using TweetBook.Contract.V1.Models;
using TweetBook.Data;
using TweetBook.Domain;

namespace TweetBook.Services
{
    public class PostService : IPostService
    {
        private DataContext _dataContext;
        private IMapper _mapper;
        public PostService(DataContext dataContext,IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        public async Task<ActionResult<Guid>> CreateAsync(CreatePostModel post)
        {
            try
            {
                var dto = _mapper.Map<Post>(post);
                dto.Id = Guid.NewGuid();
                await _dataContext.Posts.AddAsync(dto);
                var created = await _dataContext.SaveChangesAsync();
                if (created > 0)
                    return ActionResult<Guid>.Succeed(dto.Id);
                else
                    return ActionResult<Guid>.Failure("Problem occured");
            }
            catch (Exception ex)
            {
                return ActionResult<Guid>.Failure(ex.Message);
            }

        }

        public async Task<ActionResult<Post>> GetByIdAsync(Guid Id)
        {
            try
            {
                var post = await _dataContext.Posts.SingleOrDefaultAsync(p => p.Id == Id);
                if (post != null)
                    return ActionResult<Post>.Succeed(post);
                else
                    return ActionResult<Post>.Failure("Post not found");
            }
            catch (Exception ex)
            {
                return ActionResult<Post>.Failure(ex.Message);
            }
        }

        public async Task<ActionResult<IEnumerable<Post>>> GetAllAsync()
        {
            try
            {
                var users = await _dataContext.Posts.ToListAsync();
                return ActionResult<IEnumerable<Post>>.Succeed(users);
            }
            catch (Exception ex)
            {
                return ActionResult<IEnumerable<Post>>.Failure(ex.Message);
            }

        }

        public async Task<ActionResult> UpdateAsync(UpdatePostModel post)
        {
            try
            {
                var dto = _mapper.Map<Post>(post);
                _dataContext.Posts.Update(dto);
                var updated = await _dataContext.SaveChangesAsync();
                if (updated > 0)
                    return ActionResult.Succeed();
                return ActionResult.Failure("Problem occured");
            }
            catch (Exception ex)
            {
                return ActionResult.Failure(ex.Message);
            }
        }

        public async Task<ActionResult> DeleteAsync(Guid Id)
        {
            try
            {
                var post = await GetByIdAsync(Id);
                if (post.IsSucceed)
                {
                    _dataContext.Posts.Remove(post.Data);
                    var deleted = await _dataContext.SaveChangesAsync();
                    if (deleted > 0)
                        return ActionResult.Succeed();
                    else
                        return ActionResult.Failure("Problem occured when delete post");
                }
                return ActionResult.Failure("Post not found");
            }
            catch (Exception ex)
            {
                return ActionResult.Failure(ex.Message);
            }
        }
    }
}
