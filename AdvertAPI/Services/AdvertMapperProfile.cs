using AdvertAPI.Models;
using AutoMapper;

namespace AdvertAPI.Services
{
    public class AdvertMapperProfile: Profile
    {
        public AdvertMapperProfile()
        {
            CreateMap<AdvertModel, AdvertDynamoDBModel>();
        }
    }
}
