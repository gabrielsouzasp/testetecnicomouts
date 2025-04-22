using AutoMapper;
using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Responses;

namespace Mouts.SalesRecords.Application.Core.Mappings
{
    public class SaleProfile : Profile
    {
        public SaleProfile()
        {
            CreateMap<Sale, ReadSalesResponse>().ReverseMap();
        }
    }
}
