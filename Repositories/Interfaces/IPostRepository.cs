using TesteTecnicoPloomes.Models;

namespace TesteTecnicoPloomes.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task<List<Post>> GetAllAsync();
        public List<Post> GetPublic();
        List<Post> GetOwn(User user);
        Task<Post> GetByIdAsync(int id);
        Task<Post> Add(Post post);
        Task<Post> Update(Post post);
        Task<bool> DeleteByIdAsync(int id);
    }
}
