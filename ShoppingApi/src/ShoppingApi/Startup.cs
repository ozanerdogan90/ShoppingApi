using Akka.Actor;
using Akka.Configuration;
using AutoMapper;
using CorrelationId;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShoppingApi.ShoppingCart;
using ShoppingApi.Tools;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace ShoppingApi
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
            services.AddSingleton<ActorSystem>(ctx =>
            {
                var name = Configuration["Akka:ActorSystemName"];
                var configPath = Configuration["Akka:ConfigPath"];
                return ActorSystem.Create(name, LoadConfig(configPath));
            });
            services.AddSingleton<ActorSelection>(provider =>
            {
                var actorSystem = provider.GetService<ActorSystem>();
                var remotePath = Configuration["Akka:RemotePath"];
                return actorSystem.ActorSelection(remotePath);
            });

            services.AddCorrelationId();
              AddAutoMapperServices(services);
            AddSwaggerServices(services);
            services.AddResponseCompression();
            services.AddProblemDetails();
      
            services.AddScoped<IService, Service>();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(CustomExceptionFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        private IServiceCollection AddSwaggerServices(IServiceCollection services)
        {
            return services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Shopping Api",
                    Description = "A simple shopping api uses akka",
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        private IServiceCollection AddAutoMapperServices(IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, ILoggerFactory builder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            builder.AddConsole();
            app.UseCors(x => x
                  .AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader())
      .UseAuthentication()
      .UseMiddleware<ApiLoggingMiddleware>()
      .UseCorrelationId(new CorrelationIdOptions
      {
          Header = "X-Correlation-ID",
          UseGuidForCorrelationId = true,
          UpdateTraceIdentifier = true,
          IncludeInResponse = true
      })
      .UseSwagger()
      .UseSwaggerUI(c =>
      {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping Api v1");
          c.DocumentTitle = "Shopping Api Swagger Ui";
      })
      .UseResponseCompression()
      .UseProblemDetails().UseMvc() ;

            lifetime.ApplicationStarted.Register(() =>
            {
                app.ApplicationServices.GetService<ActorSystem>();
            });
            lifetime.ApplicationStopping.Register(() =>
            {
                app.ApplicationServices.GetService<ActorSystem>().Terminate().Wait();
            });
        }

        private Config LoadConfig(string configFile)
        {
            if (File.Exists(configFile))
            {
                string config = File.ReadAllText(configFile);
                return ConfigurationFactory.ParseString(config);
            }

            return Config.Empty;
        }
    }
}
