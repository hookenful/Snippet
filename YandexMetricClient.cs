using System;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Snippet.CommonHelper.Extensions;
using Snippet.CommonHelper.Http;
using Snippet.Configuration;
using Snippet.Infrastructure.RestSharp;


namespace Snippet.SocialServices.ApiClients.WebAnalytics.YandexMetric
{
   public class YandexMetricClient
    {
        private readonly string _yandexMetricServiceUrl;
        private readonly IRequestCheckerExecutionStrategy _executionStrategy;
        private readonly string _counterId;
        private readonly string _accessToken;
        private const string CsvHeaders = "ClientId, Target, DateTime";

        public YandexMetricClient(IConfigurationManager configurationManager, IRequestCheckerExecutionStrategy executionStrategy)
        {
            _executionStrategy = executionStrategy;
            _yandexMetricServiceUrl = configurationManager.GetSetting("YandexMetricUrl");
            _counterId = configurationManager.GetSetting("YandexMetricCounterId");
            _accessToken = configurationManager.GetSetting("YandexMetricAccessToken");
        }
        public async Task Track(string clientId, string target)
        {
            var request =
                new RestRequestBuilder(
                        $"/management/v1/counter/{_counterId}/offline_conversions/upload?client_id_type=CLIENT_ID")
                    .Method(Method.POST)
                    .Build();
            request.AddFileBytes("file", Encoding.UTF8.GetBytes(GetCsvData(clientId, target)), "data.csv", "text/csv");
            request.AddHeader("Authorization", $"Bearer {_accessToken}");

                await _executionStrategy.ExecuteAsync(new Uri(_yandexMetricServiceUrl), request);
        }

        private static string GetCsvData(string clientId, string target)
        {
            var sb = new StringBuilder();
            sb.AppendLine(CsvHeaders);
            sb.AppendLine(string.Join(",", clientId, target, DateTime.Now.ToUnixTime()));
            return sb.ToString();
        }
    }
}
