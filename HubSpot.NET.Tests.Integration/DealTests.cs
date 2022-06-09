using System;
using System.Collections.Generic;
using System.Linq;

using HubSpot.NET.Api.Deal;
using HubSpot.NET.Api.Deal.Dto;
using HubSpot.NET.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HubSpot.NET.Tests.Integration
{
	[TestClass]
	public class DealTests
	{
		[TestMethod]
		public void Create_SampleDetails_IdProeprtyIsSet()
		{
			// Arrange
			var dealApi = new HubSpotDealApi(TestSetUp.Client);
			var sampleDeal = new DealHubSpotModel
			{
				Amount = 10000,
				Name = "New Created Deal",
				DealType = "newbusiness",
				Stage = TestSetUp.GetAppSetting("ClosedStageId") ?? "closedlost",
				DateCreated = DateTime.UtcNow
			};

			// Act
			DealHubSpotModel deal = dealApi.Create(sampleDeal);

			try
			{
				// Assert
				Assert.IsNotNull(deal.Id, "The Id was not set and returned.");
				Assert.AreEqual(sampleDeal.Amount, deal.Amount);
				Assert.AreEqual(sampleDeal.Name, deal.Name);
			}
			finally
			{
				// Clean-up
				dealApi.Delete(deal.Id.Value);
			}
		}

		[TestMethod]
		public void Update_SampleDetails_PropertiesAreUpdated()
		{
			// Arrange
			var dealApi = new HubSpotDealApi(TestSetUp.Client);
			var sampleDeal = new DealHubSpotModel
			{
				Amount = 10000,
				Name = "New Updated Deal",
				DealType = "newbusiness",
				Stage = TestSetUp.GetAppSetting("ClosedStageId") ?? "closedlost",
				DateCreated = DateTime.UtcNow
			};

			DealHubSpotModel deal = dealApi.Create(sampleDeal);

			deal.Amount = 20000;
			deal.Name = "Updated Deal #1";

			// Act
			dealApi.Update(deal);

			try
			{
				// Assert
				Assert.AreNotEqual(sampleDeal.Amount, deal.Amount);
				Assert.AreNotEqual(sampleDeal.Name, deal.Name);
				Assert.AreEqual(20000, deal.Amount);
				Assert.AreEqual("Updated Deal #1", deal.Name);

				// Second Act
				deal = dealApi.GetById<DealHubSpotModel>(deal.Id.Value);

				// Second Assert
				Assert.AreNotEqual(sampleDeal.Amount, deal.Amount);
				Assert.AreNotEqual(sampleDeal.Name, deal.Name);
				Assert.AreEqual(20000, deal.Amount);
				Assert.AreEqual("Updated Deal #1", deal.Name);
			}
			finally
			{
				// Clean-up
				dealApi.Delete(deal.Id.Value);
			}
		}

		[TestMethod]
		public void Delete_SampleDeal_ContactIsDeleted()
		{
			// Arrange
			var dealApi = new HubSpotDealApi(TestSetUp.Client);
			var sampleDeal = new DealHubSpotModel
			{
				Amount = 10000,
				Name = "New Deleted Deal",
				DealType = "newbusiness",
				Stage = TestSetUp.GetAppSetting("ClosedStageId") ?? "closedlost",
				DateCreated = DateTime.UtcNow
			};

			DealHubSpotModel deal = dealApi.Create(sampleDeal);

			// Act
			dealApi.Delete(deal.Id.Value);

			// Assert
			deal = dealApi.GetById<DealHubSpotModel>(deal.Id.Value);
			Assert.IsNull(deal, "The deal was searchable and not deleted.");
		}

		[TestMethod]
		public void List_5SamplesLimitedTo3WitContinuations_ReturnsCollectionWith3ItemsWithContinuationDetails()
		{
			// Arrange
			var dealApi = new HubSpotDealApi(TestSetUp.Client);
			IList<DealHubSpotModel> sampleDeals = new List<DealHubSpotModel>();
			for (int i = 1; i <= 5; i++)
			{
				var deal = dealApi.Create(new DealHubSpotModel()
				{
					Amount = 10000,
					Name = $"New Sample Deal #{i:N0}",
					DealType = "newbusiness",
					Stage = TestSetUp.GetAppSetting("ClosedStageId") ?? "closedlost",
					DateCreated = DateTime.UtcNow
				});
				sampleDeals.Add(deal);
			}

			try
			{
				var searchOptions = new ListRequestOptions
				{
					PropertiesToInclude = new List<string> { "dealname", "amount" },
					Limit = 3
				};

				// Act
				DealListHubSpotModel<DealHubSpotModel> results = dealApi.List<DealHubSpotModel>(false, searchOptions);

				// Assert
				Assert.IsTrue(results.MoreResultsAvailable, "Did not identify more results are available.");
				Assert.AreEqual(3, results.Deals.Count, "Did not return 3 of the 5 results.");
				Assert.AreEqual(false, results.Deals.Any(c => string.IsNullOrWhiteSpace(c.Name)), "Some deals do not have a name.");
				Assert.AreNotEqual(0, results.ContinuationOffset);

				// Second Act
				searchOptions.Offset = results.ContinuationOffset;
				var results2 = dealApi.List<DealHubSpotModel>(false, searchOptions);

				Assert.IsFalse(results2.MoreResultsAvailable, "Did not identify at the end of results.");
				Assert.AreEqual(2, results2.Deals.Count, "Did not return 2 of the 5 results.");
				Assert.AreEqual(false, results2.Deals.Any(c => string.IsNullOrWhiteSpace(c.Name)), "Some deals do not have a name.");
			}
			finally
			{
				// Clean-up
				for (int i = 0; i < sampleDeals.Count; i++)
				{
					dealApi.Delete(sampleDeals[i].Id.Value);
				}
			}
		}

		[TestMethod]
		public void RecentlyCreated_5SamplesLimitedTo3WitContinuations_ReturnsCollectionWith3ItemsWithContinuationDetails()
		{
			// Arrange
			var dealApi = new HubSpotDealApi(TestSetUp.Client);
			IList<DealHubSpotModel> sampleDeals = new List<DealHubSpotModel>();
			for (int i = 1; i <= 5; i++)
			{
				var deal = dealApi.Create(new DealHubSpotModel()
				{
					Amount = 10000,
					Name = $"New Created Deal #{i:N0}",
					DealType = "newbusiness",
					Stage = TestSetUp.GetAppSetting("ClosedStageId") ?? "closedlost",
					DateCreated = DateTime.UtcNow
				});
				sampleDeals.Add(deal);
			}

			// HubSpot is rather slow to update the list... wait 10 seconds to allow it to catch up
			System.Threading.Thread.Sleep(10 * 1000);

			try
			{
				/**
				 *  Get recently created deals since 7 days ago, limited to 10 records
				 *  Will default to 30 day if Since is not set.
				 *  Using DealRecentListHubSpotModel to accomodate deals returning in the "results" property.
				 */
				var currentdatetime = DateTime.SpecifyKind(DateTime.Now.AddDays(-7), DateTimeKind.Utc);
				var since = ((DateTimeOffset)currentdatetime).ToUnixTimeMilliseconds().ToString();

				var searchOptions = new DealRecentRequestOptions
				{
					Limit = 3,
					IncludePropertyVersion = false,
					Since = since
				};

				// Act
				DealRecentListHubSpotModel<DealHubSpotModel> results = dealApi.RecentlyCreated<DealHubSpotModel>(searchOptions);

				// Assert
				Assert.IsTrue(results.MoreResultsAvailable, "Did not identify more results are available.");
				Assert.AreEqual(3, results.Deals.Count, "Did not return 3 of the 5 results.");
				Assert.AreEqual(false, results.Deals.Any(c => string.IsNullOrWhiteSpace(c.Name)), "Some deals do not have a name.");
				Assert.AreNotEqual(0, results.ContinuationOffset);

				// Second Act
				searchOptions.Offset = results.ContinuationOffset;
				var results2 = dealApi.RecentlyCreated<DealHubSpotModel>(searchOptions);

				Assert.IsFalse(results2.MoreResultsAvailable, "Did not identify at the end of results.");
				Assert.AreEqual(2, results2.Deals.Count, "Did not return 2 of the 5 results.");
				Assert.AreEqual(false, results2.Deals.Any(c => string.IsNullOrWhiteSpace(c.Name)), "Some deals do not have a name.");
			}
			finally
			{
				// Clean-up
				for (int i = 0; i < sampleDeals.Count; i++)
				{
					dealApi.Delete(sampleDeals[i].Id.Value);
				}
			}
		}

		[TestMethod]
		public void RecentlyUpdated_3SamplesLimitedTo2WitContinuations_ReturnsCollectionWith2ItemsWithContinuationDetails()
		{
			// Arrange
			var dealApi = new HubSpotDealApi(TestSetUp.Client);
			IList<DealHubSpotModel> sampleDeals = new List<DealHubSpotModel>();
			for (int i = 1; i <= 5; i++)
			{
				var deal = dealApi.Create(new DealHubSpotModel()
				{
					Amount = 10000,
					Name = $"New Updated Deal #{i:N0}",
					DealType = "newbusiness",
					Stage = TestSetUp.GetAppSetting("ClosedStageId") ?? "closedlost",
					DateCreated = DateTime.UtcNow
				});
				sampleDeals.Add(deal);
			}

			for (int i = 0; i < sampleDeals.Count; i++)
			{
				DealHubSpotModel deal = sampleDeals[i];
				deal.Name = $"Updated Deal #{i:N0}";
				dealApi.Update(deal);
				// This is intentional to skip to every odd item
				i++;
			}

			// HubSpot is rather slow to update the list... wait 10 seconds to allow it to catch up
			System.Threading.Thread.Sleep(10 * 1000);

			try
			{
				/**
				 *  Get recently created deals since 7 days ago, limited to 10 records
				 *  Will default to 30 day if Since is not set.
				 *  Using DealRecentListHubSpotModel to accomodate deals returning in the "results" property.
				 */
				var currentdatetime = DateTime.SpecifyKind(DateTime.Now.AddDays(-7), DateTimeKind.Utc);
				var since = ((DateTimeOffset)currentdatetime).ToUnixTimeMilliseconds().ToString();

				var searchOptions = new DealRecentRequestOptions
				{
					Limit = 2,
					IncludePropertyVersion = false,
					Since = since
				};

				// Act
				DealRecentListHubSpotModel<DealHubSpotModel> results = dealApi.RecentlyUpdated<DealHubSpotModel>(searchOptions);

				// Assert
				Assert.IsTrue(results.MoreResultsAvailable, "Did not identify more results are available.");
				Assert.AreEqual(2, results.Deals.Count, "Did not return 3 of the 5 results.");
				Assert.AreEqual(false, results.Deals.Any(c => string.IsNullOrWhiteSpace(c.Name)), "Some deals do not have a name.");
				Assert.AreNotEqual(0, results.ContinuationOffset);

				// Cannot actually test recently updated as recently created polutes the results.
			}
			finally
			{
				// Clean-up
				for (int i = 0; i < sampleDeals.Count; i++)
				{
					dealApi.Delete(sampleDeals[i].Id.Value);
				}
			}
		}
	}
}