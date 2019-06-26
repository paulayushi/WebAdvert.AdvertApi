using AdvertAPI.Services;
using Microsoft.Extensions.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AdvertAPI.Health
{
    public class StorageHealthCheck : IHealthCheck
    {
        private readonly IAdvertServiceStore _advertServiceStore;

        public StorageHealthCheck(IAdvertServiceStore advertServiceStore)
        {
            _advertServiceStore = advertServiceStore;
        }
        public async ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var isDynamoDBHealthy = await _advertServiceStore.CheckHealthAsync();
            return HealthCheckResult.FromStatus(isDynamoDBHealthy ? CheckStatus.Healthy : CheckStatus.Unhealthy, "");
        }
    }
}
