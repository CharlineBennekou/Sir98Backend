// Sir98Backend/Interface/IPushSender.cs
using Sir98Backend.Models;

namespace Sir98Backend.Interfaces
{
    public interface IPushSender
    {
        Task<PushSendResult> SendAsync(
            IReadOnlyCollection<PushSubscription> subscriptions,
            PushNotificationPayload payload);
    }

    public sealed record PushNotificationPayload(
        string Title,
        string Body,
        string Url);

    public sealed record PushSendResult(
        int TotalAttempted,
        int Succeeded,
        int Failed,
        IReadOnlyCollection<string> SucceededEndpoints,
        IReadOnlyCollection<string> ExpiredEndpoints,
        IReadOnlyCollection<PushSendFailure> Failures);

    public sealed record PushSendFailure(
        string Endpoint,
        string Reason,
        int? StatusCode);
}
