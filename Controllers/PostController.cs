using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TesteTecnicoPloomes.DTO;
using TesteTecnicoPloomes.Models;
using TesteTecnicoPloomes.Repositories.Interfaces;

namespace TesteTecnicoPloomes.Controllers
{
    [Route("post")]
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
        public async Task<ActionResult<dynamic>> FetchAllPosts(int skip=0, int take=25)
        {
            try
            {
                return Ok(new { posts = await _postRepository.GetAllAsync(skip, take) });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("{idPost}")]
        public async Task<ActionResult<dynamic>> FetchById(int idPost)
        {
            try
            {
                var currentUser = User?.Identity?.Name;
                if (currentUser == null) {
                    currentUser = "";
                }
                var post = await _postRepository.GetByIdAsync(idPost);

                if (post == null) {
                    return BadRequest("There's no post with this ID");
                }
                else if (post.Public)
                {
                    return Ok(post); 
                }
                else if (currentUser != null )
                {
                    if(post.Owner.Username.Equals(currentUser)) { 
                        return Ok(post); 
                    }
                    
                }

                return Unauthorized(new
                    {
                        message = "You are not authorized to see this post.",
                        cause = "Not authorized role or Post is not public."
                    });
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("fetchPublic")]
        public async Task<ActionResult<dynamic>> FetchPublicPosts(int skip=0, int take=25)
        {
            try
            {
                return Ok(_postRepository.GetPublic(skip, take));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("fetchOwn")]
        [Authorize(Roles ="Admin, Publisher, Editor")]
        public async Task<ActionResult<dynamic>> FetchOwnPosts(int skip=0, int take=25)
        {
            var currentUser = User.Identity.Name;
            try
            {
                var user = (from c
                                     in await _userRepository.GetAllAsync()
                            where c.Username.Equals(currentUser)
                            select c).SingleOrDefault();

                return Ok(new { posts = _postRepository.GetOwn(user, skip, take) });
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
                var currentUser = User.Identity.Name;

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
                post = new Post();
                post.Title = postDTO.Title;
                post.Body = postDTO.Body;
                post.Public = postDTO.Public;
                post.CreatedAt = DateTime.Now;
                post.UpdatedAt = DateTime.Now;
                post.Owner = user;

                post = await _postRepository.Add(post);

                return Ok(post);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpDelete]
        [Route("{postId}/delete")]
        [Authorize(Roles = "Admin, Publisher, Editor")]
        public async Task<ActionResult<dynamic>> DeletePost(int postId)
        {
            var currentUser = User.Identity.Name;
            var isPublisher = User.IsInRole("Publisher");
            try
            {
                var user = (from c
                                    in await _userRepository.GetAllAsync()
                            where c.Username.Equals(currentUser)
                            select c).SingleOrDefault();

                var post = await _postRepository.GetByIdAsync(postId);

                if (post == null)
                {
                    return BadRequest("There's no post with this ID");
                }
                else if (isPublisher && post.Owner != user)
                {
                    return Unauthorized("You can't delet this post");
                } else
                {
                   

                    await _postRepository.DeleteByIdAsync(postId);

                    return Ok( new {message = "Post deleted succefully!"} );
                }
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
            var currentUser = User.Identity.Name;
            var isPublisher = User.IsInRole("Publisher");
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
                else if (isPublisher && post.Owner != user)
                {
                    return Unauthorized("You can't edit this post");
                }
                else
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
