using MongoDB.Bson;

namespace Simple.Models
{
    public class Product
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}
