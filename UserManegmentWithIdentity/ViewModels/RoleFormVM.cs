using System.ComponentModel.DataAnnotations;

namespace UserManegmentWithIdentity.ViewModels
{
    public class RoleFormVM
    {
        [Required,MaxLength(256)]
        public string Name { get; set; }
    }
}
