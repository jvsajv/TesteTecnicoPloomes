using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace TesteTecnicoPloomes.Models
{
    public class Post
    {
        [Required]
        [DataMember(Name = "id")]
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        [Key, ForeignKey("Owner")]
        public int IdOwner { get; set; }
        public User Owner { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Public { get; set; }
    }
}
