using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Logging;
using Microsoft.Identity.Web;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using permaAPI.services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using permaAPI.Data.Contexts;
using Microsoft.IdentityModel.Tokens;

namespace permaAPI
{
    public class Startup
    {
        protected IServiceProvider? ApplicationServices { get; set; } = null;
        string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("permaportal")));



            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddMicrosoftIdentityWebApi(options =>
          {
              Configuration.Bind("AzureAdB2C", options);

              options.TokenValidationParameters.NameClaimType = "name";
          },
          options => { Configuration.Bind("AzureAdB2C", options); });


            services.AddCors(option =>
            {
                option.AddPolicy(name: MyAllowSpecificOrigins,
                                   policy =>
                                   {
                                       policy.WithOrigins("http://localhost:4200",
                                       "http://localhost:3200", "https://portal.permarisk.gov",
                                                           "https://permaquestionnaire.azurewebsites.net").AllowAnyHeader()
                                                  .AllowAnyMethod(); ;
                                   });

            });

            services.Configure<MicrosoftIdentityOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Events.OnTokenValidated = async context =>
                {
                    if (ApplicationServices != null && context.Principal != null)
                    {
                        using var scope = ApplicationServices.CreateScope();
                        context.Principal = await scope.ServiceProvider
                            .GetRequiredService<MsGraphClaimsTransformation>()
                            .TransformAsync(context.Principal);
                    }
                };
            });

            services.AddScoped<EmailService, EmailService>();

            services.AddScoped<MsGraphService>();
            services.AddScoped<MsGraphClaimsTransformation>();
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddScoped<IMembersService, MembersService>();
            services.AddScoped<ILinesService, LinesService>();
            services.AddScoped<ICategoriesService, CategoriesService>();
            services.AddScoped<IContactsService, ContactsService>();
            services.AddScoped<IApplicationFileService, applicationFileService>();
            services.AddSingleton<IApplicationTypeService, ApplicationTypeService>();
            services.AddScoped<IApplicationsService, ApplicationsService>();
            services.AddScoped<services.IApplicationSectionService, permaAPI.services.ApplicationSectionService>();
            services.AddScoped<permaAPI.services.IApplicationElementsService, permaAPI.services.ApplicationElementsService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "permaAPI", Version = "v1" });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger(c =>
            {
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "permaAPI v1");
                c.DisplayRequestDuration(); // Show request duration in Swagger UI
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;

            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseFileServer();
            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
