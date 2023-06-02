using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TesteTecnicoPloomes.Data;
using TesteTecnicoPloomes.DTO;
using TesteTecnicoPloomes.Repositories;
using TesteTecnicoPloomes.Repositories.Interfaces;

namespace TesteTecnicoPloomes.Controllers
{
    [Route("v1/post")]
    [ApiController]
    public class PostController : ControllerBase
    {
        IPostRepository _postRepository;
        IUserRepository _userRepository;

        public PostController(IPostRepository postRepository, IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("fetchAll")]
        [Authorize(Roles = "Admin, Editor")]
        public async Task<ActionResult<dynamic>> FetchAllPosts()
        {
            try
            {
                return Ok(new { posts = await _postRepository.GetAllAsync() });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("/{id}")]
        public async Task<ActionResult<dynamic>> FetchById(int id)
        {
            try
            {
                var currentUser = User.Claims.FirstOrDefault(c => c.Type == "name").Value;

                var post = await _postRepository.GetByIdAsync(id);

                if (post == null) {
                    return BadRequest("There's no post with this ID");
                }
                else if (post.Owner.Username == currentUser || post.Public)
                {
                    return Ok(new { posts = await _postRepository.GetByIdAsync(id) });
                }
                else
                {
                    return Unauthorized(new
                    {
                        message = "You are not authorized to see this post.",
                        cause = "Not authorized role or Post is not public."
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("fetchPublic")]
        public async Task<ActionResult<dynamic>> FetchPublicPosts()
        {
            try
            {
                return Ok(new { posts = _postRepository.GetPublic() });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("fetchOwn")]
        [Authorize]
        public async Task<ActionResult<dynamic>> FetchOwnPosts()
        {
            var currentUser = User.Claims.FirstOrDefault(c => c.Type == "name").Value;
            try
            {
                var user = (from c
                                     in await _userRepository.GetAllAsync()
                            where c.Username.Equals(currentUser)
                            select c).SingleOrDefault();

                return Ok(new { posts = _postRepository.GetOwn(user) });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Roles ="Admin, Publisher, Editor")]
        public async Task<ActionResult<dynamic>> CreatePost([FromBody] PostDTO postDTO)
        {

            try
            {
                var currentUser = User.Claims.FirstOrDefault(c => c.Type == "name").Value;

                var user = (from c
                                    in await _userRepository.GetAllAsync()
                            where c.Username.Equals(currentUser)
                            select c).SingleOrDefault();

                var post = (from c
                                     in await _postRepository.GetAllAsync()
                            where c.Title.Equals(postDTO.Title)
                            select c).SingleOrDefault();
                if (post != null)
                {
                    return BadRequest("There's already a post with that title");
                }

                post.Title = postDTO.Title;
                post.Body = postDTO.Body;
                post.Public = postDTO.Public;
                post.CreatedAt = DateTime.Now;
                post.UpdatedAt = DateTime.Now;
                post.Owner = user;

                await _postRepository.Add(post);

                return Ok(new { post });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("{id}/edit")]
        [Authorize(Roles = "Admin, Publisher, Editor")]
        public async Task<ActionResult<dynamic>> UpdatePost(int id, [FromBody] PostDTO postDTO)
        {
            var currentUser = User.Claims.FirstOrDefault(c => c.Type == "name").Value;
            var currentRole = User.Claims.FirstOrDefault(c => c.Type == "role").Value;
            try
            {
                var user = (from c
                                    in await _userRepository.GetAllAsync()
                            where c.Username.Equals(currentUser)
                            select c).SingleOrDefault();

                var post = await _postRepository.GetByIdAsync(id);

                if (post == null)
                {
                    return BadRequest("There's no post with this ID");
                }
                else if (currentRole == "Publisher" && post.Owner != user)
                {
                    return Unauthorized("You can't edit this post");
                } else
                {
                    post.Title = postDTO.Title;
                    post.Body = postDTO.Body;
                    post.Public = postDTO.Public;

                    await _postRepository.Update(post);

                    return Ok(new { post });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
