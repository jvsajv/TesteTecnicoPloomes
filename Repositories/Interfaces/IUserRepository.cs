using TesteTecnicoPloomes.Models;

namespace TesteTecnicoPloomes.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();

        Task<User> GetByIdAsync(int id);
        Task<User> Add(User user);
        Task<User> Update(User user);

    }
}
