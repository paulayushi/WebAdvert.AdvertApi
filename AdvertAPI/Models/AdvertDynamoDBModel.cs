using System;
using Amazon.DynamoDBv2.DataModel;

namespace AdvertAPI.Models
{
    [DynamoDBTable("Advert")]
    public class AdvertDynamoDBModel
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        [DynamoDBProperty]
        public string Title { get; set; }
        [DynamoDBProperty]
        public string Description { get; set; }
        [DynamoDBProperty]
        public double Price { get; set; }
        [DynamoDBProperty]
        public DateTime CreationDate { get; set; }
        [DynamoDBProperty]
        public AdvertStatus Status { get; set; }
    }
}
