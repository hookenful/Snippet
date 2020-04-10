using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAbstraction.Services
{
    public interface IProviderMetricService
    {
        Task TrackPayment(string clientId, bool first, string method, string countryCode);
        Task TrackUserRegistrationDoneFromSocialNetwork(string clientId, string socialNetwork, string countryCode);
        string GetMetricUserIdFromCookie(List<Tuple<string, string, string>> cookieCollection);
    }
}
