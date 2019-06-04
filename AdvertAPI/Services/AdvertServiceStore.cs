using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertAPI.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;

namespace AdvertAPI.Services
{
    public class AdvertServiceStore : IAdvertServiceStore
    {
        private readonly IMapper _mapper;
        public AdvertServiceStore(IMapper mapper) {
            _mapper = mapper;
        }
        public async Task<string> Add(AdvertModel model)
        {
            var dynamoDbModel = _mapper.Map<AdvertDynamoDBModel>(model);
            dynamoDbModel.Id = new Guid().ToString();
            dynamoDbModel.CreationDate = DateTime.Now;
            dynamoDbModel.Status = AdvertStatus.Pending;

            using (var _client = new AmazonDynamoDBClient())
            {
                using(var _context = new DynamoDBContext(_client))
                {
                    await _context.SaveAsync(dynamoDbModel);
                }
            }
            return dynamoDbModel.Id;
        }

        public async Task Confirm(AdvertConfirmModel model)
        {
            using(var _client = new AmazonDynamoDBClient())
            {
                using(var _context = new DynamoDBContext(_client))
                {
                    var advert = await _context.LoadAsync<AdvertDynamoDBModel>(model.Id);
                    if (advert == null)
                    {
                        throw new KeyNotFoundException($"Record is not found for corresponding ID = {advert.Id}");
                    }
                    if(model.Status == AdvertStatus.Active)
                    {
                        advert.Status = AdvertStatus.Active;
                    }
                    else
                    {
                        await _context.DeleteAsync<AdvertDynamoDBModel>(advert);
                    }
                }
            }
        }
    }
}
