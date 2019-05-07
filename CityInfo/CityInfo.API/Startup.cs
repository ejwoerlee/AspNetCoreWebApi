using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CityInfo.API
{
    using CityInfo.API.Entities;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Serialization;
    using NLog.Extensions.Logging;
    using Services;

    public class Startup
    {
        // Zelf configuration toevoegen.
        public static IConfiguration Configuration { get; private set; }

        // Zelf Startup functie toevoegen
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Startup.Configuration = configuration; // asp dotnet Core 2 methode..

            // >> dotnet core 1.x manier:
            //var builder = new ConfigurationBuilder() 
            //              .SetBasePath(env.ContentRootPath)
            //              .AddJsonFile("appSettings.json", optional: false, reloadOnChange:true);

            //Configuration = builder.Build();
            //<<
            
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddMvcOptions(AddXmlDataContract);

            //.AddJsonOptions(o => {
            //    if (o.SerializerSettings.ContractResolver != null)
            //    {
            //        var castedResolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
            //        castedResolver.NamingStrategy = null;
            //    }
            //});
            //.AddJsonOptions(AddOptionsForJson);

            // Dependency injection. 3 methoden m.b.t. lifetime van de services.
            // services.AddTransient<>()  // created eacht time they are requested (live-aid stateless services)
            // services.AddScoped<>()     // created once per request
            // services.AddSingleton<>(); // created the first time they are requested (when configureServices is run)

#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
            var connectionString = Startup.Configuration["connectionStrings:cityInfoDBConnectionString"];
            services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString));

        }

        private void AddOptionsForJson(MvcJsonOptions options)
        {
            if ((options != null) && (options.SerializerSettings.ContractResolver != null))
            {
                var castedResolver = options.SerializerSettings.ContractResolver as DefaultContractResolver;
                castedResolver.NamingStrategy = null;
            }
        }

        private void AddXmlDataContract(MvcOptions options)
        {
            options?.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
        }
       

       // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // loggerFactory.AddConsole();
            // loggerFactory.AddDebug();

            // loggerFactory.AddProvider(new NLogLoggerProvider()); -> Nlog heeft default extension method:
           loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            app.UseStatusCodePages();
            app.UseMvc();

            //app.Run((context) => {
            //    throw new Exception("Example exception");
            //});

            app.Run(async (context) => { await context.Response.WriteAsync("Hello World!"); });
        }
    }
}
