﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;         

namespace stock.Entity
{
    public class Stock 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string StockId { get; set; }
        public decimal StockPrice { get; set; }
        public DateTime StockDateTime { get; set; } = DateTime.Now;
        public string CompanyCode { get; set; }
        public string ExchangeName {get;set;}
     
    }
}
