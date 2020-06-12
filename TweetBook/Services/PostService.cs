using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        private ICacheService _cacheService;
        public PostService(DataContext dataContext,IMapper mapper,ICacheService cacheService)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _cacheService = cacheService;
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

        public async Task<ActionResult<ResponsePost>> GetByIdAsync(Guid Id)
        {
            try
            {
                ResponsePost model;
                if (_cacheService.TryGet<ResponsePost>(Id.ToString(), out model))
                {
                    return ActionResult<ResponsePost>.Succeed(model);
                }
                else
                {
                    var post = await _dataContext.Posts.SingleOrDefaultAsync(p => p.Id == Id);
                    model = _mapper.Map<ResponsePost>(post);
                    _cacheService.Set<ResponsePost>(Id.ToString(), model);
                    if (model != null)
                        return ActionResult<ResponsePost>.Succeed(model);
                    else
                        return ActionResult<ResponsePost>.Failure("Post not found");
                }
            }
            catch (Exception ex)
            {
                return ActionResult<ResponsePost>.Failure(ex.Message);
            }
        }

        public async Task<ActionResult<IEnumerable<ResponsePost>>> GetAllAsync()
        {
            try
            {
                IEnumerable<ResponsePost> modelList;
                if (_cacheService.TryGet<IEnumerable<ResponsePost>>("posts",out modelList))
                {
                    return ActionResult<IEnumerable<ResponsePost>>.Succeed(modelList);
                }
                else {
                    var posts = await _dataContext.Posts.ToListAsync();
                     modelList = _mapper.Map<IEnumerable<ResponsePost>>(posts);
                    _cacheService.Set<IEnumerable<ResponsePost>>("posts", modelList);
                    return ActionResult<IEnumerable<ResponsePost>>.Succeed(modelList);
                }
               
            }
            catch (Exception ex)
            {
                return ActionResult<IEnumerable<ResponsePost>>.Failure(ex.Message);
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
                var post= await _dataContext.Posts.SingleOrDefaultAsync(x => x.Id == Id);
                if (post!=null)
                {
                    _dataContext.Posts.Remove(post);
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
