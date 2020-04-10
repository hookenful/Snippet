using Snippet.Configuration;
using Snippet.Logging;
using Snippet.Logging.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Snippet.SocialServices.ApiClients.WebAnalytics.GoogleAnalytics
{
    public class GoogleMetricClient
    {
        private static readonly ILogger Logger = LogManager.GetLogger(nameof(GoogleMetricClient));
        private readonly string _googleAnalyticsServiceUrl;
        private readonly string _version;
        private readonly string _trackingId;

        public GoogleMetricClient(IConfigurationManager configurationManager)
        {
            _googleAnalyticsServiceUrl = configurationManager.GetSetting("GoogleAnalyticsUrl");
            _version = configurationManager.GetSetting("GoogleAnalyticsApiVersion");
            _trackingId = configurationManager.GetSetting("GoogleAnalyticsTrackingId");
        }

        public async Task Track(string clientId, string category, string action, string label, string countryCode = null)
        {
            var payload = new Dictionary<string, string>
            {
                ["t"] = "event",
                ["v"] = _version,
                ["tid"] = _trackingId,
                ["cid"] = clientId ?? Guid.NewGuid().ToString(),
                ["ec"] = category,
                ["ea"] = action,
                ["el"] = label,
                ["geoid"] = countryCode
            };
            using (var client = GetClient())
            {
                var response = await client
                    .PostAsync(_googleAnalyticsServiceUrl, new FormUrlEncodedContent(payload));
                Logger.Info("Track: {statusCode} {clientId} {category} {action} {label}", response.StatusCode,
                    clientId, category, action, label);
            }
        }

        private static HttpClient GetClient()
        {
            return new HttpClient(new RequestResponseLoggerDelegatingHandler(new HttpClientHandler()));
        }
    }

}

