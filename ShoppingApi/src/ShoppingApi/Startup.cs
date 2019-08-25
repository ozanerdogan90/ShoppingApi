using Akka.Actor;
using Akka.Configuration;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingApi.ShoppingCart;
using System.IO;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<ActorSystem>(ctx =>
            {
                return ActorSystem.Create("ShoppingApiAkkaServer", LoadConfig("akka-client.conf"));
            });

            services.AddSingleton<IActorRef>(provider =>
            {
                var actorSystem = provider.GetService<ActorSystem>();
                var actor = actorSystem.ActorOf(Props.Create(() => new Shared.Client.Actor()), "BasketActor");
                return actor;
            });

            services.AddSingleton<ActorSelection>(provider =>
            {
                var actorSystem = provider.GetService<ActorSystem>();

                return actorSystem.ActorSelection("akka.tcp://ShoppingApiAkkaServer@localhost:8081/user/BasketActor");
            });

            services.AddScoped<IService, Service>();
            AddAutoMapperServices(services);
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

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
