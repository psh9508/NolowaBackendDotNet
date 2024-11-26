using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Core.SignalR.Hubs;
using NolowaBackendDotNet.Core.Mapper;
using NolowaBackendDotNet.Models.Configuration;
using NolowaBackendDotNet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NolowaBackendDotNet.Core.SignalR;
using NolowaBackendDotNet.Core.CacheMonitor;
using NolowaBackendDotNet.Core.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NolowaBackendDotNet.Core.MessageQueue;
using SharedLib.MessageQueue;

namespace NolowaBackendDotNet
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
            services.AddControllers().AddJsonOptions(x =>x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
            services.AddControllers().AddNewtonsoftJson(config => config.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddHttpContextAccessor();

            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = Configuration.GetConnectionString("Redis");
            //    options.InstanceName = "Nolowa_";
            //});

            #region Redis
            services.Configure<RedisCacheOptions1>(options =>
                {
                    options.Configuration = Configuration.GetConnectionString("Redis_DM");
                    options.InstanceName = "Nolowa_DM_";
                });
            services.Configure<RedisCacheOptions2>(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis_Post");
                options.InstanceName = "Nolowa_Post_";
            });
            services.Configure<RedisCacheOptions3>(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis_Search");
                options.InstanceName = "Nolowa_Search_";
            });
            #endregion

            var jwtKey = Configuration.GetSection("JWT:secret").Value;

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddDbContext<NolowaContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("NolowaContext"));
            });

            services.AddSignalR();

            // Add functionality to inject IOptions<T>
            services.AddOptions();
            services.Configure<JWT>(Configuration.GetSection("JWT"));
            services.Configure<GoogleLoginConfiguration>(Configuration.GetSection("SocialLoginGroup:GoogleLoginOption"));

            //services.Add(ServiceDescriptor.Singleton<IDirectMessageRedis, DirectMessageRedis>());
            //services.Add(ServiceDescriptor.Singleton<IPostRedis, PostRedis>());

            // Message

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NolowaBackendDotNet", Version = "v1" });
            });

            // 싱글톤 객체에 복사본을 만들어서 Injection을 할 수 없는 상황에서 Resolve 할 때 사용
            InstanceResolver.Instance.ServiceProvider = services.BuildServiceProvider(); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NolowaBackendDotNet v1"));
                IdentityModelEventSource.ShowPII = true;
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<DirectMessageHub>("/NolowaSocket");
            });

            // DB, 레디스등 올바로 동작하는지 확인
            // 여기서 타임아웃까지 기다리지 않도록 처리해야함
           
            //var redis = InstanceResolver.Instance.Resolve<IPostRedis>();
            //redis.GetAsync("healthCheck");
        }
    }
}
