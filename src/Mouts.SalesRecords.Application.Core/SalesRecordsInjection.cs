using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Mouts.SalesRecords.Application.Core.Services;
using Mouts.SalesRecords.Application.Core.Services.Interfaces;

namespace Mouts.SalesRecords.Application.Core
{
    public static class SalesRecordsInjection
    {
        public static IServiceCollection AddSalesRecordsInjection(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<ISalesService, SalesService>();


            return services;
        }
    }
}
