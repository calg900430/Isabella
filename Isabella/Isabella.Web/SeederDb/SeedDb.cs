namespace Isabella.Web.SeederDb
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types.Enums;

    using Models.Entities;
    using Data;
    using Resources;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Seeder
    /// </summary>
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del Seeder
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="roleManager"></param>
        /// <param name="userManager"></param>
        /// <param name="configuration"></param>
        public SeedDb(DataContext dataContext, RoleManager<IdentityRole<int>> roleManager, 
        UserManager<User> userManager,
        IConfiguration configuration )
        {
           this._dataContext = dataContext;
           this._roleManager = roleManager;
           this._userManager = userManager;
           this._configuration = configuration;
        }

        /// <summary>
        /// Seeder
        /// </summary>
        /// <returns></returns>
        public async Task SeedAsync()
        {
            try
            {
                //Verifica si el archivo de recursos existe, sino lo manda a generar.
                /*var path = $"{Directory.GetCurrentDirectory()}\\Resources\\ResourcesFile.resources";
                var file_exist = File.Exists(path);
                if (!file_exist)
                CreateResourcesFile.GenerateResourceFileAsync(path);*/
                //Borra la base de datos si existe(Esto solo está habilitado en el momento de hacer pruebas, 
                //cuando todo este listo, debemos comentar esta linea)
                //await _dataContext.Database.EnsureDeletedAsync().ConfigureAwait(false);
                //Verifica si existe la base de datos, si no existe la crea.
                await _dataContext.Database.EnsureCreatedAsync().ConfigureAwait(false);

                /*Crea los roles del sistema*/
                string[] roles =
                {
                   Constants.RolesOfSystem[0],
                };
                foreach (string role in roles)
                {
                    if (await this._roleManager.FindByNameAsync(role).ConfigureAwait(false) != null)
                    continue;
                    IdentityRole<int> new_role = new IdentityRole<int> { Name = role, };
                    await this._roleManager.CreateAsync(new_role).ConfigureAwait(false);
                }

                /*Crea los usuarios admins y 2 usuarios clientes para pruebas.*/
                User[] users =
                {
                   new User
                   {
                        UserName = "yuniorvcg",
                        FirstName = "Yunior Vicente",
                        LastName = "Cabrera Gónzalez",
                        Email = "yvcabrerago@gmail.com",
                        PhoneNumber = "+5354678932",
                        Address = "Ave Tunas #23",
                   },
                   new User
                   {
                        UserName = "calg900430",
                        FirstName = "Carlos Antonio",
                        LastName = "Lamoth Guilarte",
                        Email = "calg900430@gmail.com",
                        PhoneNumber = "+5354069957",
                        Address = "Cuartel / 2 y 3 sur",
                   },
                   new User
                   {
                        UserName = "rafael535338",
                        FirstName = "Rafael",
                        LastName = "Taller",
                        Email = "taller.cursor@gmail.com",
                        PhoneNumber = "+5353388034",
                        Address = "Ferreiro",
                   },
                };
                if (!await this._dataContext.Users.AnyAsync().ConfigureAwait(false))
                {
                    string[] section_file_secret =
                    {
                       new string("admin1"),
                       new string("admin2"),
                       new string("admin3"),
                    };
                    /*Crea 2 usuarios admins.*/
                    for (int i = 0; i < users.Length; i++)
                    {
                        //Genera un codigo unico para cada usuario
                        users[i].IdForClaim = Guid.NewGuid();
                        users[i].DateCreated = DateTime.UtcNow;
                        users[i].DateUpdated = DateTime.UtcNow;
                        users[i].LastDateConnected = DateTime.UtcNow;
                        try
                        {
                            //Crea los usuarios admins
                            if (i < 3)
                            {
                                var identity_user = await this._userManager.CreateAsync(users[i],
                                this._configuration.GetSection("UsersDefault")
                                .GetSection(section_file_secret[i]).Value).ConfigureAwait(false);
                                if (identity_user.Succeeded != true)
                                {
                                    throw new InvalidOperationException($"No se creó el usuario {users[i].UserName}.");
                                }
                                var user_admins_notifications = new UserAdminsNotifications
                                {
                                    User = users[i],
                                };
                                if(i == 0)
                                user_admins_notifications.UserTelegramChatId = 955104865;
                                if(i == 1)
                                user_admins_notifications.UserTelegramChatId = 973260166;
                                if(i == 2)
                                user_admins_notifications.UserTelegramChatId = 1382606262;
                                await this._dataContext.UserAdminsNotifications.AddAsync(user_admins_notifications).ConfigureAwait(false);
                                await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                            }
                            //Asigna el role admin usuarios
                            if (i < 3)
                            {
                                var identity_role = await this._userManager.AddToRoleAsync(users[i], roles[0]).ConfigureAwait(false);
                                if (identity_role.Succeeded != true)
                                {
                                    throw new InvalidOperationException($"No se agregó el rol al usuario {users[i].UserName}.");
                                }
                            }
                            //Genera el Token de confirmación de correo.
                            var token = await _userManager.GenerateEmailConfirmationTokenAsync(users[i]).ConfigureAwait(false);
                            //Confirma el correo.
                            await _userManager.ConfirmEmailAsync(users[i], token).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"Se generó el siguiente error desde el Seeder: {ex.Message}");
                        }
                    }
                }

                //Crea los datos del restaurante.
                if(!await this._dataContext.Restaurants.AnyAsync().ConfigureAwait(false))
                {
                    var restaurant = new Restaurant
                    {
                        BeginHour = DateTime.UtcNow,
                        CloseHour = DateTime.UtcNow,
                        IsOpenRestaurant = false,
                        Name = "Isabella",
                    };
                    await this._dataContext.Restaurants
                    .AddAsync(restaurant)
                    .ConfigureAwait(false);
                    //Guarda los cambios
                    await this._dataContext.SaveChangesAsync();
                }

                /*Crea las categorias de los productos.*/
                List<Category> categorys = new List<Category>();
                if (!await this._dataContext.Categories.AnyAsync().ConfigureAwait(false))
                {
                    categorys = new List<Category>
                    {
                        new Category
                        {    
                           Id = 1,
                           Name = "Entrantes"
                        }, //0-Entrantes
                        new Category
                        {
                            Id = 2,
                            Name = "Platos Principales"
                        }, //1-Platos Principales
                        new Category
                        {
                             Id = 3,
                             Name = "Pizzas y Pastas"
                        }, //2-Pizzas y Pastas
                        new Category
                        {
                            Id = 4,
                            Name = "Postres"
                        }, //3-Postres
                        new Category
                        {
                             Id = 5,
                             Name = "Bebidas"
                        }, //4-Bebidas 
                    };     
                    await this._dataContext.Categories.AddRangeAsync(categorys).ConfigureAwait(false);
                    //Habilita la inserción de datos de forma explicita.
                    await this._dataContext.Database.OpenConnectionAsync().ConfigureAwait(false);
                    await this._dataContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Categories ON").ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                    await this._dataContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Categories OFF").ConfigureAwait(false);
                    await this._dataContext.Database.CloseConnectionAsync().ConfigureAwait(false);
                }

                /*Crea 8 agregados*/
                if (!await this._dataContext.Aggregates.AnyAsync().ConfigureAwait(false))
                {
                    var agregates = new List<Aggregate>
                    {
                        new Aggregate
                        {
                            Name = "Queso Blanco",
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Queso blanco de excelente calidad",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 30,
                            Stock = 120,
                        }, //0-Queso Blanco
                        new Aggregate     //1-Queso Gouda
                        {
                            Name = "Queso Gouda",
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Queso Gouda de importación.",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 45,
                            Stock = 150,
                        }, //1-Queso Gouda
                        new Aggregate
                        {
                            Name = "Jamón",
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Jamón de excelente calidad.",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 30,
                            Stock = 170,
                        }, //2-Jamóm
                        new Aggregate
                        {
                            Name = "Chorizo",
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Chorizo de excelente calidad.",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 30,
                            Stock = 250,
                        }, //3-Chorizo
                        new Aggregate
                        {
                            Name = "Aceitunas",
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Aceitunas de Goya.",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 45,
                            Stock = 150,
                        }, //4-Aceitunas
                        new Aggregate
                        {
                            Name = "Champiñón",
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Champiñón de importación.",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 40,
                            Stock = 250,
                        }, //5-Champiñón
                        new Aggregate
                        {
                            Name = "Atún",
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Atún de excelente calidad.",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 55,
                            Stock = 110,
                        }, //6-Atún
                        new Aggregate
                        {
                            Name = "Camarón",
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Camarón de producción nacional.",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 65,
                            Stock = 250,
                        }, //7-Camarón
                    };
                    //Asigna las imagenes a los agregos
                    agregates[0].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                           Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageQuesoBlanco)
                       },
                    };
                    agregates[1].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                           Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageQuesoGouda)
                       },
                    };
                    agregates[2].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                          Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageJamon)
                       },
                    };
                    agregates[3].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                          Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageChorizo)
                       },
                    };
                    agregates[4].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                          Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageAceitunas)
                       },
                    };
                    agregates[5].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                          Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageSetas)
                       },
                    };
                    agregates[6].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                          Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageAtun)
                       },
                    };
                    agregates[7].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                          Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageCamarones)
                       },
                    };
                    await this._dataContext.Aggregates
                    .AddRangeAsync(agregates)
                    .ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync()
                    .ConfigureAwait(false);
                }

                /*Crea 8 productos*/
                if (!await this._dataContext.Products.AnyAsync().ConfigureAwait(false))
                {
                    var products = new List<Product>
                    {
                        new Product
                        {
                           Name = "Ensalda Fría",
                           Category = categorys[0],
                           Description = "Ensalada Fría con queso, jamón y aceitunas.",
                           Stock = 100,
                           IsAvailabe = true,
                           Price = 55,
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           LastBuy = DateTime.UtcNow,
                           SupportAggregate = false,
                           //Average = 5,
                        },
                        new Product
                        {
                           Name = "Coco Glasé",
                           Category = categorys[3],
                           Description = "Excelente postre de coco.",
                           Stock = 65,
                           IsAvailabe = true,
                           Price = 120,
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           LastBuy = DateTime.UtcNow,
                           SupportAggregate = false,
                           SubCategories = new List<SubCategory>
                           {
                               new SubCategory
                               {
                                  Name = "Ración Doble",
                                  Price = 240,
                                  Description = "El postre coco glasé, en una variante doble.",
                                  IsAvailable = true,
                               }
                           },
                           //Average = 5,
                         },
                        new Product
                        {
                           Name = "Bistec de Cerdo",
                           Category = categorys[1],
                           Description = "Bistec de Cerdo, un plato recomendado por la casa.",
                           Stock = 150,
                           IsAvailabe = true,
                           Price = 200,
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           LastBuy = DateTime.UtcNow,
                           SupportAggregate = false,
                           SubCategories = new List<SubCategory>
                           {
                               new SubCategory
                               {
                                  Name = "Ración Doble",
                                  Price = 400,
                                  Description = "El bistec de cerdo, en mayor proporción.",
                                  IsAvailable = true,
                               }
                           },
                           //Average = 4,
                        },
                        new Product
                        {
                           Name = "Camarón Grillé",
                           Category = categorys[1],
                           Description = "Excelente variante de camarones, recomendado por la casa.",
                           Stock = 180,
                           IsAvailabe = true,
                           Price = 250,
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           LastBuy = DateTime.UtcNow,
                           SupportAggregate = false,
                           //Average = 4,
                        },
                        new Product
                        {
                           Name = "Malta Holland",
                           Category = categorys[4],
                           Description = "Malta de excelente calidad",
                           Stock = 240,
                           IsAvailabe = true,
                           Price = 45,
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           LastBuy = DateTime.UtcNow,
                           SupportAggregate = false,
                           //Average = 4,
                        },
                        new Product
                        {
                           Name = "Vino Blanco Constelación",
                           Category = categorys[4],
                           Description = "Vino recomendado para cenas especiales.",
                           Stock = 240,
                           IsAvailabe = true,
                           Price = 45,
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           LastBuy = DateTime.UtcNow,
                           SupportAggregate = false,
                           //Average = 4,
                        },
                        new Product
                        {
                           Name = "Espaguetti con Jamon",
                           Category = categorys[2],
                           Description = "Espagueti con jamón, queso y aceitunas.",
                           Stock = 80,
                           IsAvailabe = true,
                           Price = 130,
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           LastBuy = DateTime.UtcNow,
                           SupportAggregate = true,
                           SubCategories = new List<SubCategory>
                           {
                               new SubCategory
                               {
                                  Name = "Queso Gouda",
                                  Price = 165,
                                  Description = "Espaguettis elaborados con queso gouda.",
                                  IsAvailable = true,
                               }
                           },
                           //Average = 5,
                        },
                        new Product
                        {
                           Name = "Pizza con Camarones",
                           Category = categorys[2],
                           Description = "Esta es la mejor pizza de la casa.",
                           Stock = 140,
                           IsAvailabe = true,
                           Price = 155,
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           LastBuy = DateTime.UtcNow,
                           SupportAggregate = true,
                           SubCategories = new List<SubCategory>
                           {
                               new SubCategory
                               {
                                  Name = "Ración Familiar",
                                  Price = 310,
                                  Description = "Pizza para la familia.",
                                  IsAvailable = true,
                               },
                               new SubCategory
                               {
                                  Name = "Ración Familiar con Queso Gouda",
                                  Price = 350,
                                  Description = "Pizza para la familia elaborada con queso gouda.",
                                  IsAvailable = true,
                               },
                               new SubCategory
                               {
                                  Name = "Queso Gouda",
                                  Price = 190,
                                  Description = "Pizza de camarones elaborados con queso gouda.",
                                  IsAvailable = false,
                               }
                           },
                           //Average = 5,
                        },
                    };
                    //Agrega las imagenes del producto1(Ensalada Fria)
                    products[0].Images = new List<ImageProduct>
                    {
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageEnsaldadaFria)
                       }
                    };
                    //Agrega las imagenes del producto2(Coco Glasé)
                    products[1].Images = new List<ImageProduct>
                    {
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageCocoGlaset)
                       },
                    };
                    //Agrega las imagenes del producto3(Bistec de Cerdo)
                    products[2].Images = new List<ImageProduct>
                    {
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageBistecCerdo)
                       },
                    };
                    //Agrega las imagenes del producto4(Camarón Grille)
                    products[3].Images = new List<ImageProduct>
                    {
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageCamaronGrille)
                       },
                    };
                    //Agrega las imagenes del producto5(Malta Holland)
                    products[4].Images = new List<ImageProduct>
                    {
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageMaltaHolland)
                       },
                    };
                    //Agrega las imagenes del producto6(Vino Blanco Constelación)
                    products[5].Images = new List<ImageProduct>
                    {
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageVinoBlanco)
                       },
                    };
                    //Agrega las imagenes del producto7(Espagatti con jamon)
                    products[6].Images = new List<ImageProduct>
                    {
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageEspaguettiJamon)
                       },
                    };
                    //Agrega las imagenes del producto8(Pizza con camarones)
                    products[7].Images = new List<ImageProduct>
                    {
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImagePizzaCamarones)
                       },
                    };
                    await this._dataContext.Products.AddRangeAsync(products).ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
               throw ex;
            }
        }
    }
}
