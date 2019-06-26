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
            dynamoDbModel.Id = Guid.NewGuid().ToString();
            dynamoDbModel.CreationDate = DateTime.Now;
            dynamoDbModel.Status = AdvertStatus.Pending;

            using (var _client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1))
            {
                using(var _context = new DynamoDBContext(_client))
                {
                    await _context.SaveAsync(dynamoDbModel);
                }
            }
            return dynamoDbModel.Id;
        }

        public async Task<bool> CheckHealthAsync()
        {
            using (var _client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1))
            {
                var dbHealth = await _client.DescribeTableAsync("Advert");
                return dbHealth.Table.TableStatus.ToString().ToLower() == "active";
            }
        }

        public async Task Confirm(AdvertConfirmModel model)
        {
            using(var _client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1))
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
                        await _context.SaveAsync(advert);
                    }
                    else
                    {
                        await _context.DeleteAsync<AdvertDynamoDBModel>(advert);
                    }
                }
            }
        }

        public async Task<List<AdvertModel>> GetAll()
        {
            using(var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1))
            {
                using(var context = new DynamoDBContext(client))
                {
                    var scanResult = await context.ScanAsync<AdvertDynamoDBModel>(new List<ScanCondition>()).GetNextSetAsync();

                    return scanResult.Select(item => _mapper.Map<AdvertModel>(item)).ToList();
                }
            }
        }

        public async Task<AdvertDynamoDBModel> GetById(string id)
        {
            var dbContext = new AdvertDynamoDBModel();
            using (var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1))
            {
                using(var context = new DynamoDBContext(client))
                {
                    dbContext = await context.LoadAsync<AdvertDynamoDBModel>(id);
                }
            }
            return dbContext;
        }
    }
}
