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
    using Models;
    using Data;
   

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

                /*Crea los productos de agrego*/
                if (!await this._dataContext.ProductAggregates.AnyAsync().ConfigureAwait(false))
                {
                    var products_agregate = new List<ProductAggregate>
                    {
                        new ProductAggregate
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
                        }, //0-Queso
                        new ProductAggregate     //1-Queso Gouda
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
                        new ProductAggregate
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
                        new ProductAggregate
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
                        new ProductAggregate
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
                        new ProductAggregate
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
                        new ProductAggregate
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
                        new ProductAggregate
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
                    /*products_agregate[0].ImageProductAggregates = new List<ImageProductAggregate>
                    {
                       new ImageProductAggregate
                       {
                           Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/agregados/queso_blanco0.jpg"),
                       },
                    };
                    products_agregate[1].ImageProductAggregates = new List<ImageProductAggregate>
                    {
                       new ImageProductAggregate
                       {
                          Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/agregados/queso_gouda0.jpg"),
                       },
                    };
                    products_agregate[2].ImageProductAggregates = new List<ImageProductAggregate>
                    {
                       new ImageProductAggregate
                       {
                          Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/agregados/jamon0.jpg"),
                       },
                    };
                    products_agregate[3].ImageProductAggregates = new List<ImageProductAggregate>
                    {
                       new ImageProductAggregate
                       {
                         Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/agregados/chorizo0.jpg"),
                       },
                    };
                    products_agregate[4].ImageProductAggregates = new List<ImageProductAggregate>
                    {
                       new ImageProductAggregate
                       {
                         Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/agregados/aceitunas0.jpg"),
                       },
                    };
                    products_agregate[5].ImageProductAggregates = new List<ImageProductAggregate>
                    {
                       new ImageProductAggregate
                       {
                         Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/agregados/champiñon0.jpg"),
                       },
                    };
                    products_agregate[6].ImageProductAggregates = new List<ImageProductAggregate>
                    {
                       new ImageProductAggregate
                       {
                         Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/agregados/atun0.jpg"),
                       },
                    };
                    products_agregate[7].ImageProductAggregates = new List<ImageProductAggregate>
                    {
                       new ImageProductAggregate
                       {
                         Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/agregados/camaron0.jpg"),
                       },
                    };*/
                    await this._dataContext.ProductAggregates
                    .AddRangeAsync(products_agregate)
                    .ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync()
                    .ConfigureAwait(false);
                }

                /*Crea 6 productos estandar*/
                if (!await this._dataContext.ProductsStandards.AnyAsync().ConfigureAwait(false))
                {
                    var products_standars = new List<ProductStandard>
                    {
                        //Entrantes
                        new ProductStandard
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
                        new ProductStandard
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
                        new ProductStandard
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
                        new ProductStandard
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
                        new ProductStandard
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
                        new ProductStandard
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
                    };
                    //Agrega las imagenes del producto1(Ensalada Fria)
                    products_standars[0].ImageProductStandards = new List<ImageProductStandard>
                    {
                       new ImageProductStandard
                       {
                         Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/entrantes/ensalada_fria1.jpg"),
  
                       },
                       new ImageProductStandard
                       {
                         Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/entrantes/ensalada_fria2.jpg"),
                       }
                    };
                    //Agrega las imagenes del producto2(Coco Glasé)
                    products_standars[1].ImageProductStandards = new List<ImageProductStandard>
                    {
                       new ImageProductStandard
                       {
                          Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/postres/coco_glaset1.jpg"),
                       },
                    };
                    //Agrega las imagenes del producto3(Bistec de Cerdo)
                    products_standars[2].ImageProductStandards = new List<ImageProductStandard>
                    {
                       new ImageProductStandard
                       {
                          Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/platos_principales/bisteccerdo1.jpg"),
                       },
                       new ImageProductStandard
                       {
                          Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/platos_principales/bisteccerdo2.jpg"),
                       },
                       new ImageProductStandard
                       {
                          Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/platos_principales/bisteccerdo3.jpg"),
                       },
                    };
                    //Agrega las imagenes del producto4(Camarón Grille)
                    products_standars[3].ImageProductStandards = new List<ImageProductStandard>
                    {
                       new ImageProductStandard
                       {
                         
                         Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/mariscos/camaron_grille1.jpg"),
                       },
                       new ImageProductStandard
                       {

                         Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/mariscos/camaron_grille2.jpg"),
                       },
                       new ImageProductStandard
                       {

                         Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/mariscos/camaron_grille3.jpg"),
                       },
                    };
                    //Agrega las imagenes del producto5(Malta Holland)
                    products_standars[4].ImageProductStandards = new List<ImageProductStandard>
                    {
                       new ImageProductStandard
                       {
                           Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/bebidas/malta_holland1.jpg"),
                       },
                    };
                    //Agrega las imagenes del producto6(Vino Blanco Constelación)
                    products_standars[5].ImageProductStandards = new List<ImageProductStandard>
                    {
                       new ImageProductStandard
                       {
                         Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/vinos_licores/vino_blanco1.jpg"),
                       },
                    };
                    await this._dataContext.ProductsStandards.AddRangeAsync(products_standars).ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                }
                
                /*Crea 2 productos especiales*/
                if(!await this._dataContext.ProductsSpecials.AnyAsync().ConfigureAwait(false))
                {
                    var product_special = new List<ProductSpecial>
                    {
                       new ProductSpecial
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
                       new ProductSpecial
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
                    //Agrega las imagenes del producto7(Espagatti con jamon)
                    product_special[0].ImageProductSpecials = new List<ImageProductSpecial>
                    {
                       new ImageProductSpecial
                       {
                         
                         Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/pastas/espaguetti_jamon1.jpg"),
                       },
                       new ImageProductSpecial
                       {
                         Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/pastas/espaguetti_jamon2.jpg"),
                       },
                    };
                    //Agrega las imagenes del producto8(Pizza con camarones)
                    product_special[1].ImageProductSpecials = new List<ImageProductSpecial>
                    {
                       new ImageProductSpecial
                       {
                          Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/pizzas/pizza_camarones1.jpg"),
                       },
                       new ImageProductSpecial
                       {
                         Image = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/images/images_products/pizzas/pizza_camarones2.jpg"),
                       },
                    };
                    await this._dataContext.ProductsSpecials.AddRangeAsync(product_special).ConfigureAwait(false);
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
