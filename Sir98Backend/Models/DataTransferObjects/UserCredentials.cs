namespace Sir98Backend.Models.DataTransferObjects
{
    /// <summary>
    /// Dto for user credentials.
    /// </summary>
    public class UserCredentials
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
