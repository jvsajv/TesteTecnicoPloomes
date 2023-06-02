using System.ComponentModel.DataAnnotations;
using TesteTecnicoPloomes.Enums;

namespace TesteTecnicoPloomes.Models
{
    public class User
    {
        [Key, Required]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public RoleUser Role { get; set; }

        public ICollection<Post> Posts { get; }

    }
}
