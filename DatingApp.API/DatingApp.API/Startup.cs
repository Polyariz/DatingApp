using System.Net;
using System.Text;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; 
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;  
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc.Authorization; 

namespace DatingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
         public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x => {
                x.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });

        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            //UseMySql or UseSqlSever
             services.AddDbContext<DataContext>(x => {
                x.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
            });

             ConfigureServices(services); 
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        { 
            //services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
           //   UseMySql(Configuration.GetConnectionString("DefaultConnection")));//!!!!!!!!!!
            
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddRazorPages();//https://docs.microsoft.com/en-us/aspnet/core/migration/22-to-30?view=aspnetcore-2.2&tabs=visual-studio#update-routing-startup-code

            /*
            ReferenceLoopHandling.Error: By default Json.NET will error if a reference loop is encountered (otherwise the serializer will get into an infinite loop).
            ReferenceLoopHandling.Ignore: Json.NET will ignore objects in reference loops and not serialize them. The first time an object is encountered it will be serialized as usual but if the object is encountered as a child object of itself the serializer will skip serializing it.
            ReferenceLoopHandling.Serialize: This option forces Json.NET to serialize objects in reference loops. This is useful if objects are nested but not indefinitely. 
            */
            //OLD METHOD
            // .AddJsonOptions(options => 
            // {
            //      //options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; 
            // });
            
            //NEW
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }; 

            services.AddCors(); 
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddAutoMapper(typeof(DatingRepository).Assembly);
            services.AddTransient<Seed>();
            services.AddScoped<IAuthRepository, AuthRepository>(); 
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

                services.AddScoped<LogUserActivity>();
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
               app.UseExceptionHandler(builder => {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null) 
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts(); 
            }

            //app.UseHttpsRedirection();
            
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            // app.UseCors(x => x.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
           
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            
             app.UseRouting(); 

            //old variant
            // app.UseEndpoints(endpoints =>
            // {
            //     endpoints.MapControllers(); 
            //     endpoints.MapRazorPages(); 
            //     endpoints.MapControllerRoute("spa-fallback", "{controller=Fallback}/{action=Index}");
            // });

            app.UseEndpoints(endpoints =>
            {  
                endpoints.MapFallbackToController(action:"Index", controller:"Fallback");
                // endpoints.MapControllerRoute("default", "{controller=Fallback}/{action=Index}/{id?}");
                endpoints.MapRazorPages(); 
            });
          
          /* 
            //https://metanit.com/sharp/aspnetcore/2.2.php
             // обработка маршрутов, которые не сопоставлены с ресурсам ранее
             //Таким образом, для всех запросов, которые не сопоставлены с ресурсами в приложении, будет отправляться файл wwwroot/index.html.
            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html"; 
                await context.Response.SendFileAsync(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"));
            });
            */
        }
    }
}
