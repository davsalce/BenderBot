using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackSeries.Core.Client
{
    public partial interface ITrackSeriesClient
    {
        void SetCurrentUserToken(string token);
    }

    public partial class TrackSeriesClient : ITrackSeriesClient
    {
        public void SetCurrentUserToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
        }
    }
}
