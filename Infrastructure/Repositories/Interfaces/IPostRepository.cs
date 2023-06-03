using TesteTecnicoPloomes.Models;

namespace TesteTecnicoPloomes.Infrastructure.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task<List<Post>> GetAllAsync(int skip=0, int take=-1);
        public List<Post> GetPublic(int skip, int take);
        List<Post> GetOwn(User user, int skip, int take);
        Task<Post> GetByIdAsync(int id);
        Task<Post> Add(Post post);
        Task<Post> Update(Post post);
        Task<bool> DeleteByIdAsync(int id);
    }
}
