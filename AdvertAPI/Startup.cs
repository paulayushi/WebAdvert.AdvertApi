using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoMapper;
using AdvertAPI.Services;
using AdvertAPI.Health;
using Swashbuckle.AspNetCore.Swagger;

namespace AdvertAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper();
            services.AddTransient<IAdvertServiceStore, AdvertServiceStore>();
            services.AddHealthChecks();
            services.AddHealthChecks(checks => {
                checks.AddCheck<StorageHealthCheck>("DynamoDBHealth", new TimeSpan(0, 1, 0));
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("advertapi", new Info
                {
                    Title = "Advert Web API",
                    Description = "Documentation for Advert Api",
                    Version = "version 1.0",
                    Contact = new Contact
                    {
                        Name = "Sambhu Paul",
                        Email = "a.b@c.com"
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/Health");
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/advertapi/swagger.json", "Web Advert Api");
            });
            app.UseMvc();
        }
    }
}
