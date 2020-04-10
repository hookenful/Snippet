using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAbstraction.Services
{
    public interface IMetricService
    {
        void SaveCookies(string userId, List<Tuple<string, string, string>> cookieCollection);
        Task TrackPayment(string userId, int totalPayments, string paymentMethod, string countryCode);
        Task TrackRegistrationFromSn(string userId, string socialNetwork, string countryCode);
    }
}
