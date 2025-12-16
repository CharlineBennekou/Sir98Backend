namespace Sir98Backend.Interface
{
    public interface IPushSubscriptionService
    {
        /// <summary>
        /// Insert or update a push subscription for the given user.
        /// </summary>
        Task UpsertAsync(string userEmail, string endpoint, string p256dh, string auth);

        /// <summary>
        /// Remove a push subscription for the given user and endpoint.
        /// </summary>
        Task RemoveAsync(string userEmail, string endpoint);
    }
}
