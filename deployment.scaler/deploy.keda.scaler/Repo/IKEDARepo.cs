using deploy.keda.scaler.Model;

namespace deploy.keda.scaler.Repo
{
    public interface IKEDARepo
    {
        Task<DeploymentScaleInfo> GetDeploymentScaleInfoAsync(string namespaceName, string deploymentName);
    }
}
