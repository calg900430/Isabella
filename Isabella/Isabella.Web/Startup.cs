namespace Isabella.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Reflection;
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpsPolicy;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;

    using AutoMapper;

    using Data;
    using Models.Entities;
    using Extras;
    using SeederDb;
    using Repositorys;
    using Repositorys.API;
    using Services;
    using Services.API;
    using Hubs;
    using Hubs.Services;
   
    /// <summary>
    /// StartUp
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructor Starup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            //Agrega y configura el servicio para el manejo de usuarios y roles
            services.AddIdentity<User, IdentityRole<int>>(cfg =>
            {
                cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                cfg.SignIn.RequireConfirmedEmail = true;
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequiredLength = 8;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<DataContext>();
           
            //Agrega y configura el servicio para conectarnos a SQL Server
            var connectionstring = Constants.GetStringConnectionSQLServer(Configuration.GetSection("SQLServerLocal"));
            //var connectionstring = Constants.GetStringConnectionSQLServer(Configuration.GetSection("SQLServer"));
            services.AddDbContext<DataContext>(c =>
            {
                c.UseSqlServer(connectionstring);
            });

            //Agrega y configura el servicio del de Swagger
            services.AddSwaggerGen(c =>
            {
                //Define uno o más documentos para crea el generador Swagger
                c.SwaggerDoc("api", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "InkNation",
                    Version = "v1",
                });
                //Accede al archivo XML que genera VS 19, de la documentación de nuestro programa
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //Swagger accede a la documentación
                c.IncludeXmlComments(xmlPath);
                var scheme_security = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                };
                c.AddSecurityDefinition("Bearer", scheme_security);
            });

            //Agrega y congigura el esquema de autenticación(Json Web Tokens)
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddCookie()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = this.Configuration["AppSettings:Issuer"],
                    ValidAudience = this.Configuration["AppSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(Configuration
                  .GetSection("AppSettings:Token").Value)),
                };
                //Para obtener el Token de las solicitudes de SignalR ya que el Token para 
                //SignalR se envia  como un parametro de consulta y no en un encabezado.
                /*options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                   {

                   }
                };*/
            });

            //Agrega el servicio del Seeder
            services.AddTransient<SeedDb>();

            //Agrega el AutoMapper
            services.AddAutoMapper(typeof(Startup));

            //Agrea y configura el servicio de SignalR
            services.AddSignalR();

            //Agrega el servicio para el manejo de usuarios
            services.AddScoped<IUserRepositoryAPI, UserServiceAPI>();
           
            //Agrega el servicio para el envio de correos
            services.AddScoped<IMailRepositoryCommon, MailServiceCommon>();

            //Agrega el servicio para el manejo de archivos que envia el usuario
            services.AddScoped<UploadFileServiceCommon>();

            //Agrega el servicio para el manejo del diccionario que almacena las conexiones del Hub
            //Utilizamos el patrón Singleton para mantener una sola instancia durante toda la ejecución
            //del programa.
            services.AddSingleton<DicctionaryConnectedHubService>();

            //Agrega el servicio para el manejo de los productos standard.
            services.AddScoped<IProductStandardRepositoryAPI, ProductStandardServiceAPI>();

            //Agrega el servicio para el manejo de los productos especiales.
            services.AddScoped<IProductSpecialRepositoryAPI, ProductSpecialServiceAPI>();

            //Agrega el servicio para el manejo de los quesos.
            services.AddScoped<IProductTypeCheeseRepositoryAPI, ProductTypeCheeseServiceAPI>();

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //Agrega el Middleware del Swagger(JSON)
            app.UseSwagger();
            //Usar SwaggerUI(Sitio Web de Swagger que se construye desde el JSON)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/api/swagger.json", "InkNation");
            });
            app.UseEndpoints(endpoints =>
            {
                //Agrega el endpoint para el Hub de las notificaciones
                endpoints.MapHub<NotificationsHub>("/notificationhub");
                endpoints.MapControllerRoute( name: "default",pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
