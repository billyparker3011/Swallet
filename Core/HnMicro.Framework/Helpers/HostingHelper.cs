using FluentValidation.AspNetCore;
using HnMicro.Core.Scopes;
using HnMicro.Framework.Configs;
using HnMicro.Framework.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace HnMicro.Framework.Helpers
{
    public static class HostingHelper
    {
        private const string _aspNetCoreEnvironment = "ASPNETCORE_ENVIRONMENT";
        private const string _aspNetCoreDevelopment = "development";
        private const string _aspNetCoreStaging = "staging";
        private const string _aspNetCoreProduction = "production";

        public static void BuildServices(this WebApplicationBuilder builder, params Type[] filters)
        {
            builder.Services.AddHealthChecks();
            builder.Services.AddResponseCaching();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });
            //  Authentication Validate
            var authenticationValidateOption = builder.Configuration.GetSection(AuthenticationValidateOption.AppSettingName).Get<AuthenticationValidateOption>() ?? throw new ArgumentNullException(nameof(AuthenticationValidateOption));
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = authenticationValidateOption.AuthenticationAddress;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = authenticationValidateOption.ValidateAudience,
                    ValidAudience = authenticationValidateOption.ValidAudience,

                    ValidateIssuer = authenticationValidateOption.ValidateIssuer,
                    ValidIssuer = authenticationValidateOption.ValidIssuer,

                    ValidateIssuerSigningKey = authenticationValidateOption.ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationValidateOption.IssuerSigningKey)),

                    ValidateLifetime = authenticationValidateOption.ValidateLifetime
                };
                options.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration();   //  Fix bug: Unable to obtain configuration .well-known/openid-configuration
            });
            builder.Services.AddControllers(configure =>
            {
                foreach (var item in filters) configure.Filters.Add(item);
            })
            .AddFluentValidation(configure =>
            {
                configure.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            })
            .AddJsonOptions(config =>
            {
                config.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
            //  CORS
            var corsOption = builder.Configuration.GetSection(CorsOption.AppSettingName).Get<CorsOption>() ?? throw new ArgumentNullException(nameof(CorsOption));
            builder.Services.AddCors(config =>
            {
                config.AddPolicy(corsOption.Name, policy =>
                {
                    policy.WithOrigins(corsOption.Urls.ToArray())
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            builder.Services.AddDependencies();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(config =>
            {
                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            builder.Services.AddRouting(option =>
            {
                option.LowercaseUrls = true;
            });
        }

        public static void BuildSignalRService(this WebApplicationBuilder builder, params Type[] filters)
        {
            builder.Services.AddHealthChecks();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();
            //  Authentication Validate
            var authenticationValidateOption = builder.Configuration.GetSection(AuthenticationValidateOption.AppSettingName).Get<AuthenticationValidateOption>() ?? throw new ArgumentNullException(nameof(AuthenticationValidateOption));
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = authenticationValidateOption.AuthenticationAddress;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = authenticationValidateOption.ValidateAudience,
                    ValidAudience = authenticationValidateOption.ValidAudience,

                    ValidateIssuer = authenticationValidateOption.ValidateIssuer,
                    ValidIssuer = authenticationValidateOption.ValidIssuer,

                    ValidateIssuerSigningKey = authenticationValidateOption.ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationValidateOption.IssuerSigningKey)),

                    ValidateLifetime = authenticationValidateOption.ValidateLifetime
                };
                options.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration();   //  Fix bug: Unable to obtain configuration .well-known/openid-configuration
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Query[WebSocketConfigs.AccessToken];
                        if (context.HttpContext.WebSockets.IsWebSocketRequest && string.IsNullOrEmpty(token)) throw new ArgumentNullException(WebSocketConfigs.AccessToken);
                        context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });
            var socketOption = builder.Configuration.GetSection(WebSocketOption.AppSettingName).Get<WebSocketOption>() ?? throw new ArgumentNullException(nameof(WebSocketOption));
            builder.Services.AddSignalR(option =>
            {
                option.EnableDetailedErrors = socketOption.EnableDetailedErrors;
                option.KeepAliveInterval = TimeSpan.FromSeconds(socketOption.KeepAliveIntervalInSeconds);
                option.ClientTimeoutInterval = TimeSpan.FromSeconds(socketOption.ClientTimeoutIntervalInSeconds);

                foreach (var item in filters) option.AddFilter(item);
            });
            //  CORS
            var corsOption = builder.Configuration.GetSection(CorsOption.AppSettingName).Get<CorsOption>() ?? throw new ArgumentNullException(nameof(CorsOption));
            builder.Services.AddCors(config =>
            {
                config.AddPolicy(corsOption.Name, policy =>
                {
                    policy.WithOrigins(corsOption.Urls.ToArray())
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(origin => true)
                        .AllowCredentials()
                        .SetIsOriginAllowedToAllowWildcardSubdomains();
                });
            });
            builder.Services.AddDependencies();
        }

        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddDynamicDependencyByAttribute();
            services.AddDynamicDependencyByInterface();
        }

        private static void AddDynamicDependencyByAttribute(this IServiceCollection services)
        {
            var scopes = new List<Type> { typeof(ScopedLifeTimeAttribute), typeof(TransientLifeTimeAttribute), typeof(SingletonLifeTimeAttribute) };
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            foreach (var item in scopes)
            {
                var lifeTime = ServiceLifetime.Scoped;

                switch (item.Name)
                {
                    case nameof(TransientLifeTimeAttribute):
                        lifeTime = ServiceLifetime.Transient;
                        break;
                    case nameof(SingletonLifeTimeAttribute):
                        lifeTime = ServiceLifetime.Singleton;
                        break;
                    case nameof(ScopedLifeTimeAttribute):
                        lifeTime = ServiceLifetime.Scoped;
                        break;
                    default:
                        throw new ArgumentException(string.Format("{0} is not a valid type in this context.", item.Name));
                }

                var servicesToBeRegistered = assemblies.SelectMany(f => f.GetTypes()).Where(f1 => f1.IsDefined(item, false)).ToList();
                foreach (var serviceType in servicesToBeRegistered)
                {
                    var implementations = new List<Type>();

                    if (serviceType.IsGenericType && serviceType.IsGenericTypeDefinition)
                    {
                        implementations = assemblies.SelectMany(a => a.GetTypes())
                                                    .Where(type => type.IsGenericType && type.IsClass && type.GetInterfaces()
                                                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == serviceType.GetGenericTypeDefinition()))
                                                    .ToList();
                    }
                    else
                    {
                        implementations = assemblies.SelectMany(a => a.GetTypes())
                                                    .Where(type => serviceType.IsAssignableFrom(type) && type.IsClass).ToList();
                    }

                    if (implementations.Any())
                    {
                        foreach (var implementation in implementations)
                        {
                            var isGenericTypeDefinition = implementation.IsGenericType && implementation.IsGenericTypeDefinition;
                            var service = isGenericTypeDefinition && serviceType.IsGenericType && !serviceType.IsGenericTypeDefinition && serviceType.ContainsGenericParameters
                                      ? serviceType.GetGenericTypeDefinition()
                                      : serviceType;

                            var isAlreadyRegistered = services.Any(s => s.ServiceType == service && s.ImplementationType == implementation);

                            if (!isAlreadyRegistered)
                            {
                                services.Add(new ServiceDescriptor(service, implementation, lifeTime));
                            }
                        }
                    }
                    else
                    {
                        if (serviceType.IsClass)
                        {
                            var isAlreadyRegistered = services.Any(s => s.ServiceType == serviceType && s.ImplementationType == serviceType);

                            if (!isAlreadyRegistered)
                            {
                                services.Add(new ServiceDescriptor(serviceType, serviceType, lifeTime));
                            }
                        }
                    }
                }
            }
        }

        private static void AddDynamicDependencyByInterface(this IServiceCollection services)
        {
            var scopes = new List<Type> { typeof(IScopedDependency), typeof(ITransientDependency), typeof(ISingletonDependency) };
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            //  Fix bug: Cannot load reference module
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (var dll in Directory.GetFiles(path, "*.dll"))
            {
                if (assemblies.Any(f => f.Location == dll)) continue;
                assemblies.Add(Assembly.LoadFile(dll));
            }

            foreach (var item in scopes)
            {
                var lifeTime = ServiceLifetime.Scoped;

                switch (item.Name)
                {
                    case nameof(ITransientDependency):
                        lifeTime = ServiceLifetime.Transient;
                        break;
                    case nameof(ISingletonDependency):
                        lifeTime = ServiceLifetime.Singleton;
                        break;
                    case nameof(IScopedDependency):
                        lifeTime = ServiceLifetime.Scoped;
                        break;
                    default:
                        throw new ArgumentException(string.Format("{0} is not a valid type in this context.", item.Name));
                }

                var implementations = assemblies.SelectMany(f => f.GetTypes()).Where(f1 => item.IsAssignableFrom(f1) && f1.IsClass).ToList();
                foreach (var implementation in implementations)
                {
                    var servicesToBeRegistered = implementation.GetInterfaces().Where(i => i != typeof(ITransientDependency) && i != typeof(IScopedDependency) && i != typeof(ISingletonDependency) && i.GetInterfaces().Contains(item)).ToList();
                    if (servicesToBeRegistered.Any())
                    {
                        foreach (var serviceType in servicesToBeRegistered)
                        {
                            var isGenericTypeDefinition = implementation.IsGenericType && implementation.IsGenericTypeDefinition;
                            var service = isGenericTypeDefinition && serviceType.IsGenericType && !serviceType.IsGenericTypeDefinition && serviceType.ContainsGenericParameters
                                    ? serviceType.GetGenericTypeDefinition()
                                    : serviceType;

                            var isAlreadyRegistered = services.Any(s => s.ServiceType == service && s.ImplementationType == implementation);
                            if (isAlreadyRegistered) continue;
                            services.Add(new ServiceDescriptor(service, implementation, lifeTime));
                        }
                    }
                    else if (implementation.IsClass)
                    {
                        var isAlreadyRegistered = services.Any(s => s.ServiceType == implementation && s.ImplementationType == implementation);
                        if (isAlreadyRegistered) continue;
                        services.Add(new ServiceDescriptor(implementation, implementation, lifeTime));
                    }
                }
            }
        }

        public static IConfiguration BuildConfiguration(this string basePath)
        {
            var val = Environment.GetEnvironmentVariable(_aspNetCoreEnvironment);
            if (string.IsNullOrEmpty(val)) val = _aspNetCoreDevelopment;

            var appSettingsFile = "appsettings.json";

            if (val.Equals(_aspNetCoreDevelopment, StringComparison.OrdinalIgnoreCase))
            {
                appSettingsFile = "appsettings.Development.json";
            }
            else if (val.Equals(_aspNetCoreStaging, StringComparison.OrdinalIgnoreCase))
            {
                appSettingsFile = "appsettings.Staging.json";
            }
            else if (val.Equals(_aspNetCoreProduction, StringComparison.OrdinalIgnoreCase))
            {
                appSettingsFile = "appsettings.Production.json";
            }
            return new ConfigurationBuilder()
                            .SetBasePath(basePath)
                            .AddJsonFile(appSettingsFile, false, true)
                            .AddEnvironmentVariables()
                            .Build();
        }
    }
}
