using MapOfIndustryDataAccess.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapOfIndustryDataAccess
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, Action<DbContextOptions> setupAction)
        {
            var options = new DbContextOptions(services);
            setupAction?.Invoke(options);

            return services;
        }
    }
}
