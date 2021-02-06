namespace Isabella.Web.SeederDb
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
                await _dataContext.Database.EnsureDeletedAsync().ConfigureAwait(false);
                //Verifica si existe la base de datos, si no existe la crea.
                await _dataContext.Database.EnsureCreatedAsync().ConfigureAwait(false);

                /*Crea los roles del sistema*/
                string[] roles = { "admin", "owner", "client"};
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
                        UserName = "alexisjesus92",
                        FirstName = "Alexis Jesús",
                        LastName = "Caballero Salazar",
                        Email = "alexisjesus92@gmail.com",
                        PhoneNumber = "+5355000000",
                        Address = "23 y J",
                      },
                   new User
                      {
                        UserName = "juliosantander92",
                        FirstName = "Julio Armando",
                        LastName = "Suárez Santander",
                        Email = "juliosantander92@gmail.com",
                        PhoneNumber = "+5355000000",
                        Address = "23 y M",
                      },
                };
                if (!await this._dataContext.Users.AnyAsync().ConfigureAwait(false))
                {
                    string[] password =
                    {
                      new string("Yvcg.*456789*"),
                      new string("MW3.*456789*"),
                      new string("1234567890")
                    };
                    /*Crea los 2 usuarios admins, un usuario owner y 1 usuario client.*/
                    for (int i = 0; i < users.Length; i++)
                    {
                        //Genera un codigo unico para cada usuario
                        users[i].CodeUser = Guid.NewGuid();
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
                            //Crea los otros usuarios
                            else
                            {
                                var identity_user = await this._userManager.CreateAsync(users[i], password[2]).ConfigureAwait(false);
                                if (identity_user.Succeeded != true)
                                {
                                    throw new InvalidOperationException($"No se creó el usuario {users[i].UserName}.");
                                }
                            }
                            //Asigna el role admin a los 2 primeros usuarios
                            if (i < 2)
                            {
                                var identity_role = await this._userManager.AddToRoleAsync(users[i], roles[0]).ConfigureAwait(false);
                                if (identity_role.Succeeded != true)
                                {
                                    throw new InvalidOperationException($"No se agregó el rol al usuario {users[i].UserName}.");
                                }
                            }
                            //Asigna el role owner a al 3er usuario
                            else if (i == 2)
                            {
                                var identity_role = await this._userManager.AddToRoleAsync(users[i], roles[1]).ConfigureAwait(false);
                                if (identity_role.Succeeded != true)
                                {
                                    throw new InvalidOperationException($"No se agregó el rol al usuario {users[i].UserName}.");
                                }
                            }
                            //Asigna el role client a los restantes usuarios
                            else if (i > 2)
                            {
                                var identity_role = await this._userManager.AddToRoleAsync(users[i], roles[2]).ConfigureAwait(false);
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

                /*Crea las categorias de los productos standard*/
                List<CategoryProductStandard> category_standard = new List<CategoryProductStandard>();
                if (!await this._dataContext.CategoryProductStandards.AnyAsync().ConfigureAwait(false))
                {
                    category_standard = new List<CategoryProductStandard>
                    {
                        new CategoryProductStandard
                        {            
                           Name = "Entrantes"
                        }, //0-Entrantes
                        new CategoryProductStandard
                        {
                            Name = "Postres"
                        }, //1-Postres
                        new CategoryProductStandard
                        {
                             Name = "Platos Principales"
                        }, //2-Platos Principales
                        new CategoryProductStandard
                        {
                           Name = "Mariscos"
                        }, //3-Mariscos
                        new CategoryProductStandard
                        {
                             Name = "Bebidas"
                        }, //4-Bebidas
                        new CategoryProductStandard
                        {
                             Name = "Vinos y Licores"
                        }, //5-Vinos y Licores
                    };
                    await this._dataContext.CategoryProductStandards.AddRangeAsync(category_standard).ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                }

                /*Crea las categorias de los productos especiales*/
                List<CategoryProductSpecial> category_especial = new List<CategoryProductSpecial>();
                if (!await this._dataContext.CategoryProductSpecials.AnyAsync().ConfigureAwait(false))
                {
                    category_especial = new List<CategoryProductSpecial>
                    {
                       new CategoryProductSpecial
                       {
                          Name = "Pastas"
                       }, //6-Pastas
                       new CategoryProductSpecial
                       {
                            Name = "Pizzas"
                       }, //7-Pizzas
                    };
                    await this._dataContext.CategoryProductSpecials.AddRangeAsync(category_especial).ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                }

                /*Crea las categorias de los productos de agrego*/
                List<CategoryProductTypeAggregate> category_aggregate = new List<CategoryProductTypeAggregate>();
                if (!await this._dataContext.CategoryProductTypeAggregates.AnyAsync().ConfigureAwait(false))
                {
                    category_aggregate = new List<CategoryProductTypeAggregate>
                    {
                       new CategoryProductTypeAggregate
                       {
                          Name = "Quesos"
                       },
                       new CategoryProductTypeAggregate
                       {
                          Name = "Frutas"
                       },
                       new CategoryProductTypeAggregate
                       {
                          Name = "Setas"
                       },
                       new CategoryProductTypeAggregate
                       {
                          Name = "Embutidos"
                       },
                       new CategoryProductTypeAggregate
                       {
                          Name = "Mariscos"
                       },
                    };
                    await this._dataContext.CategoryProductTypeAggregates.AddRangeAsync(category_aggregate).ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                }

                /*Crea las categorias de los tipos de quesos*/
                List<CategoryProductTypeCheese> category_cheese = new List<CategoryProductTypeCheese>();
                if (!await this._dataContext.CategoryProductTypeCheeses.AnyAsync().ConfigureAwait(false))
                {
                    category_cheese = new List<CategoryProductTypeCheese>
                    {
                       new CategoryProductTypeCheese
                       {
                          Name = "Queso Gouda"
                       },
                    };
                    await this._dataContext.CategoryProductTypeCheeses.AddRangeAsync(category_cheese).ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                }

                /*Crea los productos de agrego*/
                if (!await this._dataContext.ProductTypeAggregates.AnyAsync().ConfigureAwait(false))
                {
                    var products_agregate = new List<ProductTypeAggregate>
                    {
                        new ProductTypeAggregate
                        {
                            Name = "Queso",
                            CategoryProductTypeAggregate = category_aggregate[0],
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Queso blanco de excelente calidad",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 30,
                            Stock = 120,
                        }, //0-Queso
                        new ProductTypeAggregate     //1-Queso Gouda
                        {
                            Name = "Queso Gouda",
                            CategoryProductTypeAggregate = category_aggregate[0],
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Queso Gouda de importación.",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 45,
                            Stock = 150,
                        }, //1-Queso Gouda
                        new ProductTypeAggregate
                        {
                            Name = "Jamón",
                            CategoryProductTypeAggregate = category_aggregate[3],
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Jamón de excelente calidad.",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 30,
                            Stock = 170,
                        }, //2-Jamóm
                        new ProductTypeAggregate
                        {
                            Name = "Chorizo",
                            CategoryProductTypeAggregate = category_aggregate[3],
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Chorizo de excelente calidad.",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 30,
                            Stock = 250,
                        }, //3-Chorizo
                        new ProductTypeAggregate
                        {
                            Name = "Aceitunas",
                            CategoryProductTypeAggregate = category_aggregate[1],
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Aceitunas de Goya.",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 45,
                            Stock = 150,
                        }, //4-Aceitunas
                        new ProductTypeAggregate
                        {
                            Name = "Champiñón",
                            CategoryProductTypeAggregate = category_aggregate[2],
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Champiñón de importación.",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 40,
                            Stock = 250,
                        }, //5-Champiñón
                        new ProductTypeAggregate
                        {
                            Name = "Atún",
                            CategoryProductTypeAggregate = category_aggregate[4],
                            DateCreated = DateTime.UtcNow,
                            DateUpdate = DateTime.UtcNow,
                            Description = "Atún de excelente calidad.",
                            IsAvailabe = true,
                            LastBuy = DateTime.UtcNow,
                            Price = 55,
                            Stock = 110,
                        }, //6-Atún
                        new ProductTypeAggregate
                        {
                            Name = "Camarón",
                            CategoryProductTypeAggregate = category_aggregate[4],
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
                    products_agregate[0].ImageProductTypeAggregates = new List<ImageProductTypeAggregate>
                    {
                       new ImageProductTypeAggregate
                       {
                         ImageProductPath = "~/images/image_products/agregados/queso_blanco0.jpg"
                       },
                    };
                    products_agregate[1].ImageProductTypeAggregates = new List<ImageProductTypeAggregate>
                    {
                       new ImageProductTypeAggregate
                       {
                         ImageProductPath = "~/images/image_products/agregados/queso_gouda0.jpg"
                       },
                    };
                    products_agregate[2].ImageProductTypeAggregates = new List<ImageProductTypeAggregate>
                    {
                       new ImageProductTypeAggregate
                       {
                         ImageProductPath = "~/images/image_products/agregados/jamon0.jpg"
                       },
                    };
                    products_agregate[3].ImageProductTypeAggregates = new List<ImageProductTypeAggregate>
                    {
                       new ImageProductTypeAggregate
                       {
                         ImageProductPath = "~/images/image_products/agregados/chorizo0.jpg"
                       },
                    };
                    products_agregate[4].ImageProductTypeAggregates = new List<ImageProductTypeAggregate>
                    {
                       new ImageProductTypeAggregate
                       {
                         ImageProductPath = "~/images/image_products/agregados/aceitunas0.jpg"
                       },
                    };
                    products_agregate[5].ImageProductTypeAggregates = new List<ImageProductTypeAggregate>
                    {
                       new ImageProductTypeAggregate
                       {
                         ImageProductPath = "~/images/image_products/agregados/champiñón0.jpg"
                       },
                    };
                    products_agregate[6].ImageProductTypeAggregates = new List<ImageProductTypeAggregate>
                    {
                       new ImageProductTypeAggregate
                       {
                         ImageProductPath = "~/images/image_products/agregados/atún0.jpg"
                       },
                    };
                    products_agregate[7].ImageProductTypeAggregates = new List<ImageProductTypeAggregate>
                    {
                       new ImageProductTypeAggregate
                       {
                         ImageProductPath = "~/images/image_products/agregados/camaron0.jpg"
                       },
                    };
                    await this._dataContext.ProductTypeAggregates
                    .AddRangeAsync(products_agregate)
                    .ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync()
                    .ConfigureAwait(false);
                }

                /*Crea los productos tipos de queso*/
                if(!await this._dataContext.ProductTypeCheese.AnyAsync().ConfigureAwait(false))
                {
                    var products_cheese = new List<ProductTypeCheese>
                    {
                        new ProductTypeCheese
                        {
                           Name = "Queso Gouda",
                           CategoryProductTypeCheese = category_cheese[0],
                           DateCreated = DateTime.UtcNow,
                           DateUpdate = DateTime.UtcNow,
                           Description = "Queso Gouda de excelente calidad.",
                           IsAvailabe = true,
                           LastBuy = DateTime.UtcNow,
                           Price = 35,
                           Stock = 220,
                        },
                    };
                    products_cheese[0].ImageProductTypeCheeses = new List<ImageProductTypeCheese>
                    {
                       new ImageProductTypeCheese
                       {
                         ImageProductPath = "~/images/image_products/type_cheeses/queso_gouda0.jpg"
                       },
                    };
                    await this._dataContext.ProductTypeCheese.AddRangeAsync(products_cheese).ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
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
                           CategoryProductStandard = category_standard[0],
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
                           CategoryProductStandard = category_standard[1],
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
                           CategoryProductStandard = category_standard[2],
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
                           CategoryProductStandard = category_standard[3],
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
                           CategoryProductStandard = category_standard[4],
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
                           CategoryProductStandard = category_standard[5],
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
                         ImageProductPath = "~/images/image_products/entrantes/ensalada_fria1.jpg"
                       },
                       new ImageProductStandard
                       {
                         ImageProductPath = "~/images/image_products/entrantes/ensalada_fria2.jpg"
                       }
                    };
                    //Agrega las imagenes del producto2(Coco Glasé)
                    products_standars[1].ImageProductStandards = new List<ImageProductStandard>
                    {
                       new ImageProductStandard
                       {
                         ImageProductPath = "~/images/image_products/postres/coco_glaset1.jpg"
                       },
                    };
                    //Agrega las imagenes del producto3(Bistec de Cerdo)
                    products_standars[2].ImageProductStandards = new List<ImageProductStandard>
                    {
                       new ImageProductStandard
                       {
                         ImageProductPath = "~/images/image_products/platos_principales/bisteccerdo1.jpg"
                       },
                       new ImageProductStandard
                       {
                         ImageProductPath = "~/images/image_products/platos_principales/bisteccerdo2.jpg"
                       },
                       new ImageProductStandard
                       {
                         ImageProductPath = "~/images/image_products/platos_principales/bisteccerdo3.jpg"
                       },
                    };
                    //Agrega las imagenes del producto4(Camarón Grille)
                    products_standars[3].ImageProductStandards = new List<ImageProductStandard>
                    {
                       new ImageProductStandard
                       {
                         ImageProductPath = "~/images/image_products/mariscos/camaron_grille1.jpg"
                       },
                       new ImageProductStandard
                       {
                         ImageProductPath = "~/images/image_products/mariscos/camaron_grille2.jpg"
                       },
                       new ImageProductStandard
                       {
                         ImageProductPath = "~/images/image_products/mariscos/camaron_grille3.jpg"
                       },
                    };
                    //Agrega las imagenes del producto5(Malta Holland)
                    products_standars[4].ImageProductStandards = new List<ImageProductStandard>
                    {
                       new ImageProductStandard
                       {
                         ImageProductPath = "~/images/image_products/bebidas/malta_holland1.jpg"
                       },
                    };
                    //Agrega las imagenes del producto6(Vino Blanco Constelación)
                    products_standars[5].ImageProductStandards = new List<ImageProductStandard>
                    {
                       new ImageProductStandard
                       {
                         ImageProductPath = "~/images/image_products/vinos_licores/vino_blanco1.jpg"
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
                          CategoryProductSpecial = category_especial[0],
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
                          CategoryProductSpecial = category_especial[1],
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
                         ImageProductPath = "~/images/image_products/pastas/espaguetti_jamon1.jpg"
                       },
                       new ImageProductSpecial
                       {
                         ImageProductPath = "~/images/image_products/pastas/espaguetti_jamon2.jpg"
                       },
                    };
                    //Agrega las imagenes del producto8(Pizza con camarones)
                    product_special[1].ImageProductSpecials = new List<ImageProductSpecial>
                    {
                       new ImageProductSpecial
                       {
                         ImageProductPath = "~/images/image_products/pizzas/pizza_camarones1.jpg"
                       },
                       new ImageProductSpecial
                       {
                         ImageProductPath = "~/images/image_products/pizzas/pizza_camarones1.jpg"
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
