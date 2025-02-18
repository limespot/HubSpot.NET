﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Attributes;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.Deal.Dto
{

    /// <summary>
    /// Models a Deal entity within HubSpot. Default properties are included here
    /// with the intention that you'd extend this class with properties specific to 
    /// your HubSpot account.
    /// </summary>
    [DataContract]
    public class DealHubSpotModel : IHubSpotModel
    {
        public DealHubSpotModel()
        {
            Associations =  new DealHubSpotAssociations();
        }
        /// <summary>
        /// Contacts unique Id in HubSpot
        /// </summary>
        [DataMember(Name = "dealId")]
        [IgnoreDataMember]
        public long? Id { get; set; }

        [DataMember(Name = "dealname")]
        public string Name { get; set; }

        [DataMember(Name = "dealstage")]
        public string Stage { get; set; }

        [DataMember(Name = "pipeline")]
        public string Pipeline { get; set; }

        [DataMember(Name = "hubspot_owner_id")]
        public long? OwnerId { get; set; }

        [DataMember(Name = "closedate")]
        [LongDate]
        public DateTime? CloseDate { get; set; }

        [DataMember(Name = "amount")]
        public double? Amount { get; set; }

        [DataMember(Name = "dealtype")]
        public string DealType { get; set; }

        [DataMember(Name = "createdate")]
        [LongDate]
        [IgnoreDataMember]
        public DateTime? DateCreated { get; set; }

        [DataMember(Name = "isDeleted")]
        [LongDate]
        [IgnoreDataMember]
        public bool? IsDeleted { get; set; }

        [IgnoreDataMember]
        public DealHubSpotAssociations Associations { get; }

        public string RouteBasePath => "/deals/v1";
        public bool IsNameValue => true;

        public virtual void ToHubSpotDataEntity(ref dynamic converted)
        {
            converted.Associations = Associations;
        }

        public virtual void FromHubSpotDataEntity(dynamic hubspotData)
        {
            if (hubspotData.associations != null)
            {
                Associations.AssociatedContacts = ((List<object>)hubspotData.associations.associatedVids).Cast<long>().ToArray();
                Associations.AssociatedCompany = ((List<object>) hubspotData.associations.associatedCompanyIds).Cast<long>().ToArray();
            }
        }
    }
}
