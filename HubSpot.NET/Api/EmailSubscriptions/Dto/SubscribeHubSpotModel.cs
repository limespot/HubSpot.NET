using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
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

        public bool IsNameValue { get; }

        public void ToHubSpotDataEntity(ref dynamic dataEntity)
        {
        }

        public void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }

        public string RouteBasePath => "/email/public/v1/subscriptions";
    }
}
