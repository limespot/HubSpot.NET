﻿using HubSpot.NET.Core;

namespace HubSpot.NET.Examples
{
    public class Examples
    {
        static void Main(string[] args)
        {
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
