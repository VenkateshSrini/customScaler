using scaler.control.plain.Model;

namespace scaler.control.plain.Repo
{
    public interface IControlPlanRepo
    {
        Task<DeploymentScaleInfo> GetDeploymentScaleInfoAsync(string namespaceName, string deploymentName);
        Task<DeploymentScaleInfo> UpsertDeploymentInfoAsync(DeploymentScaleInfo deploymentScaleInfo);
        Task<DeploymentScaleInfo> DeleteDeploymentInfoAsync(string namespaceName, string deploymentName);
    }
}
