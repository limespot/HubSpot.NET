using System;
using System.Collections.Generic;
using System.Linq;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HubSpot.NET.Tests.Integration
{
	[TestClass]
	public class CompanyTests
	{
		private object apiCompany;

		[TestMethod]
		public void Create_SampleDetails_IdProeprtyIsSet()
		{
			// Arrange
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var sampleCompany = new CompanyHubSpotModel
			{
				Name = "New Created Company",
				Domain = "sampledomain.com"
			};

			// Act
			CompanyHubSpotModel company = companyApi.Create(sampleCompany);

			try
			{
				// Assert
				Assert.IsNotNull(company.Id, "The Id was not set and returned.");
				Assert.AreEqual(sampleCompany.Domain, company.Domain);
				Assert.AreEqual(sampleCompany.Name, company.Name);
			}
			finally
			{
				// Clean-up
				companyApi.Delete(company.Id.Value);
			}
		}

		[TestMethod]
		public void Update_SampleDetails_PropertiesAreUpdated()
		{
			// Arrange
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var sampleCompany = new CompanyHubSpotModel
			{
				Name = "New Updated Company",
				Domain = "sampledomain.com"
			};

			CompanyHubSpotModel company = companyApi.Create(sampleCompany);

			company.Domain = "sampledomain2.com";
			company.Name = "Updated Company #1";

			// Act
			companyApi.Update(company);

			try
			{
				// Assert
				Assert.AreNotEqual(sampleCompany.Domain, company.Domain);
				Assert.AreNotEqual(sampleCompany.Name, company.Name);
				Assert.AreEqual("sampledomain2.com", company.Domain);
				Assert.AreEqual("Updated Company #1", company.Name);

				// Second Act
				company = companyApi.GetById<CompanyHubSpotModel>(company.Id.Value);

				// Second Assert
				Assert.AreNotEqual(sampleCompany.Domain, company.Domain);
				Assert.AreNotEqual(sampleCompany.Name, company.Name);
				Assert.AreEqual("sampledomain2.com", company.Domain);
				Assert.AreEqual("Updated Company #1", company.Name);
			}
			finally
			{
				// Clean-up
				companyApi.Delete(company.Id.Value);
			}
		}

		[TestMethod]
		public void Delete_SampleCompany_CompanyIsDeleted()
		{
			// Arrange
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var sampleCompany = new CompanyHubSpotModel
			{
				Name = "New Deleted Company",
				Domain = "sampledomain.com"
			};

			CompanyHubSpotModel company = companyApi.Create(sampleCompany);

			// Act
			companyApi.Delete(company.Id.Value);

			// Assert
			company = companyApi.GetById<CompanyHubSpotModel>(company.Id.Value);
			Assert.IsNull(company, "The company was searchable and not deleted.");
		}

		[TestMethod]
		public void GetByDomain_5SamplesLimitedTo3WitContinuations_ReturnsCollectionWith3ItemsWithContinuationDetails()
		{
			// Arrange
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			IList<CompanyHubSpotModel> sampleCompanys = new List<CompanyHubSpotModel>();
			for (int i = 1; i <= 5; i++)
			{
				var company = new CompanyHubSpotModel()
				{
					Name = $"New Sample Company #{i:N0}",
					Domain = $"sample{i:N0}domain.com"
				};

				if (i % 2 == 1)
					company.Domain = "sampledomain.com";

				company = companyApi.Create(company);

				sampleCompanys.Add(company);
			}

			try
			{
				var searchOptions = new CompanySearchByDomain
				{
					Limit = 2
				};

				// Act
				CompanySearchResultModel<CompanyHubSpotModel> results = companyApi.GetByDomain<CompanyHubSpotModel>("sampledomain.com", searchOptions);

				// Assert
				Assert.IsTrue(results.MoreResultsAvailable, "Did not identify more results are available.");
				Assert.AreEqual(2, results.Results.Count, "Did not return 2 of the 5 results.");
				Assert.AreEqual(false, results.Results.Any(c => string.IsNullOrWhiteSpace(c.Name)), "Some companies do not have a name.");
				Assert.IsNotNull(results.ContinuationOffset);
				Assert.AreNotEqual(0, results.ContinuationOffset.CompanyId);

				// Second Act
				searchOptions.Offset = results.ContinuationOffset;
				var results2 = companyApi.GetByDomain<CompanyHubSpotModel>("sampledomain.com", searchOptions);

				Assert.IsFalse(results2.MoreResultsAvailable, "Did not identify at the end of results.");
				Assert.AreEqual(1, results2.Results.Count, "Did not return 1 of the 5 results.");
				Assert.AreEqual(false, results2.Results.Any(c => string.IsNullOrWhiteSpace(c.Name)), "Some companies do not have a name.");
			}
			finally
			{
				// Clean-up
				for (int i = 0; i < sampleCompanys.Count; i++)
				{
					companyApi.Delete(sampleCompanys[i].Id.Value);
				}
			}
		}

		[TestMethod]
		public void Search_5SamplesLimitedTo3WitContinuations_ReturnsCollectionWith3ItemsWithContinuationDetails()
		{
			// Arrange
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			IList<CompanyHubSpotModel> sampleCompanys = new List<CompanyHubSpotModel>();
			for (int i = 1; i <= 5; i++)
			{
				var company = new CompanyHubSpotModel()
				{
					Name = $"New Sample Company #{i:N0}",
					Domain = $"sample{i:N0}domain.com",
					Website = $"http://www.sample{i:N0}domain.com"
				};

				if (i % 2 == 0)
					company.Name = $"Something Else #{i:N0}";

				company = companyApi.Create(company);

				sampleCompanys.Add(company);
			}

			// HubSpot is rather slow to update... wait 5 seconds to allow it to catch up
			System.Threading.Thread.Sleep(5 * 1000);

			try
			{
				string searchValue = "New Sample Company";
				SearchRequestFilterOperatorType searchOperator = SearchRequestFilterOperatorType.ContainsAToken;
				var searchOptions = new SearchRequestOptions
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
					},
					Limit = 2
				};

				// Act
				CompanySearchHubSpotModel<CompanyHubSpotModel> results = companyApi.Search<CompanyHubSpotModel>(searchOptions);

				// Assert
				Assert.AreEqual(2, results.Results.Count, "Did not return 2 of the 5 results.");
				Assert.AreEqual(false, results.Results.Any(c => string.IsNullOrWhiteSpace(c.Name)), "Some companies do not have a name.");
				Assert.IsNotNull(results.Paging);
				Assert.IsNotNull(results.Paging.Next);
				Assert.IsFalse(string.IsNullOrWhiteSpace(results.Paging.Next.After), "Paging did not deserlise correctly");
				Assert.AreEqual("2", results.Paging.Next.After);

				// Second Act
				searchOptions.Offset = results.Paging.Next.After;
				var results2 = companyApi.Search<CompanyHubSpotModel>(searchOptions);

				Assert.AreEqual(1, results2.Results.Count, "Did not return 1 of the 5 results.");
				Assert.AreEqual(false, results2.Results.Any(c => string.IsNullOrWhiteSpace(c.Name)), "Some companies do not have a name.");
				Assert.IsNull(results2.Paging);
			}
			finally
			{
				// Clean-up
				for (int i = 0; i < sampleCompanys.Count; i++)
				{
					companyApi.Delete(sampleCompanys[i].Id.Value);
				}
			}
		}
	}
}