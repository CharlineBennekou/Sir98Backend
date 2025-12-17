// Sir98Backend/Interface/IPushSubscriptionService.cs
namespace Sir98Backend.Interfaces
{
    public interface IPushSubscriptionService
    {
        Task UpsertAsync(string userEmail, string endpoint, string p256dh, string auth);
        Task RemoveAsync(string userEmail, string endpoint);

        // Dev/test helper
        Task<PushAllResult> PushAllAsync(string title, string body, string url);

        Task ApplySendResultAsync(PushSendResult result);

    }

    public sealed record PushAllResult(int TotalAttempted, int Failed, int RemovedExpired);

}
