using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace User_Registration.Models
{
    public class UserProfile
    {
        public string? Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string MaritalStatus { get; set; }
        public string Image { get; set; }
    }
}
