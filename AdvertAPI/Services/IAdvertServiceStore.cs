using AdvertAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvertAPI.Services
{
    public interface IAdvertServiceStore
    {
        Task<string> Add(AdvertModel model);
        Task Confirm(AdvertConfirmModel model);
        Task<bool> CheckHealthAsync();
        Task<AdvertDynamoDBModel> GetById(string id);
        Task<List<AdvertModel>> GetAll();
    }
}
