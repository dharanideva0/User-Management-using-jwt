using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace User_Registration.Models
{
    public class CreateUser
    {
        public string? Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string MaritalStatus { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
    }
}
