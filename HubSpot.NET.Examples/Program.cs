using HubSpot.NET.Core;

namespace HubSpot.NET.Examples
{
    public class Examples
    {
        static void Main(string[] args)
        {
            var _ = new Core.Requests.RequestSerializer(new Core.Requests.RequestDataConverter());
            var x = _.DeserializeListEntity<Api.SearchHubSpotModel<Api.Deal.Dto.DealHubSpotModel>>("{\"total\":1,\"results\":[{\"id\":\"7684897854\",\"properties\":{\"amount\":\"10\",\"closedate\":null,\"companyid\":\"3486b5ea9701412bb9a42f1850051d3e\",\"createdate\":\"2022-01-24T17:42:39.871Z\",\"date_of_last_activity\":\"2022-01-24\",\"dealId\":null,\"dealname\":\"Katlego Cathy Makiwa\",\"dealstage\":\"16575788\",\"dealtype\":\"newbusiness\",\"hs_lastmodifieddate\":\"2022-01-25T00:46:27.550Z\",\"hs_object_id\":\"7684897854\",\"id\":null,\"pipeline\":\"default\"},\"createdAt\":\"2022-01-24T17:42:39.871Z\",\"updatedAt\":\"2022-01-25T00:46:27.550Z\",\"archived\":false}]}");

            /**
             * Initialize the API with your API Key
             * You can find or generate this under Integrations -> HubSpot API key
             */
            var api = new HubSpotApi(System.Configuration.ConfigurationManager.AppSettings["ApiKey"]);

            Deals.Example(api);

            Companies.Example(api);

            Contacts.Example(api);

            CompanyProperties.Example(api);

            EmailSubscriptions.Example(api);
        }
    }
}
