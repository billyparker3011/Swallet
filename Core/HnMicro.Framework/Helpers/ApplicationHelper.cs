﻿using HnMicro.Core.Helpers;
using HnMicro.Framework.Middlewares;
using HnMicro.Framework.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace HnMicro.Framework.Helpers
{
    public static class ApplicationHelper
    {
        public static void Usage(this WebApplication app)
        {
            Console.WriteLine($"Print to test Environment: {app.Environment.EnvironmentName}. Is Development: {app.Environment.IsDevelopment()}.");
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHealthChecks(OtherHelper.HealthChecksPath);
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseOnlyCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            app.UseResponseCompression();
            app.MapControllers();
        }

        private static void UseOnlyCors(this WebApplication app)
        {
            var corsOption = app.Configuration.GetSection(CorsOption.AppSettingName).Get<CorsOption>();
            app.UseCors(corsOption.Name);
        }
    }
}
