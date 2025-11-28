namespace Sir98Backend.Models.DataTransferObjects
{
    public class PushSubscriptionDto
    {
        public string Endpoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }


    }
}
