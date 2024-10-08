﻿using deploy.keda.scaler.Repo;
using Grpc.Core;

namespace deploy.keda.scaler.Services
{
    public class KedaScalerService : ExternalScaler.ExternalScalerBase
    {
        private readonly ILogger<KedaScalerService> _logger;
        private readonly IKEDARepo _repo;
        public KedaScalerService(ILogger<KedaScalerService> logger, IKEDARepo repo)
        {
            _logger = logger;
            _repo = repo;
        }
        public async override Task<IsActiveResponse> IsActive(ScaledObjectRef request, ServerCallContext context)
        {
            var deployment = await _repo.GetDeploymentScaleInfoAsync(request.Namespace, request.Name);
            return new IsActiveResponse { Result = deployment?.IsScalingActive ?? false };
        }
        public override Task<GetMetricSpecResponse> GetMetricSpec(ScaledObjectRef request, ServerCallContext context)
        {
            var isActiveMetricSpec = new MetricSpec
            {
                MetricName = "is_scaling_active",
                TargetSize = 1
            };

            var desiredSizeMetricSpec = new MetricSpec
            {
                MetricName = "desired_instance_count",
                TargetSize = 1 // This is a placeholder; actual value will be provided in GetMetrics
            };

            var response = new GetMetricSpecResponse();
            response.MetricSpecs.Add(isActiveMetricSpec);
            response.MetricSpecs.Add(desiredSizeMetricSpec);
            return Task.FromResult(response);
        }
        public async override Task<GetMetricsResponse> GetMetrics(GetMetricsRequest request, ServerCallContext context)
        {
            var deploymentInfo = await _repo.GetDeploymentScaleInfoAsync(request.ScaledObjectRef.Namespace, request.ScaledObjectRef.Name);
            MetricValue isActiveMetricValue;
            MetricValue desiredSizeMetricValue;
            if (deploymentInfo == null)
            {
                _logger.LogCritical($"ScaledObject {request.ScaledObjectRef.Name} with namespace {request.ScaledObjectRef.Namespace} not foud in DB");
                isActiveMetricValue = new MetricValue
                {
                    MetricName = "is_scaling_active",
                    MetricValue_ = 0
                };
                var defMinScale = int.Parse(request.ScaledObjectRef.ScalerMetadata["minReplicaCount"]);
                desiredSizeMetricValue = new MetricValue
                {
                    MetricName = "desired_instance_count",
                    MetricValue_ = defMinScale
                };

            }
            else
            {
                isActiveMetricValue = new MetricValue
                {
                    MetricName = "is_scaling_active",
                    MetricValue_ = deploymentInfo?.IsScalingActive == true ? 1 : 0
                };
                int desiredScale; 
                if (deploymentInfo.IsScalingActive)
                {
                    var defMaxScale = int.Parse(request.ScaledObjectRef.ScalerMetadata["maxReplicaCount"]);
                    desiredScale = Math.Min(deploymentInfo.MaxScale, defMaxScale);
                    //maxScale = deploymentInfo?.MaxScale >= defMaxScale ? defMaxScale : deploymentInfo?.MaxScale;

                }
                else
                {
                    var defMinScale = int.Parse(request.ScaledObjectRef.ScalerMetadata["minReplicaCount"]);
                    desiredScale = Math.Max(deploymentInfo.MinScale , defMinScale);
                    //maxScale = deploymentInfo?.MinScale >= defMinScale ? deploymentInfo?.MinScale : defMinScale;
                }

                 desiredSizeMetricValue = new MetricValue
                {
                    MetricName = "desired_instance_count",
                    MetricValue_ = desiredScale
                 };
            }
            _logger.LogInformation($"Returning metrics: {isActiveMetricValue.MetricName}={isActiveMetricValue.MetricValue_}, {desiredSizeMetricValue.MetricName}={desiredSizeMetricValue.MetricValue_}");
            var response = new GetMetricsResponse();
            response.MetricValues.Add(isActiveMetricValue);
            response.MetricValues.Add(desiredSizeMetricValue);
            return response;
        }
        public async override Task StreamIsActive(ScaledObjectRef request, IServerStreamWriter<IsActiveResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                 
                var response = await IsActive(request, context);
                await responseStream.WriteAsync(response);

                // Wait for a short period before checking again
                await Task.Delay(5000, context.CancellationToken); // 5 seconds delay
            }
        }



    }
}
