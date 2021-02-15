namespace Isabella.API.SeederDb
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using Common.Extras;
    using Models.Entities;
    using Data;
    using ElementsForResourceFile;

    /// <summary>
    /// Seeder
    /// </summary>
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<User> _userManager;
     
        /// <summary>
        /// Constructor del Seeder
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="roleManager"></param>
        /// <param name="userManager"></param>
        public SeedDb(DataContext dataContext, RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager)
        {
           this._dataContext = dataContext;
           this._roleManager = roleManager;
           this._userManager = userManager;     
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
                var file_exist = File.Exists($"{Directory.GetCurrentDirectory()}//ResourceFile.resources");
                if (!file_exist)
                ManagerResourceFile.GenerateResourceFileAsync();
                //Borra la base de datos si existe(Esto solo está habilitado en el momento de hacer pruebas, 
                //cuando todo este listo, debemos comentar esta linea)
                //await _dataContext.Database.EnsureDeletedAsync().ConfigureAwait(false);
                //Verifica si existe la base de datos, si no existe la crea.
                await _dataContext.Database.EnsureCreatedAsync().ConfigureAwait(false);

                /*Crea los roles del sistema*/
                string[] roles = { "admin"};
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
                };
                if (!await this._dataContext.Users.AnyAsync().ConfigureAwait(false))
                {
                    string[] password =
                    {
                      new string("Yvcg.*456789*"),
                      new string("MW3_MacTavish"),
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
                            if (i < 2)
                            {
                                var identity_user = await this._userManager.CreateAsync(users[i], password[i]).ConfigureAwait(false);
                                if (identity_user.Succeeded != true)
                                {
                                    throw new InvalidOperationException($"No se creó el usuario {users[i].UserName}.");
                                }
                            }
                            //Asigna el role admin usuarios
                            if (i < 2)
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

                /*Crea las categorias de los productos.*/
                List<Category> categorys = new List<Category>();
                if (!await this._dataContext.Categories.AnyAsync().ConfigureAwait(false))
                {
                    categorys = new List<Category>
                    {
                        new Category
                        {            
                           Name = "Entrantes"
                        }, //0-Entrantes
                        new Category
                        {
                            Name = "Postres"
                        }, //1-Postres
                        new Category
                        {
                             Name = "Platos Principales"
                        }, //2-Platos Principales
                        new Category
                        {
                           Name = "Mariscos"
                        }, //3-Mariscos
                        new Category
                        {
                             Name = "Bebidas"
                        }, //4-Bebidas
                        new Category
                        {
                             Name = "Vinos y Licores"
                        }, //5-Vinos y Licores
                        new Category
                        {
                             Name = "Pizzas"
                        }, //6-Pizzas
                        new Category
                        {
                             Name = "Pastas"
                        }, //7-Pastas
                        new Category
                        {
                             Name = "Quesos"
                        }, //8-Quesos
                        new Category
                        {
                             Name = "Frutas"
                        }, //9-Frutas
                        new Category
                        {
                             Name = "Setas"
                        }, //10-Setas
                        new Category
                        {
                             Name = "Embutidos"
                        }, //11-Embutidos
                        new Category
                        {
                             Name = "Pescados"
                        }, //12-Pescados
                    };
                    await this._dataContext.Categories.AddRangeAsync(categorys).ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                }

                /*Crea 8 agregados*/
                if (!await this._dataContext.Aggregates.AnyAsync().ConfigureAwait(false))
                {
                    var products_agregate = new List<Aggregate>
                    {
                        new Aggregate
                        {
                            Name = "Queso",
                            Category = categorys[8],
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
                            Category = categorys[8],
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
                            Category = categorys[11],
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
                            Category = categorys[11],
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
                            Category = categorys[9],
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
                            Category = categorys[10],
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
                            Category = categorys[12],
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
                            Category = categorys[3],
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
                    /*products_agregate[0].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                           Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageQuesoBlanco)
                       },
                    };*/
                    products_agregate[1].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                           Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageQuesoGouda)
                       },
                    };
                    products_agregate[2].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                          Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageJamon)
                       },
                    };
                    products_agregate[3].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                          Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageChorizo)
                       },
                    };
                    products_agregate[4].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                          Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageAceitunas)
                       },
                    };
                    products_agregate[5].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                          Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageSetas)
                       },
                    };
                    products_agregate[6].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                          Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageAtun)
                       },
                    };
                    products_agregate[7].Images = new List<ImageAggregate>
                    {
                       new ImageAggregate
                       {
                          Image = GetValueResourceFile.GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageCamarones)
                       },
                    };
                    await this._dataContext.Aggregates
                    .AddRangeAsync(products_agregate)
                    .ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync()
                    .ConfigureAwait(false);
                }

                /*Crea 8 productos*/
                if (!await this._dataContext.Products.AnyAsync().ConfigureAwait(false))
                {
                    var products = new List<Product>
                    {
                        //Entrantes
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
                        },
                        new Product
                         {
                           Name = "Coco Glasé",
                           Category = categorys[1],
                           Description = "Excelente postre de coco.",
                           Stock = 65,
                           IsAvailabe = true,
                           Price = 120,
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           LastBuy = DateTime.UtcNow,
                         },
                        new Product
                        {
                           Name = "Bistec de Cerdo",
                           Category = categorys[2],
                           Description = "Bistec de Cerdo, un plato recomendado por la casa.",
                           Stock = 150,
                           IsAvailabe = true,
                           Price = 200,
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           LastBuy = DateTime.UtcNow,
                        },
                        new Product
                        {
                           Name = "Camarón Grillé",
                           Category = categorys[3],
                           Description = "Excelente variante de camarones, recomendado por la casa.",
                           Stock = 180,
                           IsAvailabe = true,
                           Price = 250,
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           LastBuy = DateTime.UtcNow,
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
                        },
                        new Product
                        {
                           Name = "Vino Blanco Constelación",
                           Category = categorys[5],
                           Description = "Vino recomendado para cenas especiales.",
                           Stock = 240,
                           IsAvailabe = true,
                           Price = 45,
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           LastBuy = DateTime.UtcNow,
                        },
                        new Product
                        {
                           Name = "Espaguetti con Jamon",
                           Category = categorys[7],
                           Description = "Espagueti con jamón, queso y aceitunas.",
                           Stock = 80,
                           IsAvailabe = true,
                           Price = 130,
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           LastBuy = DateTime.UtcNow,
                        },
                        new Product
                        {
                           Name = "Pizza con Camarones",
                           Category = categorys[6],
                           Description = "Esta es la mejor pizza de la casa.",
                           Stock = 140,
                           IsAvailabe = true,
                           Price = 155,
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           LastBuy = DateTime.UtcNow,
                        },
                    };
                    //Agrega las imagenes del producto1(Ensalada Fria)
                    products[0].Images = new List<ImageProduct>
                    {
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageEnsaldadaFria1)
                       },
                       new ImageProduct
                       {
                           Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageEnsaldadaFria2)
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
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageBistecCerdo1)
                       },
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageBistecCerdo2)
                       },
                    };
                    //Agrega las imagenes del producto4(Camarón Grille)
                    products[3].Images = new List<ImageProduct>
                    {
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageCamaronGrille1)
                       },
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageCamaronGrille2)
                       },
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageCamaronGrille2)
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
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageEspaguettiJamon1)
                       },
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImageEspaguettiJamon2)
                       },
                    };
                    //Agrega las imagenes del producto8(Pizza con camarones)
                    products[7].Images = new List<ImageProduct>
                    {
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImagePizzaCamarones1)
                       },
                       new ImageProduct
                       {
                          Image = GetValueResourceFile
                          .GetValueResourceImage(GetValueResourceFile.KeyResourceImage.ImagePizzaCamarones2)
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
