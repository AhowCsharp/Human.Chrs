using Human.Repository.SubscribeTableDependencies;

namespace SignalR_SqlTableDependency.MiddlewareExtensions
{
    public static class SqlDependencyMiddleware
    {
        public static void UseSqlTableDependency<T>(this IApplicationBuilder applicationBuilder)
            where T : ISubscribeTableDependency
        {
            var serviceProvider = applicationBuilder.ApplicationServices;
            var service = serviceProvider.GetService<T>();
            service.SubscribeTableDependency();
        }
    }
}