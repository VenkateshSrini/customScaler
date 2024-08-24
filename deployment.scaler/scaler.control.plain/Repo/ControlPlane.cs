using MongoDB.Driver;
using scaler.control.plain.Model;

namespace scaler.control.plain.Repo
{
    public class ControlPlaneRepo : IControlPlanRepo
    {
        private IMongoCollection<DeploymentScaleInfo> _scaleInfoCollection;
        private readonly ILogger<ControlPlaneRepo> _logger;
        public ControlPlaneRepo(IMongoClient mongoClient, MongoUrl mongoUrl,
            ILogger<ControlPlaneRepo> logger)
        {
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _scaleInfoCollection = database.GetCollection<DeploymentScaleInfo>("ScaleInfo");
            _logger = logger;
        }
        public async Task<DeploymentScaleInfo> DeleteDeploymentInfoAsync(string namespaceName, string deploymentName)
        {
            var filter = Builders<DeploymentScaleInfo>.Filter.And(
               Builders<DeploymentScaleInfo>.Filter.Eq(d => d.Namespace, namespaceName),
               Builders<DeploymentScaleInfo>.Filter.Eq(d => d.Name, deploymentName)
           );
            
            return await _scaleInfoCollection.FindOneAndDeleteAsync(filter);
        }

        public async Task<DeploymentScaleInfo> GetDeploymentScaleInfoAsync(string namespaceName, string deploymentName)
        {
            var filter = Builders<DeploymentScaleInfo>.Filter.Eq(d => d.Namespace, namespaceName) &
                         Builders<DeploymentScaleInfo>.Filter.Eq(d => d.Name, deploymentName);
            return await _scaleInfoCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<DeploymentScaleInfo> UpsertDeploymentInfoAsync(DeploymentScaleInfo deploymentScaleInfo)
        {
            var filter = Builders<DeploymentScaleInfo>.Filter.And(
               Builders<DeploymentScaleInfo>.Filter.Eq(d => d.Namespace, deploymentScaleInfo.Namespace),
               Builders<DeploymentScaleInfo>.Filter.Eq(d => d.Name, deploymentScaleInfo.Name)
           );

            var update = Builders<DeploymentScaleInfo>.Update
                .Set(d => d.IsScalingActive, deploymentScaleInfo.IsScalingActive)
                .Set(d => d.MinScale, deploymentScaleInfo.MinScale)
                .Set(d => d.MaxScale, deploymentScaleInfo.MaxScale);
            var options = new FindOneAndUpdateOptions<DeploymentScaleInfo> { IsUpsert = true, ReturnDocument = ReturnDocument.After };
            var result= await _scaleInfoCollection.FindOneAndUpdateAsync(filter, update, options);
            return result;
        }
    }
}
