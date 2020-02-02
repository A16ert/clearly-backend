using System.Reflection;
using ClearlyApi.Services.Auth;
using ClearlyApi.Services.Chat.Manager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SmsSender;

namespace ClearlyApi.Services
{
    public static class Installer
    {
        public static void AddBuisnessServices(this IServiceCollection container)
        {
            container
                .AddScoped<ISmsProvider, SmsRuProvider>()
                .AddScoped<IAuthService, AuthService>();
        }

        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app,
                                                        PathString path,
                                                        WebSocketHandler handler)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<WebSocketManagerMiddleware>(handler));
        }

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<ConnectionManager>();

            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }
    }
}
