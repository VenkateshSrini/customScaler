using deploy.keda.scaler.Repo;
using MongoDB.Driver;

namespace deploy.keda.scaler.Extension
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
            services.AddSingleton<IKEDARepo, KedaRepo>();
            return services;
        }
    }
}
