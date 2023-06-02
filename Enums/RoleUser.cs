using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TesteTecnicoPloomes.Enums
{
    public enum RoleUser
    {
        [Display(Name = "ADMIN")]
        Admin = 0,
        [Display(Name = "Editor")]
        Editor=1,
        [Display(Name = "Publisher")]
        Publisher =2,
        [Display(Name = "Viwer")]
        Viewer=3
    }   
}
