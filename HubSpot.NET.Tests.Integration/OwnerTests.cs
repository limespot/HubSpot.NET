using System.Linq;

using HubSpot.NET.Api.Owner;
using HubSpot.NET.Api.Owner.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HubSpot.NET.Tests.Integration
{
	[TestClass]
	public class OwnerTests
	{
		// [TestMethod]
		public void GetAll_NoOptions_ReturnsNonZeroCollection()
		{
			// Arrange
			var ownerApi = new HubSpotOwnerApi(TestSetUp.Client);

			// Act
			OwnerListHubSpotModel<OwnerHubSpotModel> results = ownerApi.GetAll<OwnerHubSpotModel>();

			// Assert
			Assert.IsTrue(results.Count > 0, "Did not return any results");
			Assert.AreEqual(false, results.Any(o => string.IsNullOrWhiteSpace(o.Email)), "Some owners do not have email addresses");
		}

		[TestMethod]
		public void GetAll_IncludeInactive_ReturnsNonZeroCollection()
		{
			// Arrange
			var ownerApi = new HubSpotOwnerApi(TestSetUp.Client);

			// Act
			OwnerListHubSpotModel<OwnerHubSpotModel> results = ownerApi.GetAll<OwnerHubSpotModel>(new OwnerGetAllRequestOptions { IncludeInactive = true });

			// Assert
			Assert.IsTrue(results.Count > 0, "Did not return any results");
			Assert.AreEqual(false, results.Any(o => string.IsNullOrWhiteSpace(o.Email)), "Some owners do not have email addresses");
		}

		// [TestMethod]
		public void GetAll_KnownEmailAddress_ReturnsNonZeroCollection()
		{
			// Arrange
			var ownerApi = new HubSpotOwnerApi(TestSetUp.Client);

			// Act
			OwnerListHubSpotModel<OwnerHubSpotModel> results = ownerApi.GetAll<OwnerHubSpotModel>(new OwnerGetAllRequestOptions { EmailAddress = "bob@squaredup.com" });

			// Assert
			Assert.IsTrue(results.Count > 0, "Did not return any results");
			Assert.AreEqual(false, results.Any(o => string.IsNullOrWhiteSpace(o.Email)), "Some owners do not have email addresses");
		}
	}
}
