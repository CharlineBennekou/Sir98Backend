namespace Sir98Backend.Models
{
    public class UserAwaitActivation
    {
        public string ActivationCode { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
