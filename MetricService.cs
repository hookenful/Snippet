using Autofac.Features.Indexed;
using DataAbstraction.Repositories;
using DataAbstraction.Services;
using Snippet.DomainModel;
using Snippet.DomainModel.Enums;
using Snippet.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snippet.BusinessLayer.Services
{
    public class MetricService : IMetricService
    {
        private readonly IAnalyticUserDataRepository _analyticUserDataRepository;
        private readonly IIndex<AnalyticProviders, IProviderMetricService> _metricsServicesFactory;
        private static readonly ILogger Logger = LogManager.GetLogger(nameof(MetricService));
        private static readonly AnalyticProviders[] MetricProviders = { AnalyticProviders.GoogleAnalytics, AnalyticProviders.YandexMetrics };

        public MetricService(IIndex<AnalyticProviders, IProviderMetricService> metricsServicesFactory, IAnalyticUserDataRepository analyticUserDataRepository)
        {
            _metricsServicesFactory = metricsServicesFactory;
            _analyticUserDataRepository = analyticUserDataRepository;
        }

        public async Task TrackPayment(string userId, int totalPayment, string paymentMethod, string countryCode)
        {
            try
            {
                foreach (var analyticProvider in MetricProviders)
                {
                    var service = GetMetricService(analyticProvider);
                    var data = _analyticUserDataRepository.GetByUserIdAndProvider(userId, analyticProvider);
                    if (data == null)
                        continue;
                    await service.TrackPayment(data.Value, totalPayment < 2, paymentMethod, countryCode);
                    Logger.Info("TrackPayment: {0}. Analytic provider: {1}, ", userId, analyticProvider);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, @"Error in TrackPayment: {0}. Stack trace: {1}", e.Message, e.StackTrace);
                throw;
            }

        }

        public async Task TrackRegistrationFromSn(string userId, string socialNetwork, string countryCode)
        {
            try
            {
                foreach (var analyticProvider in MetricProviders)
                {
                    var service = GetMetricService(analyticProvider);
                    var data = _analyticUserDataRepository.GetByUserIdAndProvider(userId, analyticProvider);
                    if (data == null)
                        continue;
                    await service.TrackUserRegistrationDoneFromSocialNetwork(data.Value, socialNetwork, countryCode);
                    Logger.Info("TrackRegistrationFromSn: {0}. Analytic provider: {1}.Social network: {2} ", userId,
                        analyticProvider, socialNetwork);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, @"Error in TrackRegistrationFromSn: {0}. Stack trace: {1}", e.Message, e.StackTrace);
                throw;
            }

        }

        public void SaveCookies(string userId, List<Tuple<string, string, string>> cookieCollection)
        {
            try
            {
                foreach (var analyticProvider in MetricProviders)
                {
                    var service = GetMetricService(analyticProvider);
                    var metricUserId = service.GetMetricUserIdFromCookie(cookieCollection);

                    if (metricUserId == null)
                    {
                        continue;
                    }

                    var data = new AnalyticUserData
                    {
                        UserId = userId,
                        ProviderId = analyticProvider,
                        Value = metricUserId
                    };
                    _analyticUserDataRepository.Upsert(data);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, @"Error in SaveCookies: {0}. Stack trace: {1}", e.Message, e.StackTrace);
                throw;
            }

        }

        private IProviderMetricService GetMetricService(AnalyticProviders provider)
        {
            IProviderMetricService service;
            if (_metricsServicesFactory.TryGetValue(provider, out service))
                return service;
            throw new ArgumentOutOfRangeException(nameof(provider), provider.ToString());
        }
    }
}
