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
            services.AddControllers().AddNewtonsoftJson(config => config.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddHttpContextAccessor();

            services.AddDbContext<NolowaContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("NolowaContext"));
            });

            services.AddSignalR();

            // Add functionality to inject IOptions<T>
            services.AddOptions();
            services.Configure<JWT>(Configuration.GetSection("JWT"));
            services.Configure<GoogleLoginConfiguration>(Configuration.GetSection("SocialLoginGroup:GoogleLoginOption"));

            AddScoped(services);

            //services.AddTransient(typeof(IHttpProvider<,>), typeof(HttpProvider<,>));
            services.AddTransient(typeof(IHttpHeader), typeof(HttpProvider));
            services.AddTransient(typeof(IHttpProvider), typeof(HttpProvider));

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

        private void AddScoped(IServiceCollection services)
        {
            services.AddScoped<IAccountsService, AccountsService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPostsService, PostsService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IJWTTokenProvider, JWTTokenProvider>();
            services.AddScoped<IDirectMessageService, DirectMessageService>();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<DirectMessageHub>("/DirectMessage");
            });
        }
    }
}
