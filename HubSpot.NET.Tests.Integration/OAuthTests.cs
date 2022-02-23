using HubSpot.NET.Core;
using HubSpot.NET.Core.OAuth;
using HubSpot.NET.Core.OAuth.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HubSpot.NET.Tests.Integration
{
	[TestClass]
	public class OAuthTests
	{
//		[TestMethod]
		public void Authorize()
		{
			// Arrange
			string clientId = TestSetUp.GetAppSetting("ClientId");
			string clientSecret = TestSetUp.GetAppSetting("ClientSecret");
			string appId = TestSetUp.GetAppSetting("AppId");
			string redirectUri = TestSetUp.GetAppSetting("RedirectUri");
			string authCode = TestSetUp.GetAppSetting("AuthCode");

			var oAuthApi = new HubSpotOAuthApi(HubSpotBaseClient.BaseUrl, clientId, clientSecret);

			// Act
			HubSpotToken token = oAuthApi.Authorize(authCode, redirectUri);

			// Assert
			Assert.IsFalse(string.IsNullOrWhiteSpace(token.RefreshToken), "No refresh token returned");
			Assert.IsFalse(string.IsNullOrWhiteSpace(token.AccessToken), "No access token returned");
		}
	}
}
