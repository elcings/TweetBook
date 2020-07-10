using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetBook.Contract.V1.Models;

namespace TweetBook.Sdk
{
    [Headers("Authorization: Bearer")]
    public interface ITweetBokkApi
    {
        [Get("/api/v1/posts")]
        Task<List<ResponsePost>> GetAll();
        [Get("/api/v1/posts{postId}")]
        Task<ResponsePost> Get(Guid postId);
        [Post("/api/v1/posts/add")]
        Task<CreatePostModel> CreatePost([Body]CreatePostModel model);
        [Put("/api/v1/posts/edit{postId}")]
        Task<CreatePostModel> UpdatePost(Guid postId, [Body]UpdatePostModel model);
        [Delete("/api/v1/posts/delete{postId}")]
        Task<CreatePostModel> RemovePost(Guid postId);
    }
}
