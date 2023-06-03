using TesteTecnicoPloomes.Models;

namespace TesteTecnicoPloomes.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync(int skip=0, int take=-1);

        Task<User> GetByIdAsync(int id);
        Task<User> Add(User user);
        Task<User> Update(User user);

    }
}
