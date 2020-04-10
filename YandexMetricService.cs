using DataAbstraction.Services;
using Solomoto.Logging;
using Solomoto.SocialServices.ApiClients.WebAnalytics.YandexMetric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solomoto.BusinessLayer.Services
{
    public class YandexMetricService : IProviderMetricService
    {
        private readonly YandexMetricClient _client;
        private static readonly ILogger Logger = LogManager.GetLogger(nameof(YandexMetricService));
        private const string YandexCookieName = "_ym_uid";
        public YandexMetricService(YandexMetricClient client)
        {
            _client = client;
        }

        public async Task TrackPayment(string clientId, bool first, string method, string countryCode)
        {
            try
            {
                await _client.Track(clientId, "user_payed");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception: YandexMetricService.TrackPayment: Message: {0}; StackTrace: {1}",
                    ex.Message, ex.StackTrace);
            }
        }

        public async Task TrackUserRegistrationDoneFromSocialNetwork(string clientId, string socialNetwork, string countryCode)
        {
            try
            {
                await _client.Track(clientId, "user_register_done_with_sn");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception: YandexMetricService.TrackUserRegistrationDoneFromSocialNetwork: Message: {0}; StackTrace: {1}",
                    ex.Message, ex.StackTrace);
            }
        }

        public string GetMetricUserIdFromCookie(List<Tuple<string, string, string>> cookieCollection)
        {
            var cookieWithMinLength = cookieCollection.Where(t => t.Item1 == YandexCookieName).OrderBy(t => (t.Item3 ?? string.Empty).Length).FirstOrDefault();
            return cookieWithMinLength?.Item2;
        }
    }
}
