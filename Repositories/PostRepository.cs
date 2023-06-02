using Microsoft.EntityFrameworkCore;
using TesteTecnicoPloomes.Data;
using TesteTecnicoPloomes.Models;
using TesteTecnicoPloomes.Repositories.Interfaces;

namespace TesteTecnicoPloomes.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DatabaseContext _databaseContext;
        public PostRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public async Task<List<Post>> GetAllAsync()
        {
            return await _databaseContext.Posts.ToListAsync();
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            try
            {
                return await _databaseContext.Posts.FirstOrDefaultAsync(post => post.Id == id);
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<Post> GetPublic()
        {
            return _databaseContext.Posts.Where(post => post.Public).ToList();
        }
        public List<Post> GetOwn(User user)
        {
            return _databaseContext.Posts.Where(post => post.Owner == user).ToList();
        }
      
        public async Task<Post> Add(Post post)
        {
            await _databaseContext.Posts.AddAsync(post);
            await _databaseContext.SaveChangesAsync();

            return post;
        }

        public async Task<Post> Update(Post post)
        {
            try
            {
                Post postById = await GetByIdAsync(post.Id);
                postById.Title = post.Title;
                postById.Body = post.Body;
                postById.Public = post.Public;
                postById.UpdatedAt = new DateTime();

                _databaseContext.Update(postById);
                await _databaseContext.SaveChangesAsync();

                return postById;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            try
            {
                Post postById = await GetByIdAsync(id);

                _databaseContext.Remove(postById);
                await _databaseContext.SaveChangesAsync();

                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
