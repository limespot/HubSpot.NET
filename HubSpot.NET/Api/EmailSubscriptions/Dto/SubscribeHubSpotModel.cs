using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.EmailSubscriptions.Dto
{
    public class SubscribeHubSpotModel : IHubSpotModel
    {
        [DataMember(Name = "portalSubscriptionLegalBasis")]
        public string SubscriptionLegalBasis { get;set; }

        [DataMember(Name = "portalSubscriptionLegalBasisExplanation")]
        public string SubscriptionLegalBasisExplanation { get; set; }

        [DataMember(Name = "subscriptionStatuses")]
        public List<SubscribeStatusHubSpotModel> SubscriptionStatuses { get; set; }

        [IgnoreDataMember]
        public bool IsNameValue { get; }

        public void ToHubSpotDataEntity(ref dynamic dataEntity)
        {
        }

        public void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }

        [IgnoreDataMember]
        public string RouteBasePath => "/email/public/v1/subscriptions";
    }

}
