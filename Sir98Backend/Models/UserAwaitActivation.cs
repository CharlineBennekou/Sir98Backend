namespace Sir98Backend.Models
{
    public class UserAwaitActivation
    {
        public required string ActivationCode { get; set; }
        public required string Email { get; set; }
        public required string HashedPassword { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
