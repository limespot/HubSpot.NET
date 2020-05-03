using System;
using System.Net;

namespace HubSpot.NET.Core
{
    [Serializable]
    public class HubSpotException : Exception
    {
        public string RawJsonResponse { get; set; }

        public HubSpotError ReturnedError { get; set; }

        HttpStatusCode StatusCode { get; set; }

        public HubSpotException()
        {
        }

        public HubSpotException(string message) : base(message)
        {
        }

        public HubSpotException(string message, string jsonResponse) : this(message)
        {
            RawJsonResponse = jsonResponse;
        }

         public HubSpotException(string message, HubSpotError error) : this(message)
        {
            ReturnedError = error;
        }

       public HubSpotException(string message, HubSpotError error, string responseContent) : this(message, error)
        {
            RawJsonResponse = responseContent;
        }

        public override string Message => base.Message + $", Response = {(string.IsNullOrWhiteSpace(RawJsonResponse) ? ReturnedError.ToString() : RawJsonResponse)}";
    }
}
