﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TweetBook.Common;
using TweetBook.Contract.V1;
using TweetBook.Contract.V1.Models;
using TweetBook.Domain;
using TweetBook.Services;

namespace TweetBook.Controllers.V1
{
    public class PostController : MainController
    {
        private IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;   
        }


        [HttpGet(ApiRoute.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetAllAsync();
            if (posts.IsSucceed)
                return Ok(posts);
            return BadRequest(posts.FailureResult);
        }


        [HttpGet(ApiRoute.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute]Guid postId)
        {
            var post = await _postService.GetByIdAsync(postId);
            if (post.IsSucceed)
                return Ok(post);
            return BadRequest(post.FailureResult);
        }

        [HttpDelete(ApiRoute.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute]Guid postId)
        {
            var deleted =await _postService.DeleteAsync(postId);
            if (deleted.IsSucceed)
                return Ok();
            return BadRequest(deleted.FailureResult);
        }
        [HttpPut(ApiRoute.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute]Guid postId, [FromBody]UpdatePostModel post)
        {
            post.Id = postId;
            var updated = await _postService.UpdateAsync(post);
            if (updated.IsSucceed)
                return Ok();
            return BadRequest(updated.FailureResult);
        }

        [HttpPost(ApiRoute.Posts.Create)]
        public async Task<IActionResult> Create([FromBody]CreatePostModel post)
        {
            var createdId= await _postService.CreateAsync(post);
            if (createdId.IsSucceed)
            {
                string baseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
                string createdUri = baseUri + "/" + ApiRoute.Posts.Get.Replace("{postId}", createdId.Data.ToString());
                return Created(createdUri, post);
            }
            return BadRequest(createdId.FailureResult);
        }
    }
}