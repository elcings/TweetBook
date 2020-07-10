using FluentAssertions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TweetBook.Contract.V1;
using TweetBook.Contract.V1.Models;
using Xunit;

namespace TweetBokk.Integration
{
    public class PostControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithoutAnyPosts_ReturnEmptyResponse()
        {
            //arrangesss
            await Auth();
            //act
            var response =await _httpClient.GetAsync(ApiRoute.Posts.GetAll);
            // 1Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadAsStringAsync();
            (JsonConvert.DeserializeObject<List<ResponsePost>>(result)).Should().BeEmpty();
        }

        [Fact]
        public async Task Get__ReturnResponse()
        {
            //arrange
            await Auth();
            var createdId= await CreatePost(new CreatePostModel { Content = "testststststststst", Title = "Test" });
            //act
            var response = await _httpClient.GetAsync(ApiRoute.Posts.Get.Replace("{postId}", createdId.ToString()));
            // 1Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = JsonConvert.DeserializeObject<ResponsePost>(await response.Content.ReadAsStringAsync());
            result.Id.Should().Be(createdId);
            result.Title.Should().Be("Test");
        }
    }
}
