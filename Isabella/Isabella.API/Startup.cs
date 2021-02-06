namespace Isabella.API
{
    using System;
    using System.IO;
    using System.Reflection;
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
    using Extras;
    using Models;
    using RepositorysModels;
    using SeederDb;
    using ServicesModels;
    using ServicesControllers;

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
                    Title = "Isabella",
                    Version = "v1",
                });
                //Accede al archivo XML que genera VS 19, de la documentación de nuestro programa
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //Swagger accede a la documentación
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
           
            //Agrega el servicio del Seeder
            services.AddTransient<SeedDb>();

            //Agrega el servicio para el manejo de correos.
            services.AddScoped<MailServiceModel>();

            //Agrega el servicio para el manejo de archivos que envia el usuario
            services.AddScoped<UploadFileServiceModel>();

            //Agrega el servicio de pruebas para borrar los elementos de la base de datos.
            services.AddTransient<AllDeleteDatabaseExecuteSeederServiceModel>();

            //Agrega los servicios para el manejo de usuarios
            services.AddScoped<IUserRepositoryModel, UserServiceModel>();
            services.AddScoped<UserServiceController>();

            //Agrega los servicios para el manejo de los productos standard.
            services.AddScoped<IProductStandardRepositoryModel, ProductStandardServiceModel>();
            services.AddScoped<ProductStandardServiceController>();

            //Agrega los servicios para el manejo de los productos special.
            services.AddScoped<IProductSpecialRepositoryModel, ProductSpecialServiceModel>();
            services.AddScoped<ProductSpecialServiceController>();

            //Agrega los servicios para el manejo de los productos aggregate.
            services.AddScoped<IProductAggregateRepositoryModel, ProductAggregateServiceModel>();
            services.AddScoped<ProductAggregateServiceController>();

            //Agrega los servicios para el manejo de las categorias.
            services.AddScoped<ICategoryProductStandardRepositoryModel, CategoryProductStandardServiceModel>();
            services.AddScoped<ICategoryProductSpecialRepositoryModel, CategoryProductSpecialServiceModel>();
            services.AddScoped<ICategoryProductAggregateRepositoryModel, CategoryProductAggregateServiceModel>();

            //Agrega los servicios para el manejo del carrito de compras
            services.AddScoped<ICarShopRepositoryModel, CarShopServiceModel>();
            services.AddScoped<CarShopServiceController>();

            //Agrega los servicios para el manejo de los códigos de verificación
            services.AddScoped<ICodeIdentificationModel, CodeIdentificationServiceModel>();
            services.AddScoped<CodeIdentificationServiceController>();

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
                c.SwaggerEndpoint("/swagger/api/swagger.json", "Isabella");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
