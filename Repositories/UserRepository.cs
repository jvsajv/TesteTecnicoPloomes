using Microsoft.EntityFrameworkCore;
using TesteTecnicoPloomes.Data;
using TesteTecnicoPloomes.Models;
using TesteTecnicoPloomes.Repositories.Interfaces;

namespace TesteTecnicoPloomes.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly DatabaseContext _databaseContext;
        public UserRepository(DatabaseContext databaseContext) {
            _databaseContext = databaseContext;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _databaseContext.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _databaseContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        }
        public async Task<User> Add(User user)
        {
            await _databaseContext.Users.AddAsync(user);
            await _databaseContext.SaveChangesAsync();

            return user;
        }
        public async Task<User> Update(User user)
        {
            try { 
                User userById = await GetByIdAsync(user.Id);
                userById.Username = user.Username;
                userById.Password = user.Password;
                userById.Role = user.Role;

                _databaseContext.Update(userById);
                await _databaseContext.SaveChangesAsync();

                return userById;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            
        }
    }
}
