using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NFCDataRESTApi.Repositories;
using NFCDataRESTApi.SQLiteDataBase;
using RESTApi.Repositories;
using SQLitePCL;
using Swashbuckle.AspNetCore.Swagger;

namespace RESTApi
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
            // Add framework services.
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Moritz Schmidt, Andreas Weber, REST-Api", Version = "v3" });
                c.IncludeXmlComments(this.GetSwaggerPath());
            });

            services.AddDbContext<DataBase>(options =>
            {
                options.UseSqlite(@"DataSource=mydatabase.db;");
            });

            services.AddTransient<StudiCardRepository>();
            services.AddTransient<ErrorHandlingRepository>();

            Batteries_V2.Init();
            Console.WriteLine("Initialized database, in case the host is a linux system.");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "REST-Api");
            });

            app.UseMvc();

            app.UseMvc();
        }

        private string GetSwaggerPath()
        {
            var result = string.Empty;

            result = Path.ChangeExtension(Assembly.GetEntryAssembly().Location, "xml");

            return result.Replace("Release", "Debug");
        }
    }
}
