using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MSA.Common.Contracts.Domain;
using MSA.Common.Contracts.Settings;
using MSA.Common.Domain;
using MSA.Common.Settings;

namespace MSA.Common.MongoDB;

public static class Extensions
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
        // Register Mongo Client
        services.AddSingleton(ServiceProvider =>
        {
            var config = ServiceProvider.GetService<IConfiguration>();
            var serviceSetting = config.GetSection(nameof(ServiceSetting)).Get<ServiceSetting>();
            var mongoDBSetting = config.GetSection(nameof(MongoDBSetting)).Get<MongoDBSetting>();
            var mongoClient = new MongoClient(mongoDBSetting.ConnectionString);
            return mongoClient.GetDatabase(serviceSetting.ServiceName);
        });
        return services;
    }
    public static IServiceCollection AddRepositories<T>(this IServiceCollection services, string collectionName) where T : IEntity
    {
        services.AddSingleton<IRepository<T>>(ServiceProvider =>
        {
            var database = ServiceProvider.GetService<IMongoDatabase>();
            return new MongoRepository<T>(database, collectionName);
        });
        return services;
    }
}