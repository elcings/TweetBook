using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Common;
using TweetBook.Contract.V1.Models;
using TweetBook.Domain;

namespace TweetBook.Services
{
    public interface IPostService
    {
        Task<ActionResult<IEnumerable<Post>>> GetAllAsync();
        Task<ActionResult<Post>> GetByIdAsync(Guid Id);
        Task<ActionResult<Guid>> CreateAsync(CreatePostModel post);
        Task<ActionResult> UpdateAsync(UpdatePostModel post);
        Task<ActionResult> DeleteAsync(Guid Id);
    }
}
