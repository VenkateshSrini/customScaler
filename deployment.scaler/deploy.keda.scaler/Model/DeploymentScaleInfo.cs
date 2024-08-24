using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace deploy.keda.scaler.Model
{
    public class DeploymentScaleInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string Name { get; set; }
        public string Namespace { get; set; }
        public bool IsScalingActive { get; set; }
        public int MinScale { get; set; }
        public int MaxScale { get; set; }
    }
}
