namespace Isabella.Web.Services.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Common;
    using Common.Dtos.Order;
    using Common.Extras;
    using Models.Entities;
    using Data;
    using Repositorys.API;

    /// <summary>
    /// Servicio para gestionar la entidad Order(Pedidos)
    /// </summary>
    public class OrderServiceAPI : GenericService<Order>, IOrderRepositoryAPI
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="mapper"></param>
        public OrderServiceAPI(DataContext dataContext, IMapper mapper) : base(dataContext)
        {
            this._dataContext = dataContext;
            this._mapper = mapper;
        }

        /// <summary>
        /// Agrega Productos al carrito de compras
        /// </summary>
        /// <param name="addProductToCarShopDto"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddProductToCarShopAsync(AddProductToCarShopDto addProductToCarShopDto)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (addProductToCarShopDto == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Debe enviar los datos necesarios para agregar el producto al carrito de compras.";
                    return serviceResponse;
                }
                if (addProductToCarShopDto.Quantity < 0)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "La cantidad de productos debe ser un valor positivo.";
                    return serviceResponse;
                }
                //Verifica que el usuario este registrado en la base de datos.
                var user = await this._dataContext.Users.FirstOrDefaultAsync(c => c.UserName == addProductToCarShopDto.CodeUser);
                if (user == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El usuario no se encuentra registrado en la base de datos.";
                    return serviceResponse;
                }
                //Verifica que el producto se encuentre en la base de datos.
                var product = await this._dataContext.Products
                .Include(c => c.ProductImages)
                .FirstOrDefaultAsync(c => c.Id == addProductToCarShopDto.ProductId);
                if (product == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El producto no se encuentra en la base de datos.";
                    return serviceResponse;
                }
                //Verifica que el producto este en el carrito de compras del usuario
                var exist_product_in_carsshop = await this._dataContext.CarShops
                .Include(c => c.CantProductForAdds)
                .Include(c => c.User)
                .Include(c => c.Product)
                .Where(c => c.User == user && c.Product == product)
                .FirstOrDefaultAsync();
                //Verifica si el usuario solicita sus productos con agregados
                List<CantProductForAdd> productForAdds = new List<CantProductForAdd>();
                if (addProductToCarShopDto.CantProductForAdds != null)
                {
                  foreach(AddCantProductForAddDto cantProductForAdd in addProductToCarShopDto.CantProductForAdds)
                  {
                     var addproduct = await this._dataContext.ProductForAdds
                     .FirstOrDefaultAsync(c => c.Id == cantProductForAdd.ProductForAddId)
                     .ConfigureAwait(false);
                     if (addproduct != null)
                     { 
                       if (cantProductForAdd.Quantity < 0)
                        {
                           serviceResponse.Data = false;
                           serviceResponse.Success = false;
                           serviceResponse.Message = $"La cantidad de agregos deben ser de un valor positivo.";
                           return serviceResponse;
                        }
                       //Asigna los agregados que desea el usuario
                       productForAdds.Add(new CantProductForAdd 
                       { 
                         Price = addproduct.Price,
                         ProductForAdd = addproduct,
                         Quantity = cantProductForAdd.Quantity,
                         CarShop = exist_product_in_carsshop,
                       });
                     }
                     else
                     {
                        serviceResponse.Data = false;
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"El agregado {cantProductForAdd.ProductForAddId} no se encuentra en la base de datos.";
                        return serviceResponse;
                     }
                  }
                }
                //El producto no está en el carrito de compras
                if (exist_product_in_carsshop == null )
                {
                    //Agrega el producto al carrito
                    var carshop = new CarShop
                    {
                        CheeseGouda = addProductToCarShopDto.CheeseGouda,
                        Price = product.Price,
                        DateCreated = DateTime.UtcNow,
                        Product = product,
                        Quantity = addProductToCarShopDto.Quantity,
                        User = user,
                    };
                    //El usuario quiere el producto con agregados
                    if(productForAdds != null)
                    carshop.CantProductForAdds = productForAdds;
                    //TODO: Implementar la opción cuando el usuario quiere queso Gouda.
                    //Verifica si el usuario quiere su producto con queso Gouda en ese caso
                    //se verifica si hay queso gouda disponible.
 
                    //Agrega el producto al carrito de compras
                    await this._dataContext.CarShops.AddAsync(carshop);
                    //Guarda los cambios en la base de datos.
                    await this._dataContext.SaveChangesAsync();
                    serviceResponse.Data = true;
                    serviceResponse.Success = true;
                    serviceResponse.Message = "Se ha agregado el producto a el carrito de compras del usuario.";
                }
                //El producto se encuentra en el carrito de compras(en este caso se suma la cantidad de productos)
                else
                { 
                    //Aumenta la cantidad de productos.
                    exist_product_in_carsshop.Quantity += addProductToCarShopDto.Quantity;
                    //El producto no tenía agregados
                    if (exist_product_in_carsshop.CantProductForAdds == null)
                    {  
                       //El usuario quiere incluir agregos al producto
                       if (productForAdds != null)
                       exist_product_in_carsshop.CantProductForAdds = productForAdds;
                    }
                    //El producto tenía agregados
                    else
                    {
                       //Verifica cuales son los agregados nuevos y los que hay que actualizar
                       if (productForAdds != null)
                       {
                          foreach(CantProductForAdd cantProductForAdd in productForAdds)
                          {
                             var cantProduct = exist_product_in_carsshop.CantProductForAdds
                             .FirstOrDefault(c => c.ProductForAdd == cantProductForAdd.ProductForAdd);
                             //Nuevo agrego para el producto
                             if(cantProduct == null)
                             exist_product_in_carsshop.CantProductForAdds.Add(cantProductForAdd);
                             //Actualiza un agrego existente
                             else
                             {
                               cantProduct.Quantity += cantProductForAdd.Quantity;
                               exist_product_in_carsshop.CantProductForAdds.Add(cantProductForAdd);
                             }
                             //Actualiza el carrito de compras
                             this._dataContext.CarShops.Update(exist_product_in_carsshop);
                             //Guarda los cambios en la base de datos.
                             await this._dataContext.SaveChangesAsync();
                          }
                       } 
                    }
                    serviceResponse.Data = true;
                    serviceResponse.Success = true;
                    serviceResponse.Message = "Se ha actualizado el producto en el carrito de compras del usuario.";
                }
            }
            catch (SystemException)
            {
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = "No se encuentra el usuario en la base de datos.";
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        /// <summary>
        /// Agrega un producto Pizza o Pasta al carrito de compras.
        /// </summary>
        /// <param name="addProductToCarShopDto"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddProduct_PizzasPastas_ToCarShopAsync(AddProduct_PizzaPasta_ToCarShopDto addProductToCarShopDto)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (addProductToCarShopDto == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Debe enviar los datos necesarios para agregar el producto al carrito de compras.";
                    return serviceResponse;
                }
                if (addProductToCarShopDto.Quantity < 0)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "La cantidad de productos debe ser un valor positivo.";
                    return serviceResponse;
                }
                if(addProductToCarShopDto.CantProductForAdds != null)
                {
                    var quantity_negative = addProductToCarShopDto.CantProductForAdds.Any(c => c.Quantity <= 0);
                    if(quantity_negative)
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "La cantidad de agregados para el product0 debe ser un valor positivo.";
                    return serviceResponse;
                }
                //Verifica que el usuario este registrado en la base de datos.
                var user = await this._dataContext.Users.FirstOrDefaultAsync(c => c.UserName == addProductToCarShopDto.CodeUser);
                if (user == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El usuario no se encuentra registrado en la base de datos.";
                    return serviceResponse;
                }
                //Verifica que el producto se encuentre en la base de datos.
                var product = await this._dataContext.Products
                .Include(c => c.ProductImages)
                .FirstOrDefaultAsync(c => c.Id == addProductToCarShopDto.Product_Pizza_PastaId);
                if (product == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El producto no se encuentra en la base de datos.";
                    return serviceResponse;
                }
                //Verifica que el producto este en el carrito de compras del usuario
                var exist_product_in_carsshop = await this._dataContext.CarShops
                .Include(c => c.Product)
                .Where(c => c.User == user && c.Product == product)
                .FirstOrDefaultAsync();
                //El producto no está en el carrito del usuario
                if (exist_product_in_carsshop == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El producto no se encuentra en la base de datos.";
                    return serviceResponse;
                }
                //Mapea de AddProduct_PizzaPasta_ToCarShopDto a TypeAddForProduct_PizzaPasta
                var pizza_pasta = this._mapper.Map<List<TypeAddForProduct_PizzaPasta>>(addProductToCarShopDto.CantProductForAdds);
                if (addProductToCarShopDto.CantProductForAdds != null)
                {
                    foreach(TypeAddForProduct_PizzaPasta cantProductForAdd in addProductToCarShopDto.CantProductForAdds)
                    {
                        var addproduct = await this._dataContext.ProductForAdds
                        .FirstOrDefaultAsync(c => c.Id == cantProductForAdd.ProductForAddId)
                        .ConfigureAwait(false);
                        if (addproduct != null)
                        {
                            if (cantProductForAdd.Quantity < 0)
                            {
                                serviceResponse.Data = false;
                                serviceResponse.Success = false;
                                serviceResponse.Message = $"La cantidad de agregos deben ser de un valor positivo.";
                                return serviceResponse;
                            }
                            //Asigna los agregados que desea el usuario
                            productForAdds.Add(new CantProductForAdd
                            {
                                Price = addproduct.Price,
                                ProductForAdd = addproduct,
                                Quantity = cantProductForAdd.Quantity,
                                CarShop = exist_product_in_carsshop,
                            });
                        }
                        else
                        {
                            serviceResponse.Data = false;
                            serviceResponse.Success = false;
                            serviceResponse.Message = $"El agregado {cantProductForAdd.ProductForAddId} no se encuentra en la base de datos.";
                            return serviceResponse;
                        }
                    }
                }
                //El producto no está en el carrito de compras
                if (exist_product_in_carsshop == null)
                {
                    //Agrega el producto al carrito
                    var carshop = new CarShop
                    {
                        CheeseGouda = addProductToCarShopDto.CheeseGouda,
                        Price = product.Price,
                        DateCreated = DateTime.UtcNow,
                        Product = product,
                        Quantity = addProductToCarShopDto.Quantity,
                        User = user,
                    };
                    //El usuario quiere el producto con agregados
                    if (productForAdds != null)
                        carshop.CantProductForAdds = productForAdds;
                    //TODO: Implementar la opción cuando el usuario quiere queso Gouda.
                    //Verifica si el usuario quiere su producto con queso Gouda en ese caso
                    //se verifica si hay queso gouda disponible.

                    //Agrega el producto al carrito de compras
                    await this._dataContext.CarShops.AddAsync(carshop);
                    //Guarda los cambios en la base de datos.
                    await this._dataContext.SaveChangesAsync();
                    serviceResponse.Data = true;
                    serviceResponse.Success = true;
                    serviceResponse.Message = "Se ha agregado el producto a el carrito de compras del usuario.";
                }
                //El producto se encuentra en el carrito de compras(en este caso se suma la cantidad de productos)
                else
                {
                    //Aumenta la cantidad de productos.
                    exist_product_in_carsshop.Quantity += addProductToCarShopDto.Quantity;
                    //El producto no tenía agregados
                    if (exist_product_in_carsshop.CantProductForAdds == null)
                    {
                        //El usuario quiere incluir agregos al producto
                        if (productForAdds != null)
                            exist_product_in_carsshop.CantProductForAdds = productForAdds;
                    }
                    //El producto tenía agregados
                    else
                    {
                        //Verifica cuales son los agregados nuevos y los que hay que actualizar
                        if (productForAdds != null)
                        {
                            foreach (CantProductForAdd cantProductForAdd in productForAdds)
                            {
                                var cantProduct = exist_product_in_carsshop.CantProductForAdds
                                .FirstOrDefault(c => c.ProductForAdd == cantProductForAdd.ProductForAdd);
                                //Nuevo agrego para el producto
                                if (cantProduct == null)
                                    exist_product_in_carsshop.CantProductForAdds.Add(cantProductForAdd);
                                //Actualiza un agrego existente
                                else
                                {
                                    cantProduct.Quantity += cantProductForAdd.Quantity;
                                    exist_product_in_carsshop.CantProductForAdds.Add(cantProductForAdd);
                                }
                                //Actualiza el carrito de compras
                                this._dataContext.CarShops.Update(exist_product_in_carsshop);
                                //Guarda los cambios en la base de datos.
                                await this._dataContext.SaveChangesAsync();
                            }
                        }
                    }
                    serviceResponse.Data = true;
                    serviceResponse.Success = true;
                    serviceResponse.Message = "Se ha actualizado el producto en el carrito de compras del usuario.";
                }
            }
            catch (SystemException)
            {
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = "No se encuentra el usuario en la base de datos.";
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        /// <summary>
        /// Realiza el pedido del usuario.
        /// </summary>
        /// <param name="confirmOrderDto"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> ConfirmAllOrdersOfUserAsync(AddConfirmOrderDto confirmOrderDto)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if(confirmOrderDto == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Debe enviar los datos necesarios para confirmar la orden.";
                    return serviceResponse;
                }
                //Verifica que el usuario este registrado en la base de datos.
                var user = await this._dataContext.Users
                .FirstOrDefaultAsync(c => c.CodeUser == Guid.Parse(confirmOrderDto.CodeUser))
                .ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El usuario no se encuentra registrado en la base de datos.";
                    return serviceResponse;
                }
                //Obtiene el carrito de compras del usuario actual
                var carShops = await this._dataContext.CarShops
                .Include(c => c.CantProductForAdds)
                .Include(c => c.Product)
                .Include(c => c.User)
                .Where(c => c.User == user)
                .OrderByDescending(c => c.DateCreated).ToListAsync();
                if (carShops == null || carShops.Count() == 0)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El usuario no tiene productos en su carrito de compras.";
                    return serviceResponse;
                }
                //Mapea CarShop a OrderDetail(Obtiene los detalles de la orden)
                var order_detail = carShops.Select(c => new OrderDetail 
                {   
                    CheeseGouda = c.CheeseGouda,
                    Quantity = c.Quantity,
                    Product = c.Product,
                    CantProductForAdds = c.CantProductForAdds,
                    Price = c.Price,
                }).ToList();
                //Ejecuta método para asignar el tiempo de entrega

                //Crea el pedido del usuario
                var new_order = new Order
                {
                    Address = confirmOrderDto.Address,
                    Gps = new Models.Entities.Gps
                    {
                       Latitude_Gps = confirmOrderDto.Gps.latitude_gps,
                       Longitude_Gps = confirmOrderDto.Gps.longitude_gps,
                       Favorite_Gps = confirmOrderDto.Gps.favorite_gps,
                       Name_Gps = confirmOrderDto.Gps.name_gps,
                    },
                    //TODO: DeliveryDate = Asignar Fecha de entrega
                    User = user,
                    Items = order_detail, 
                    OrderDate = DateTime.UtcNow,
                };
                //Agrega el pedido a la base de datos
                await this._dataContext.Orders.AddAsync(new_order);
                //Vaciamos el carrito de compras del usuario
                this._dataContext.CarShops.RemoveRange(carShops);
                //Guarda los cambios en la base de datos.
                await this._dataContext.SaveChangesAsync();
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = "Se ha guardado el pedido del usuario en la base de datos.";
            }
            catch (SystemException)
            {
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = "No se encuentra el usuario en la base de datos.";
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        /// <summary>
        /// Obtiene todos los actuales pedidos del usuario.
        /// </summary>
        /// <param name="CodeUser"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetAllOrdersOfUserDto>>> GetAllOrderOfUserAsync(string CodeUser)
        {
            ServiceResponse<List<GetAllOrdersOfUserDto>> serviceResponse = new ServiceResponse<List<GetAllOrdersOfUserDto>>();
            try
            {
                if(CodeUser == null)
                {
                   serviceResponse.Data = null;
                   serviceResponse.Success = false;
                   serviceResponse.Message = "Debe enviar su código de usuario.";
                   return serviceResponse;
                }
                //Verifica que el usuario este registrado en la base de datos.
                var user = await this._dataContext.Users
                .FirstOrDefaultAsync(c => c.CodeUser == Guid.Parse(CodeUser));
                if (user == null)
                {
                   serviceResponse.Data = null;
                   serviceResponse.Success = false;
                   serviceResponse.Message = "El usuario no se encuentra registrado en la base de datos.";
                   return serviceResponse;
                }
                //Obtiene todos los pedidos actuales del usuario.
                var all_orders_users = await this._dataContext.Orders
                .Where(c => c.User ==  user)
                .OrderByDescending(c => c.OrderDate).ToListAsync();
                if (all_orders_users == null || all_orders_users.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El usuario no tiene pedidos pendientes.";
                    return serviceResponse;
                }
                //Obtiene todos los OrderDetails de cada pedido(Orden) del usuario.
                List<OrderDetail> orderDetails = new List<OrderDetail>();
                foreach(Order order in all_orders_users)
                orderDetails.AddRange(await this._dataContext.OrderDetails
                .Include(c => c.Product).ThenInclude(c => c.ProductImages)
                .Include(c => c.CantProductForAdds)
                .Where(c => c.Order == order).ToListAsync());
                if (orderDetails == null || orderDetails.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El usuario ha vaciado su carrito de compras.";
                    return serviceResponse;
                }
                //Obtiene los detalles de cada pedido para enviarlos al usuario cliente(Mapea de OrderDetail a GetOrderDetail).
                var getallorders_details = all_orders_users.Select(c => new GetAllOrdersOfUserDto
                {
                    UserId = user.Id,
                    Address = c.Address,
                    Gps = new Common.Extras.Gps
                    {
                      latitude_gps = c.Gps.Latitude_Gps, 
                      longitude_gps = c.Gps.Longitude_Gps, 
                      favorite_gps = c.Gps.Favorite_Gps, 
                      name_gps = c.Gps.Name_Gps
                    },
                    DateCreated = c.OrderDate,
                    DateDelivery = c.DeliveryDate,
                    Items = orderDetails.Select(x => new GetOrderDetailDto 
                    { 
                       NameProduct = x.Product.Name,
                       ProductId = x.Product.Id,
                       Price = x.Price,
                       Quantity = x.Quantity,
                       GetProductImages = x.Product.ProductImages.Select(y => new GetProductImageDto
                       {   Id = y.Id,
                           ProductImageId = y.Product.Id,
                           ImageProductPath = y.ImageProductPath,
                       }).ToList(),
                       CantProductForAdds = x.CantProductForAdds.Select(z => new GetCantProductForAddDto 
                       {
                          Price = z.Price,
                          Quantity = z.Quantity,
                          ProductForAddId = z.ProductForAdd.Id,
                          //GetProductForAddImages = 
                       }).ToList(),
                    }),
                }).ToList();
                serviceResponse.Data = getallorders_details;
                serviceResponse.Success = true;
                serviceResponse.Message = "Se obtuvieron todos los pedidos actuales del usuario.";
                return serviceResponse;
            }
            catch (SystemException)
            {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = "No se encuentra el usuario en la base de datos.";
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene el carrito de compras del usuario
        /// </summary>
        /// <param name="CodeUser"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetCarShopDto>>> GetCarsShopAsync(string CodeUser)
        {
            ServiceResponse<List<GetCarShopDto>> serviceResponse = new ServiceResponse<List<GetCarShopDto>>();
            try
            {
                if (CodeUser == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Debe enviar su código de usuario.";
                    return serviceResponse;
                }
                //Verifica que el usuario este registrado en la base de datos.
                var user = await this._dataContext.Users
                .FirstOrDefaultAsync(c => c.CodeUser == Guid.Parse(CodeUser))
                .ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El usuario no se encuentra registrado en la base de datos.";
                    return serviceResponse;
                }
                //Obtiene el carrito de compras del usuario actual
                var carShops = await this._dataContext.CarShops
                .Include(c => c.User)
                .Include(c => c.Product)
                .Where(c => c.User == user)
                .OrderByDescending(c => c.DateCreated).ToListAsync();
                if (carShops == null || carShops.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El usuario no tiene productos en su carrito de compras.";
                    return serviceResponse;
                }
                //Mapea de una lista de CarShop a una lista GetCarShop
                var getCarShops = carShops.Select(c => new GetCarShopDto
                {
                    

                });
                //Obtiene los otros campos restantes
                foreach (GetCarShopDto getCarShop in getCarShops)
                {
                    var product = await this._dataContext.Products.FirstOrDefaultAsync(c => c.Id == getCarShop.ProductId);
                    getCarShop.Name = product.Name;
                    getCarShop.Average = product.Average;
                }
                //serviceResponse.Data = getCarShops;
                serviceResponse.Success = true;
                serviceResponse.Message = "Se obtuvo el carrito de compras del usuario.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
        
        /// <summary>
        /// Elimina un Producto de su carrito de compras
        /// </summary>
        /// <param name="deleteProductToCarShop"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> DeleteProductToCarShopAsync(DelProductToCarShopDto deleteProductToCarShop)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que el usuario este registrado en la base de datos.
                var user = await this._dataContext.Users
                .FirstOrDefaultAsync(c => c.CodeUser == Guid.Parse(deleteProductToCarShop.CodeUser));
                if (user == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El usuario no se encuentra registrado en la base de datos.";
                    return serviceResponse;
                }
                //Verifica que el producto se encuentre en la base de datos.
                var product = await this._dataContext.Products.FirstOrDefaultAsync(c => c.Id == deleteProductToCarShop.ProductId);
                if (product == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El producto no se encuentra en la base de datos.";
                    return serviceResponse;
                }
                //Verifica que el usuario tenga el producto en su carrito de compras
                var exist_product = await this._dataContext.CarShops
               .Where(c => c.User == user && c.Product == product)
               .FirstOrDefaultAsync();
                if (exist_product == null)
                {
                   serviceResponse.Data = false;
                   serviceResponse.Success = false;
                   serviceResponse.Message = "El producto no se encuentra en el carrito de compras del usuario.";
                   return serviceResponse;
                }
                //Elimina el producto del carrito de compras del usuario
                this._dataContext.CarShops.Remove(exist_product);
                //Guarda los cambios en la base de datos.
                await this._dataContext.SaveChangesAsync();
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = "Se ha borrado el producto del carrito de compras del usuario.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        /// <summary>
        /// Actualiza detalles en el carrito de compras.
        /// </summary>
        /// <param name="updateCarShop"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdateCarShopAsync(UpdateCarShopDto updateCarShop)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que el usuario este registrado en la base de datos.
                var user = await this._dataContext.Users
                .FirstOrDefaultAsync(c => c.CodeUser == Guid.Parse(updateCarShop.CodeUser))
                .ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El usuario no se encuentra registrado en la base de datos.";
                    return serviceResponse;
                }
                //Verifica que el producto se encuentre en la base de datos.
                var product = await this._dataContext.Products.FirstOrDefaultAsync(c => c.Id == updateCarShop.ProductId);
                if (product == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El producto no se encuentra en la base de datos.";
                    return serviceResponse;
                }
                //Obtiene el carrito de compras del usuario
                var car_shop = await this._dataContext.CarShops
                .Where(c => c.User == user)
                .OrderByDescending(c => c.DateCreated)
                .ToListAsync()
                .ConfigureAwait(false);
                if(car_shop == null || car_shop.Count() == 0)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El usuario no tiene productos en su carrito de compras.";
                    return serviceResponse;
                }
                //Busca el producto en el carrito de compras del usuario.
                var found_product = car_shop.FirstOrDefault(c => c.Product == product);
                if( found_product == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El producto no se encuentra en el carrito de compras del usuario.";
                    return serviceResponse;
                }
                //Asigna la nueva cantidad del producto
                found_product.Quantity = updateCarShop.Quantity;
                //Actualiza el carrito de compras
                this._dataContext.CarShops.Update(found_product);
                //Guarda los cambios en la base de datos.
                await this._dataContext.SaveChangesAsync();
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = "Se ha actualizado el carrito de compras.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

       
    }
}
