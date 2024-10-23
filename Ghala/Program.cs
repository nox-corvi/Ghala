using Ghala.Components;
using GhalaDataPool;
using MotisDataPool.Data;
using Nox;
using Nox.WebApi;
using Radzen;
using XAuthPool;
using XAuthPool.Security;

namespace Ghala
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(new XLogProvider(builder.Configuration));
            builder.Services.AddXLog();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddRadzenComponents();


            
            builder.Services.AddSingleton<XAuth>();
            builder.Services.AddSingleton<AuthTokenManager>();
            builder.Services.AddSingleton<UserProfileManager>();

            builder.Services.AddScoped<GhalaDataPool.Ghala>();
            
            builder.Services.AddScoped<MotisDataPool.Motis>();
            builder.Services.AddScoped<TrackingInfo>();

            var app = builder.Build();

            var Configuration = app.Services.GetService<IConfiguration>() ?? throw new NullReferenceException("IConfiguration");
            var Logger = app.Services.GetService<ILogger>();
            var timer1 = XAuth.GetTokenRenewTimer(app.Services.GetRequiredService<XAuth>(),
                Configuration["GhalaDataProvider:Token"] ?? throw new ArgumentNullException("GhalaDataProvider:Token"),
                Configuration["GhalaDataProvider:Secret"] ?? throw new ArgumentNullException("GhalaDataProvider:Secret"));
            
            var timer2 = XAuth.GetTokenRenewTimer(app.Services.GetRequiredService<XAuth>(),
                Configuration["MotisDataProvider:Token"] ?? throw new ArgumentNullException("MotisDataProvider:Token"),
                Configuration["MotisDataProvider:Secret"] ?? throw new ArgumentNullException("MotisProvider:Secret"));

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            
            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
