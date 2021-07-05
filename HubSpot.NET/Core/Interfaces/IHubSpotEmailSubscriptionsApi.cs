using HubSpot.NET.Api.EmailSubscriptions.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotEmailSubscriptionsApi
    {
        /// <summary>
        /// Gets the available email subscription types available in the portal
        /// </summary>
        SubscriptionTypeListHubSpotModel GetEmailSubscriptionTypes();
        /// <summary>
        /// Get subscription status for the given email address
        /// </summary>
        /// <param name="email"></param>
        SubscriptionStatusHubSpotModel GetStatus(string email);
        /// <summary>
        /// Unsubscribe the given email address from ALL email
        /// WARNING: There is no UNDO for this operation
        /// </summary>
        /// <param name="email"></param>
        void UnsubscribeAll(string email);
        /// <summary>
        /// Unsubscribe the given email address from the given subscription type
        /// WARNING: There is no UNDO for this operation
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id">The ID of the subscription type</param>
        void UnsubscribeFrom(string email, long id);

        /// <summary>
        /// Subscribe the given email address to the given subscription type
        /// See <see cref="https://legacydocs.hubspot.com/docs/methods/email/update_status"/>
        /// </summary>
        void SubscribeTo(string email, long id, string basis, string basisExplaination);
    }
}