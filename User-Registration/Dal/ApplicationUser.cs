using Microsoft.AspNetCore.Identity;

namespace User_Registration.Dal
{
    public class ApplicationUser : IdentityUser
    {
        public string? ImageUrl { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public string? MaritalStatus { get; set; }
        public DateTime? Created_Date { get; set; }
        public DateTime? Password_Modified_Date { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
