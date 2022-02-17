using System.Collections.Generic;
using Flurl;
using HubSpot.NET.Api.ContactList.Dto;
using HubSpot.NET.Core.Interfaces;
using RestSharp;

namespace HubSpot.NET.Api.ContactList
{
    public class HubSpotContactListApi : IHubSpotContactListApi
    {
        private readonly IHubSpotClient _client;

        public HubSpotContactListApi(IHubSpotClient client)
        {
            _client = client;
        }

        public ListContactListModel GetContactLists(ListOptions opts = null)
        {
            if (opts == null)
            {
                opts = new ListOptions();
            }
            
            var path = $"{new ListContactListModel().RouteBasePath}".SetQueryParam("count", opts.Limit);
            if (opts.Offset.HasValue)
            {
                path = path.SetQueryParam("offset", opts.Offset);
            }
            
            var data = _client.ExecuteList<ListContactListModel>(path, convertToPropertiesSchema: false);

            return data;
        }

        public ListContactListModel GetStaticContactLists(ListOptions opts = null)
        {
            if (opts == null)
            {
                opts = new ListOptions();
            }
            
            var path = $"{new ListContactListModel().RouteBasePath}/static".SetQueryParam("count", opts.Limit);
            if (opts.Offset.HasValue)
            {
                path = path.SetQueryParam("offset", opts.Offset);
            }
            
            var data = _client.ExecuteList<ListContactListModel>(path, convertToPropertiesSchema: false);

            return data;
        }

        public ContactListModel GetContactListById(long contactListId)
        {
            var path = $"{new ContactListModel().RouteBasePath}/{contactListId}";

            var data = _client.ExecuteList<ContactListModel>(path, convertToPropertiesSchema: false);

            return data;
        }

        public ContactListUpdateResponseModel AddContactsToList(long listId, IEnumerable<long> contactIds)
        {
            var model = new ContactListUpdateModel();
            var path = $"{model.RouteBasePath}/{listId}/add";
            model.ContactIds.AddRange(contactIds);
            var data = _client.Execute<ContactListUpdateResponseModel>(path, model, Method.POST, convertToPropertiesSchema: false);

            return data;
        }

        public ContactListUpdateResponseModel RemoveContactsFromList(long listId, IEnumerable<long> contactIds)
        {
            var model = new ContactListUpdateModel();
            var path = $"{model.RouteBasePath}/{listId}/remove";
            model.ContactIds.AddRange(contactIds);
            var data = _client.Execute<ContactListUpdateResponseModel>(path, model, Method.POST, convertToPropertiesSchema: false);

            return data;
        }
    }
}