using IdentityServer4.Anonnymous;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TestBackend
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            _ = services.AddIdentityServer(options =>
            {
                options.Discovery.CustomEntries.Add(Constants.EndpointNames.AnonnymousAuthorization, $"~{Constants.EndpointPaths.AnonnymousAuthorizationEndpoint}");
            })
               .AddDeveloperSigningCredential()        //This is for dev only scenarios when you don’t have a certificate to use.
               .AddInMemoryApiScopes(Config.ApiScopes)
               .AddInMemoryClients(Config.Clients)
               .AddAnonnymousAuthorization(_config, _config.GetConnectionString("DefaultConnection"))
               .AddTwilioProviders(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
