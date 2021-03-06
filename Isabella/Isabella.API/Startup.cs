namespace Isabella.API
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;

    using Common.RepositorysDtos;
    using Data;
    using Models.Entities;
    using SeederDb;
    using ServicesControllers;
    using Helpers;
    using Helpers.RepositoryHelpers;
    using AutoMapper;
    using Isabella.API.Hubs;
    using Isabella.API.Hubs.Services;


    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="webHostEnvironment"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// ConfigureService
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// WebHostEnvironment
        /// </summary>
        public IWebHostEnvironment WebHostEnvironment { get; }


        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
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
                cfg.SignIn.RequireConfirmedAccount = true;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<DataContext>();

            //Agrega y configura el servicio para conectarnos a SQL Server
            //var connectionstring = Constants.GetStringConnectionSQLServer(Configuration.GetSection("SQLServerLocal"));
            var connectionstring = Constants.GetStringConnectionSQLServer(Configuration.GetSection("SQLServer"));
            services.AddDbContext<DataContext>(c =>
            {
                //c.UseLazyLoadingProxies() //Para poder usar carga diferida
                c.UseSqlServer(connectionstring);
            });

            //Agrega y configura el servicio del de Swagger
            services.AddSwaggerGen(c =>
            {
                //Define uno o m?s documentos para crea el generador Swagger
                c.SwaggerDoc("api", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Isabella",
                    Version = "v1",
                });
                //Accede al archivo XML que genera VS 19, de la documentaci?n de nuestro programa
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //Swagger accede a la documentaci?n
                c.IncludeXmlComments(xmlPath);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
                {
                    Description = "JWT Authorization header using the Bearer scheme.Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                  {
                    new  OpenApiSecurityScheme
                    {
                       Reference =  new  OpenApiReference
                       {
                          Type = ReferenceType.SecurityScheme,
                          Id =  "Bearer"
                       }
                    },
                    new string[] {}
                  }
                });
            });

            //Agrega y congigura el esquema de autenticaci?n(Json Web Tokens)
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddCookie()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = this.Configuration["AppSettings:Issuer"],
                   ValidAudience = this.Configuration["AppSettings:Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding
                   .ASCII.GetBytes(Configuration
                   .GetSection("AppSettings:Token").Value)),
                };
                //Para obtener el Token de las solicitudes de SignalR ya que el Token para 
                //SignalR se envia  como un parametro de consulta y no en un encabezado.
                options.Events = new JwtBearerEvents
                {
                   OnMessageReceived = context =>
                   {
                       var accessToken = context.Request.Query["access_token"];
                       //Si la solicitud es enviada a la Hub
                       var path = context.Request.Path;
                       if(!string.IsNullOrEmpty(accessToken) && 
                       path.StartsWithSegments("/hub_notifications_admins"))
                       {
                           context.Token = accessToken;
                       }
                       return Task.CompletedTask;
                   }
                };
            });
            /*.AddGoogle(go =>
            {
                IConfigurationSection googleAuth = Configuration.GetSection("LogingGoogle");
                go.ClientId = googleAuth["GoogleClientId"];
                go.ClientSecret = googleAuth["GoogleClientSecret"];
                go.SaveTokens = true;
            });
            .AddApple(apple => 
            {
                IConfiguration appleAuth = Configuration.GetSection("LoginApple");
                apple.ClientId = appleAuth["AppleClientId"];
                apple.TeamId = appleAuth["AppleTeamId"];
                apple.KeyId = appleAuth["AppleKeyId"];
                apple.SaveTokens = true;
            })
            .AddFacebook(fc => 
            {
                IConfiguration facebookAuth = Configuration.GetSection("LoginFacebook");
                fc.ClientId = facebookAuth["FacebookClientId"];
                fc.ClientSecret = facebookAuth["FacebookClientSecret"];
                fc.SaveTokens = true;
            })
            .AddTwitter(tw => 
            {
               IConfiguration twitterAuth = Configuration.GetSection("LoginTwitter");
               tw.ConsumerKey = twitterAuth["TwitterClientId"];
               tw.ConsumerSecret = twitterAuth["TwitterClientSecret"];
               tw.SaveTokens = true;
            });*/


            //Agrega el servicio para el manejo del diccionario que almacena las conexiones del Hub
            //Utilizamos el patr?n Singleton para mantener una sola instancia durante toda la ejecuci?n
            //del programa.
            services.AddSingleton<DicctionaryConnectedHub>();

            //Agrega el servicio del AutoMapper
            services.AddAutoMapper(typeof(Startup));

            //Agrega el servicio del Seeder
            services.AddTransient<SeedDb>();

            //Agrega el servicio para el manejo de correos.
            services.AddScoped<MailHelper>();

            //Agrega el servicio para el manejo de archivos que envia el usuario
            services.AddScoped<UploadFileHelper>();

            //Agrega los servicios para el manejo de usuarios.
            services.AddScoped<UserServiceController>();
            services.AddScoped<IUserRepositoryHelper, IUserHelper>();
            services.AddScoped<ServiceGenericHelper<User>>();
          
            //Agrega los servicios para el manejo de los productos.
            services.AddScoped<ServiceGenericHelper<Product>>();
            services.AddScoped<ServiceGenericHelper<ImageProduct>>();
            services.AddScoped<ProductServiceController>();

            //Agrega los servicios para el manejo de los agregados.
            services.AddScoped<AggregateServiceController>();
            services.AddScoped<ServiceGenericHelper<Aggregate>>();
            services.AddScoped<ServiceGenericHelper<ImageAggregate>>();

            //Agrega los servicios para el manejo de las subcategorias.
            services.AddScoped<CategoryServiceController>();
            services.AddScoped<ServiceGenericHelper<SubCategory>>();

            //Agrega los servicios para el manejo de las categorias.
            services.AddScoped<SubCategoryServiceController>();
            services.AddScoped<ServiceGenericHelper<Category>>();

            //Agrega los servicios para el manejo del carrito de compras
            services.AddScoped<CartShopServiceController>();
            services.AddScoped<ServiceGenericHelper<CartShop>>();
            services.AddScoped<ServiceGenericHelper<CantAggregate>>();
            services.AddScoped<ServiceGenericHelper<ProductCombined>>();

            //Agrega el servicio para el manejo de las ordenes
            services.AddScoped<OrderServiceController>();
            services.AddScoped<ServiceGenericHelper<Order>>();
            services.AddScoped<ServiceGenericHelper<OrderDetail>>();

            //Agrega el servicio para el manejo de las notificaciones
            services.AddScoped<ServiceGenericHelper<UserAdminsNotifications>>();
            services.AddScoped<ServiceGenericHelper<NotificationPendients>>();

            //Agrega el servicio de SignalR
            services.AddSignalR(options => 
            { 
               
            });
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
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //Agrega el Middleware del Swagger(JSON)
            app.UseSwagger();
            //Usar SwaggerUI(Sitio Web de Swagger que se construye desde el JSON)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/api/swagger.json", "Isabella");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationsHub>("/hub_notifications_admins", options => 
                {
                    //options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
                    //options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;

                });
                endpoints.MapControllers();
            });
        }
    }
}
