﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CityInfo.API
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Serialization;

    public class Startup
    {
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
