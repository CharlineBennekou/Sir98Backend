namespace Sir98Backend.Models.DataTransferObjects
{
    public class RegisterAccount
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PasswordRepeated { get; set; }
    }
}
