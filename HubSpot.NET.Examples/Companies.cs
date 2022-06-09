using System;
using System.Collections.Generic;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core;

namespace HubSpot.NET.Examples
{
    public class Companies
    {
        public static void Example(HubSpotApi api)
        {
            /**
             * Create a company
             */
            var company = api.Company.Create(new CompanyHubSpotModel()
            {
                Domain = "squaredup.com",
                Name = "Squared Up"
            });

            /**
             * Update a company's property
             */
            company.Description = "Data Visualization for Enterprise IT";
            api.Company.Update(company);

            /**
             * Get all companies with domain name "squaredup.com"
             */
            var companies = api.Company.GetByDomain<CompanyHubSpotModel>("squaredup.com", new CompanySearchByDomain()
            {
                Limit = 10
            });

            /**
             * Search for a company
             */
            string searchValue = company.Name;
            SearchRequestFilterOperatorType searchOperator = SearchRequestFilterOperatorType.EqualTo;
            var searchedCompany = api.Company.Search<CompanyHubSpotModel>(new SearchRequestOptions()
            {
                FilterGroups = new List<SearchRequestFilterGroup>
                {
                    new SearchRequestFilterGroup
                    {
                        Filters = new List<SearchRequestFilter>
                        {
                            new SearchRequestFilter
                            {
                                PropertyName = "name",
                                Operator = searchOperator,
                                Value = searchValue
                            }
                        }
                    }
                },
                PropertiesToInclude = new List<string>
                {
                    "domain", "name", "website"
                }
            });
            if (searchedCompany.Total < 1)
                throw new InvalidOperationException("No companies found.");
            foreach (var d in searchedCompany.Results)
            {
                if (d.Id == null)
                    throw new InvalidOperationException("The found company does not have an ID set");
                if (d.Name != company.Name)
                    throw new InvalidOperationException("The found company does not have the same name");
            }

            /**
             * Get Associations
             */
            company = api.Company.GetAssociations(company);
           
            /**
             * Delete a contact
             */
            api.Company.Delete(company.Id.Value);

            /*
             * Create several companies and test searching
             */
            IList<CompanyHubSpotModel> sampleCompanies = new List<CompanyHubSpotModel>();
            for (int i = 1; i <= 22; i++)
            {
                company = api.Company.Create(new CompanyHubSpotModel()
                {
                    Domain = "squaredup.com",
                    Name = $"Squared Up {i:N0}"
                });
                sampleCompanies.Add(company);
            }
            searchValue = "Squared Up";
            searchOperator = SearchRequestFilterOperatorType.ContainsAToken;
            var searchedCompanies = api.Company.Search<CompanyHubSpotModel>(new SearchRequestOptions()
            {
                FilterGroups = new List<SearchRequestFilterGroup>
                {
                    new SearchRequestFilterGroup
                    {
                        Filters = new List<SearchRequestFilter>
                        {
                            new SearchRequestFilter
                            {
                                PropertyName = "name",
                                Operator = searchOperator,
                                Value = searchValue
                            }
                        }
                    }
                },
                PropertiesToInclude = new List<string>
                {
                    "domain", "name", "website"
                }
            });
            if (searchedCompanies.Total < 1)
                throw new InvalidOperationException("No companies found.");
            if (searchedCompanies.Total != 22)
                throw new InvalidOperationException($"'{searchedCompanies.Total:N0}' companies found when we expected 22.");
            if (searchedCompanies.Paging == null || searchedCompanies.Paging.Next == null || string.IsNullOrWhiteSpace(searchedCompanies.Paging.Next.After))
                throw new InvalidOperationException("Paging did not deserlise correctly.");
            if (searchedCompanies.Paging.Next.After != "20")
                throw new InvalidOperationException($"'{searchedCompanies.Paging.Next.After}' as a value for Paging.Next.After was not the expted 20.");
            for (int i = 0; i < sampleCompanies.Count; i++)
            {
                api.Company.Delete(sampleCompanies[i].Id.Value);
            }
        }
    }
}
