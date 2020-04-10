using DataAbstraction.Services;
using Snippet.Logging;
using Snippet.SocialServices.ApiClients.WebAnalytics.GoogleAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Snippet.BusinessLayer.Services
{
    public class GoogleMetricService : IProviderMetricService
    {
        private readonly GoogleMetricClient _client;
        private static readonly ILogger Logger = LogManager.GetLogger(nameof(GoogleMetricService));
        private const string GoogleCookieName = "_gid";


        public GoogleMetricService(GoogleMetricClient client)
        {
            _client = client;
        }

        public async Task TrackPayment(string clientId, bool first, string method, string countryCode)
        {
            try
            {
                await _client.Track(clientId, "payment", first ? "first_payed" : "notfirst_payed", method, countryCode);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception: GoogleMetricService.TrackPayment: Message: {0}; StackTrace: {1}",
                    ex.Message, ex.StackTrace);

            }
        }

        public async Task TrackUserRegistrationDoneFromSocialNetwork(string clientId, string socialNetwork, string countryCode)
        {
            try
            {
                await _client.Track(clientId, "registration", "user_register_sn_done", socialNetwork);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception: GoogleMetricService.TrackUserRegistrationDoneFromSocialNetwork: Message: {0}; StackTrace: {1}",
                    ex.Message, ex.StackTrace);
            }
        }
        public string GetMetricUserIdFromCookie(List<Tuple<string, string, string>> cookieCollection)
        {
            var cookieWithMinLength = cookieCollection.Where(t => t.Item1 == GoogleCookieName).OrderBy(t => (t.Item3 ?? string.Empty).Length).FirstOrDefault();

            return cookieWithMinLength?.Item2.Split('.')[2];

        }

    }
}
