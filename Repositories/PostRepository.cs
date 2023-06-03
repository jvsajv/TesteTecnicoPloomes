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
        public async Task<List<Post>> GetAllAsync(int skip=0, int take=-1)
        {
            if(take < 0){
                return await _databaseContext.Posts.Skip(skip).ToListAsync();
            }
            return await _databaseContext.Posts.OrderBy(post => post.Id).Skip(skip).Take(take).ToListAsync();
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
        public List<Post> GetPublic(int skip, int take)
        {
            return _databaseContext.Posts.Where(post => post.Public).OrderBy(post => post.Id).Skip(skip).Take(take).ToList();
        }
        public List<Post> GetOwn(User user, int skip, int take)
        {
            return _databaseContext.Posts.Where(post => post.Owner == user).OrderBy(post => post.Id).Skip(skip).Take(take).ToList();
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
