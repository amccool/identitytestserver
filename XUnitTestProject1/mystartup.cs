using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace XUnitTestProject1
{
    public class myStartup
    {
        private readonly JwtBearerOptions _options;
        public myStartup(IConfiguration configuration, JwtBearerOptions options)
        {
            _options = options;
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services
            .AddAuthentication(GetAuthenticationOptions)
            .AddJwtBearer(GetJwtBearerOptions);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app
            .UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(ep =>
            {
                ep.MapControllers();
            });
        }

        private static void GetAuthenticationOptions(AuthenticationOptions options)
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }

        private void GetJwtBearerOptions(JwtBearerOptions options)
        {
            //BTW trying to just copy the ojbect doesnt work
            //options = _options; 
            //need to deep copy
            options.Authority = _options.Authority;
            options.Audience = _options.Audience;
            options.RequireHttpsMetadata = _options.RequireHttpsMetadata;
            options.BackchannelHttpHandler = _options.BackchannelHttpHandler;

        }
    }
}