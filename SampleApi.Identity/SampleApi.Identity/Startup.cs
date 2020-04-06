using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SampleApi.Identity.Constants;
using Serilog;
using System;
using System.Collections.Generic;

namespace SampleApi.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        public Microsoft.AspNetCore.Hosting.IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddOptions();

            services.Configure<AdminConfiguration>(Configuration.GetSection(ConfigurationConsts.AdminConfigurationKey));

            //services.TryAddSingleton<IRootConfiguration, RootConfiguration>();
            services.AddSingleton( isp => {
                var iopts = isp.GetService<IOptions<AdminConfiguration>>();
                IRootConfiguration rootConfig = new RootConfiguration(iopts);
                return rootConfig;
            } );

            //OidcApiName
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(ApiConfigurationConsts.ApiVersionV1, new OpenApiInfo { Title = ApiConfigurationConsts.ApiName, Version = ApiConfigurationConsts.ApiVersionV1 });
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{Configuration.GetSection(ConfigurationConsts.AdminConfigurationKey).GetSection("IdentityServerBaseUrl").Value}/connect/authorize"),
                            //TokenUrl = new Uri("your-auth-url"),
                            Scopes = new Dictionary<string, string> {
                                { Configuration.GetSection(ConfigurationConsts.AdminConfigurationKey).GetSection("OidcApiName").Value, ApiConfigurationConsts.ApiName }
                            }
                        }
                    }
                });
                options.IncludeXmlComments($@"{HostingEnvironment.ContentRootPath}\SampleApi.Identity.xml", true);
                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });
            
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddIdentityServerAuthentication(options =>
    {
        options.Authority = Configuration.GetSection(ConfigurationConsts.AdminConfigurationKey).GetSection("IdentityServerBaseUrl").Value;
        options.ApiName = Configuration.GetSection(ConfigurationConsts.AdminConfigurationKey).GetSection("OidcApiName").Value;
#if DEBUG
        options.RequireHttpsMetadata = false;
#else
                    options.RequireHttpsMetadata = true;
#endif
    });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, ILoggerFactory loggerFactorys, IRootConfiguration rootConfiguration)
        {

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/" + ApiConfigurationConsts.ApiVersionV1 + "/swagger.json", ApiConfigurationConsts.ApiName);
                c.OAuthClientId(rootConfiguration.AdminConfiguration.OidcSwaggerUIClientId);
                c.OAuthAppName(ApiConfigurationConsts.ApiName);

            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //app.UseMvc();
        }
    }
}
