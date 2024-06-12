using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HnMicro.Modules.EntityFrameworkCore.Helpers
{
    public static class MigrationHelper
    {
        public static void Migrate<T>(this WebApplication webApplication) where T : DbContext
        {
            using var scope = webApplication.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<T>();
            if (dbContext == default(T))
            {
                throw new ArgumentNullException(nameof(T));
            }
            dbContext.Database.Migrate();
        }

        public static async Task MigrateAsync<T>(this WebApplication webApplication) where T : DbContext
        {
            using var scope = webApplication.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<T>();
            if (dbContext == default(T))
            {
                throw new ArgumentNullException(nameof(T));
            }
            await dbContext.Database.MigrateAsync();
        }
    }
}
