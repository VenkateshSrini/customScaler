using deploy.keda.scaler.Model;
using MongoDB.Driver;

namespace deploy.keda.scaler.Repo
{
    public class KedaRepo : IKEDARepo
    {
        private IMongoCollection<DeploymentScaleInfo> _scaleInfoCollection;
        private readonly ILogger<KedaRepo> _logger;
        public KedaRepo(IMongoClient mongoClient, MongoUrl mongoUrl,
            ILogger<KedaRepo> logger) {
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _scaleInfoCollection = database.GetCollection<DeploymentScaleInfo>("ScaleInfo");
            _logger = logger;
        }
        public async Task<DeploymentScaleInfo> GetDeploymentScaleInfoAsync(string namespaceName, string deploymentName)
        {
            var filter = Builders<DeploymentScaleInfo>.Filter.Eq(d => d.Namespace, namespaceName) &
                         Builders<DeploymentScaleInfo>.Filter.Eq(d => d.Name, deploymentName);
            return await _scaleInfoCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
