using MongoDB.Driver;
using scaler.control.plain.Repo;

namespace scaler.control.plain.ServiceExtension
{
    public static class RepoExtension
    {
        public static IServiceCollection AddMongoRepo(this IServiceCollection services,
            IConfiguration configuration, string sectionName, string connectionStrringAttribute)
        {
            var mongourl = MongoUrl.Create(configuration[$"{sectionName}:{connectionStrringAttribute}"]);
            var mongoClient = new MongoClient(mongourl);
            services.AddSingleton<MongoUrl>(mongourl);
            services.AddSingleton<IMongoClient>(mongoClient);
            services.AddSingleton<IControlPlanRepo, ControlPlaneRepo>();
            return services;
        }
    }
}
