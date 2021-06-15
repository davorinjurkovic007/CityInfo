using CityInfo.API.Context;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace CityInfo.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc();
            services.AddControllers()
                .AddNewtonsoftJson(setupAction =>
                {
                    //setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    //setupAction.SerializerSettings.ContractResolver = new DefaultContractResolver();
                })
                .AddXmlDataContractSerializerFormatters();
#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
            // ako stavim u launchSetting.json parametre za connection string, sa ovim ih mogu dohvatiti. To je sigurniji naèin nego u
            // application.json fajlovima. Stavim ga ispod ASPNETCORE_ENVIRONMENT i to se vidi u properties -> Eviroment variables
            //var connectionString = Configuration["something:cityInfoDatabase"];
            services.AddDbContext<CityInfoContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("cityInfoDatabase"));
            });

            services.AddScoped<ICityInfoRepository, CityInfoRepository>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                 app.UseExceptionHandler(errorApp => errorApp.Run(_ => Task.CompletedTask));
            }

            app.UseStatusCodePages();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //    //throw new Exception("Example exception");
                //});
            });

            //app.Run((context) =>
            //{
            //    //await context.Response.WriteAsync("Hello World!");
            //    throw new Exception("Example exception");
            //});
        }
    }
}
