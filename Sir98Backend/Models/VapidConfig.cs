namespace Sir98Backend.Models
{
    public class VapidConfig
    {
        //Normally, you would not hardcode these values but load them from a secure configuration source..
        public string Subject { get; set; } = "mailto:you@example.com"; // contact email
        public string PublicKey { get; set; } = "YOUR_PUBLIC_VAPID_KEY";
        public string PrivateKey { get; set; } = "YOUR_PRIVATE_VAPID_KEY";
    }
}
