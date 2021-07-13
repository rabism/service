using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace stock.Entity
{
    public class Company 
    {
        [BsonId]
        public string CompanyCode{get;set;}
        public List<Exchange> Exchanges {get;set;}
        
    }
}