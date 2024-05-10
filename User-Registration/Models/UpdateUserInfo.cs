namespace User_Registration.Models
{
    public class UpdateUserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public IFormFile Image { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
