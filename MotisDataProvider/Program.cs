using GhalaDataPool;
using Nox;
using Nox.WebApi;
using XAuthPool;

namespace MotisDataProvider
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(new XLogProvider(builder.Configuration));
            builder.Services.AddXLog();

            // add services
            builder.Services.AddSingleton<XAuth>();
            builder.Services.AddSingleton<GhalaDataPool.Ghala>();
            builder.Services.AddSingleton<MotisDataAccess.Motis>();

            var app = builder.Build();

            // renew timer for motis token ... 
            var Configuration = app.Services.GetService<IConfiguration>() ?? throw new NullReferenceException("IConfiguration");
            var Logger = app.Services.GetService<ILogger>();
            var timer = XAuth.GetTokenRenewTimer(app.Services.GetRequiredService<XAuth>(),
                Configuration["MotisDataProvider:Token"] ?? throw new ArgumentNullException("MotisDataProvider:Token"),
                Configuration["MotisDataProvider:Secret"] ?? throw new ArgumentNullException("MotisDataProvider:Secret"));
            var timer2 = XAuth.GetTokenRenewTimer(app.Services.GetRequiredService<XAuth>(),
                Configuration["GhalaDataProvider:Token"] ?? throw new ArgumentNullException("GhalaDataProvider:Token"),
                Configuration["GhalaDataProvider:Secret"] ?? throw new ArgumentNullException("XAuthProvider:Secret"));


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
