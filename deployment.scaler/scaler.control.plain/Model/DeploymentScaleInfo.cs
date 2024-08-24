namespace scaler.control.plain.Model
{
    public class DeploymentScaleInfo
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public bool IsScalingActive { get; set; }
        public int MinScale { get; set; }
        public int MaxScale { get; set; }
    }
}
