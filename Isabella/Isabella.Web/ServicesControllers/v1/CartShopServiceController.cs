namespace Isabella.Web.ServicesControllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    using Common;
    using Common.Dtos.CarShop;
    using Common.RepositorysDtos;
    using Helpers.RepositoryHelpers;
    using Helpers;
    using Models.Entities;
    using Resources;
    using AutoMapper;
    using Isabella.Common.Dtos.SubCategorie;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Claims;

    /// <summary>
    /// Servicio para el controlador del carrito de compras.
    /// </summary>
    public class CartShopServiceController : ICartShopDto
    {

        private readonly ServiceGenericHelper<CartShop> _serviceGenericCartShopHelper;
        private readonly ServiceGenericHelper<Product> _serviceGenericProductHelper;
        private readonly ServiceGenericHelper<Aggregate> _serviceGenericAggregateHelper;
        private readonly ServiceGenericHelper<CantAggregate> _serviceGenericCantAggregateHelper;
        private readonly ServiceGenericHelper<SubCategorie> _serviceGenericSubCategoryHelper;
        private readonly ServiceGenericHelper<ProductCombined> _serviceGenericProductCombinedHelper;
        private readonly IUserRepositoryHelper _userServiceHelper;
        private readonly IMapper _mapper;

        /// <summary>
        /// Claims del usuario.
        /// </summary>
        public ClaimsPrincipal ClaimsPrincipal { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceGenericCarShopHelper"></param>
        /// <param name="serviceGenericProductHelper"></param>
        /// <param name="serviceGenericAggregateHelper"></param>
        /// <param name="serviceGenericCantAggregateHelper"></param>
        /// <param name="serviceGenericSubCategoryHelper"></param>
        /// <param name="serviceGenericProductCombinedHelper"></param>
        /// <param name="userrepositoryHelper"></param>
        /// <param name="mapper"></param>
        public CartShopServiceController(ServiceGenericHelper<CartShop> serviceGenericCarShopHelper, 
        ServiceGenericHelper<Product> serviceGenericProductHelper, 
        ServiceGenericHelper<Aggregate> serviceGenericAggregateHelper,
        ServiceGenericHelper<CantAggregate> serviceGenericCantAggregateHelper,
        ServiceGenericHelper<SubCategorie> serviceGenericSubCategoryHelper,
        ServiceGenericHelper<ProductCombined> serviceGenericProductCombinedHelper,
        IUserRepositoryHelper userrepositoryHelper,
        IMapper mapper)
        {
            this._serviceGenericCartShopHelper = serviceGenericCarShopHelper;
            this._serviceGenericProductHelper = serviceGenericProductHelper;
            this._serviceGenericAggregateHelper = serviceGenericAggregateHelper;
            this._serviceGenericCantAggregateHelper = serviceGenericCantAggregateHelper;
            this._serviceGenericSubCategoryHelper = serviceGenericSubCategoryHelper;
            this._serviceGenericProductCombinedHelper = serviceGenericProductCombinedHelper;
            this._userServiceHelper = userrepositoryHelper;
            this._mapper = mapper;
        }

        /// <summary>
        /// Agrega productos al carrito de compras.
        /// </summary>
        /// <param name="addProductsToCarShop"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetCarShopProductDto>> AddProductsToCartShopAsync(AddProductToCartShopDto addProductsToCarShop)
        {
            ServiceResponse<GetCarShopProductDto> serviceResponse = new ServiceResponse<GetCarShopProductDto>();
            try
            {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                if (addProductsToCarShop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica que la cantidad se mayor que 1.
                if (addProductsToCarShop.Quantity < 1)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Verifica si el producto está en la base de datos y está disponible.
                var product = await this._serviceGenericProductHelper
                .WhereFirstEntityAsync(c => c.Id == addProductsToCarShop.ProductId && c.IsAvailabe == true, c => c.Category, c => c.SubCategories)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotIsAvailable;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotIsAvailable);
                    return serviceResponse;
                }
                //Verifica si el usuario pide el producto con subcategoria
                var subCategory = new SubCategorie();
                if (addProductsToCarShop.SubCategoryId != null)
                {
                    //Verifica si la subcategoria existe y pertenece al producto
                    subCategory = await this._serviceGenericSubCategoryHelper
                    .WhereFirstEntityAsync(c => c.Id == addProductsToCarShop.SubCategoryId && c.Product == product, c => c.Product)
                    .ConfigureAwait(false);
                    if (subCategory == null)
                    {
                        serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SubCategoryNotIsProduct;
                        serviceResponse.Data = null;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile
                        .GetValueResourceString(GetValueResourceFile.KeyResource.SubCategoryNotIsProduct);
                        return serviceResponse;
                    }
                }
                else
                subCategory = null;
                //Verifica si el producto seleccionado admite agregados
                if (product.SupportAggregate == false && addProductsToCarShop.CantAggregates != null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotSupportAggregate;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotSupportAggregate);
                    return serviceResponse;
                }
                //Verifica que los agregados solicitados esten disponibles.
                List<Models.Entities.CantAggregate> cantAggregates = new List<Models.Entities.CantAggregate>();
                if (product.SupportAggregate == true && addProductsToCarShop.CantAggregates != null)
                {
                    foreach (string key in addProductsToCarShop.CantAggregates.Keys)
                    {
                        int parse_key;
                        if (!int.TryParse(key, out parse_key))
                        {
                            serviceResponse.Code = (int)GetValueResourceFile.KeyResource.FormatAggregateNotSupport;
                            serviceResponse.Data = null;
                            serviceResponse.Success = false;
                            serviceResponse.Message = GetValueResourceFile
                            .GetValueResourceString(GetValueResourceFile.KeyResource.FormatAggregateNotSupport);
                            return serviceResponse;
                        }
                        if (parse_key < 1)
                        {
                            serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                            serviceResponse.Data = null;
                            serviceResponse.Success = false;
                            serviceResponse.Message = GetValueResourceFile
                            .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                            return serviceResponse;
                        }
                        var quantity = addProductsToCarShop.CantAggregates.GetValueOrDefault(key);
                        if (quantity < 1)
                        {
                            serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                            serviceResponse.Data = null;
                            serviceResponse.Success = false;
                            serviceResponse.Message = GetValueResourceFile
                            .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                            return serviceResponse;
                        }
                        var aggregate = await this._serviceGenericAggregateHelper
                        .WhereFirstEntityAsync(c => c.Id == parse_key && c.IsAvailabe == true)
                        .ConfigureAwait(false);
                        if (aggregate == null)
                        {
                            serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotIsAvailable;
                            serviceResponse.Data = null;
                            serviceResponse.Success = false;
                            serviceResponse.Message = GetValueResourceFile
                            .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotIsAvailable);
                            return serviceResponse;
                        }
                        //TODO: Verifica que la cantidad de agregados solicitados este disponible
                        var new_aggregate = new Models.Entities.CantAggregate
                        {
                            Aggregate = aggregate,
                            Price = aggregate.Price,
                            Quantity = quantity,
                        };
                        cantAggregates.Add(new_aggregate);
                    }
                }
                //Obtiene el contexto
                var context_cart_shop = this._serviceGenericCartShopHelper._context;
                //Define una entidad consultable
                IQueryable<CartShop> entity_carshop = context_cart_shop.AsQueryable();
                //Agrega los Include y ThenInclude necesarios
                entity_carshop = entity_carshop.Include(c => c.User)
                .Include(c => c.ProductCombined.Product.Category)
                .Include(c => c.ProductCombined.Product.SubCategories)
                .Include(c => c.ProductCombined.SubCategory.Product.Category)
                .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate);
                //Verifica si hay agregados para agregar el filtro para los agregados.
                if (cantAggregates.Any())
                {
                    //Crea la consulta con los posibles agregados, para luego verificar si el producto
                    //Combinado los posee
                    foreach (CantAggregate cantAggregate in cantAggregates)
                    {
                        entity_carshop = entity_carshop.Where(c => c.ProductCombined.CantAggregates
                       .Select(c => c.Aggregate).Contains(cantAggregate.Aggregate) == true);
                    }
                }
                //Agrega los filtros restantes
                entity_carshop = entity_carshop
                .Where(c => c.User == user)
                .Where(c => c.ProductCombined.Product == product)
                .Where(c => c.ProductCombined.CantAggregates.Count == cantAggregates.Count)
                .Where(c => c.ProductCombined.SubCategory == subCategory);
                //Ejecuta la consulta(Verifica si en el carrito del usuario existe un producto de este tipo)
                var product_in_carshop = await entity_carshop.SingleOrDefaultAsync().ConfigureAwait(false);
                //Verifica si el producto no estaba en el carrito del usuario para agregarlo.
                var product_is_cartshop = new CartShop();
                if (product_in_carshop == null)
                {
                    product_is_cartshop = new CartShop
                    {
                        User = user,
                        ProductCombined = new ProductCombined
                        {
                            Product = product,
                            Price = product.Price,
                            SubCategory = subCategory,
                            Quantity = addProductsToCarShop.Quantity,
                            CantAggregates = cantAggregates.Select(c => new Models.Entities.CantAggregate
                            {
                                Aggregate = c.Aggregate,
                                Price = c.Price,
                                Quantity = c.Quantity,
                            }).ToList(),
                        },
                        DateCreated = DateTime.UtcNow,
                    };
                    //Agrega un nuevo producto al carrito de compras
                    await this._serviceGenericCartShopHelper
                    .AddEntityAsync(product_is_cartshop)
                    .ConfigureAwait(false);
                    //Actualiza la base de datos.
                    await this._serviceGenericCartShopHelper
                    .SaveChangesBDAsync().ConfigureAwait(false);
                }
                //Si el usuario tiene ese producto combinado solo se actualizan las cantidades.
                else
                {
                   List<CantAggregate> UpdateCanAggregate = new List<CantAggregate>();
                   foreach(CantAggregate cantaggregate in product_in_carshop.ProductCombined.CantAggregates)
                   {
                      var update_cantaggregate = cantAggregates.FirstOrDefault(c => c.Aggregate.Id == cantaggregate.Aggregate.Id);
                      cantaggregate.Quantity += update_cantaggregate.Quantity;
                      UpdateCanAggregate.Add(cantaggregate);
                   }
                   //Actualiza las cantidades de los agregados
                   this._serviceGenericCantAggregateHelper.UpdateRangeEntity(UpdateCanAggregate);
                   await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync().ConfigureAwait(false);
                   //Actualiza la cantidad del producto
                   product_in_carshop.ProductCombined.Quantity += addProductsToCarShop.Quantity;
                   this._serviceGenericCartShopHelper.UpdateEntity(product_in_carshop);
                   //Guarda los cambios en la base de datos
                   await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                   product_is_cartshop = new CartShop
                   {
                        User = user,
                        ProductCombined = new ProductCombined
                        {
                            Product = product,
                            SubCategory = subCategory,
                            Price = product.Price,
                            Quantity = product_in_carshop.ProductCombined.Quantity,
                            CantAggregates = UpdateCanAggregate.Select(c => new Models.Entities.CantAggregate
                            {
                                Aggregate = c.Aggregate,
                                Price = c.Price,
                                Quantity = c.Quantity,
                            }).ToList(),
                        },
                        DateCreated = DateTime.UtcNow,
                   };
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                if(product_is_cartshop.ProductCombined.SubCategory != null && product_is_cartshop.ProductCombined.CantAggregates.Count > 0)
                serviceResponse.Data = new GetCarShopProductDto 
                {
                   Description = product_is_cartshop.ProductCombined.Product.Description,
                   DateCreated = DateTime.UtcNow,
                   IsAvailabe = product_is_cartshop.ProductCombined.Product.IsAvailabe,
                   SupportAggregate = product_is_cartshop.ProductCombined.Product.SupportAggregate,
                   Price = product_is_cartshop.ProductCombined.Product.Price,
                   ProductId = product_is_cartshop.ProductCombined.Product.Id,
                   Average = product_is_cartshop.ProductCombined.Product.Average,
                   Name = product_is_cartshop.ProductCombined.Product.Name,
                   ProductCombinedId = product_is_cartshop.ProductCombined.Id,
                   Category = new Common.Dtos.Categorie.GetCategorieDto
                   { 
                       Name = product_is_cartshop.ProductCombined.Product.Category.Name,
                       Id = product_is_cartshop.ProductCombined.Product.Category.Id,
                   },
                   SubCategory = new GetSubCategorieDto
                   {
                       Id = product_is_cartshop.ProductCombined.SubCategory.Id,
                       ProductId = product_is_cartshop.ProductCombined.SubCategory.Product.Id,
                       Name = product_is_cartshop.ProductCombined.SubCategory.Name,
                       Price = product_is_cartshop.ProductCombined.SubCategory.Price, 
                       Description = product_is_cartshop.ProductCombined.SubCategory.Description,
                       IsAvailable = product_is_cartshop.ProductCombined.SubCategory.IsAvailable
                   },
                   CantAggregates = product_is_cartshop.ProductCombined.CantAggregates.Select(c => new GetCantAggregateDto 
                   { 
                     Id = c.Aggregate.Id,
                     Name = c.Aggregate.Name,
                     Price = c.Aggregate.Price,
                     Quantity = c.Quantity,               
                   }).ToList(),
                   Quantity = product_is_cartshop.ProductCombined.Quantity,
                };
                else if(product_is_cartshop.ProductCombined.SubCategory != null && product_is_cartshop.ProductCombined.CantAggregates.Count <= 0)
                serviceResponse.Data = new GetCarShopProductDto
                {
                     Description = product_is_cartshop.ProductCombined.Product.Description,
                     DateCreated = DateTime.UtcNow,
                     IsAvailabe = product_is_cartshop.ProductCombined.Product.IsAvailabe,
                     SupportAggregate = product_is_cartshop.ProductCombined.Product.SupportAggregate,
                     Price = product_is_cartshop.ProductCombined.Product.Price,
                     ProductId = product_is_cartshop.ProductCombined.Product.Id,
                     Average = product_is_cartshop.ProductCombined.Product.Average,
                     Name = product_is_cartshop.ProductCombined.Product.Name,
                     ProductCombinedId = product_is_cartshop.ProductCombined.Id,
                     Category = new Common.Dtos.Categorie.GetCategorieDto
                     {
                         Name = product_is_cartshop.ProductCombined.Product.Category.Name,
                         Id = product_is_cartshop.ProductCombined.Product.Category.Id,
                     },
                     SubCategory = new GetSubCategorieDto
                     {
                         Id = product_is_cartshop.ProductCombined.SubCategory.Id,
                         ProductId = product_is_cartshop.ProductCombined.SubCategory.Product.Id,
                         Name = product_is_cartshop.ProductCombined.SubCategory.Name,
                         Price = product_is_cartshop.ProductCombined.SubCategory.Price,
                         Description = product_is_cartshop.ProductCombined.SubCategory.Description,
                         IsAvailable = product_is_cartshop.ProductCombined.SubCategory.IsAvailable
                     },
                     CantAggregates = null,
                     Quantity = product_is_cartshop.ProductCombined.Quantity,
                 };
                else if(product_is_cartshop.ProductCombined.SubCategory == null && product_is_cartshop.ProductCombined.CantAggregates.Count > 0)
                serviceResponse.Data = new GetCarShopProductDto
                {
                        Description = product_is_cartshop.ProductCombined.Product.Description,
                        DateCreated = DateTime.UtcNow,
                        IsAvailabe = product_is_cartshop.ProductCombined.Product.IsAvailabe,
                        SupportAggregate = product_is_cartshop.ProductCombined.Product.SupportAggregate,
                        Price = product_is_cartshop.ProductCombined.Product.Price,
                        ProductId = product_is_cartshop.ProductCombined.Product.Id,
                        Average = product_is_cartshop.ProductCombined.Product.Average,
                        Name = product_is_cartshop.ProductCombined.Product.Name,
                        ProductCombinedId = product_is_cartshop.ProductCombined.Id,
                        Category = new Common.Dtos.Categorie.GetCategorieDto
                        {
                            Name = product_is_cartshop.ProductCombined.Product.Category.Name,
                            Id = product_is_cartshop.ProductCombined.Product.Category.Id,
                        },
                        SubCategory = null,
                        CantAggregates = product_is_cartshop.ProductCombined.CantAggregates.Select(c => new GetCantAggregateDto
                        {
                            Id = c.Aggregate.Id,
                            Name = c.Aggregate.Name,
                            Price = c.Aggregate.Price,
                            Quantity = c.Quantity,
                        }).ToList(),
                        Quantity = product_is_cartshop.ProductCombined.Quantity,
                    };
                else if (product_is_cartshop.ProductCombined.SubCategory == null && product_is_cartshop.ProductCombined.CantAggregates.Count <= 0)
                serviceResponse.Data = new GetCarShopProductDto
                {
                        Description = product_is_cartshop.ProductCombined.Product.Description,
                        DateCreated = DateTime.UtcNow,
                        IsAvailabe = product_is_cartshop.ProductCombined.Product.IsAvailabe,
                        SupportAggregate = product_is_cartshop.ProductCombined.Product.SupportAggregate,
                        Price = product_is_cartshop.ProductCombined.Product.Price,
                        ProductId = product_is_cartshop.ProductCombined.Product.Id,
                        Average = product_is_cartshop.ProductCombined.Product.Average,
                        Name = product_is_cartshop.ProductCombined.Product.Name,
                        ProductCombinedId = product_is_cartshop.ProductCombined.Id,
                        Category = new Common.Dtos.Categorie.GetCategorieDto
                        {
                            Name = product_is_cartshop.ProductCombined.Product.Category.Name,
                            Id = product_is_cartshop.ProductCombined.Product.Category.Id,
                        },
                        SubCategory = null,
                        CantAggregates = null,
                        Quantity = product_is_cartshop.ProductCombined.Quantity,
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception ex)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene el carrito de compras de un usuario.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<GetAllProductOfCartShopDto>> GetMyCartShopAsync()
        {
            ServiceResponse<GetAllProductOfCartShopDto> serviceResponse = new ServiceResponse<GetAllProductOfCartShopDto>();
            try
            {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Verifica si hay productos en el carrito
                var all_products_in_carshop = await this._serviceGenericCartShopHelper._context
                .Include(c => c.User)
                .Include(c => c.ProductCombined.Product.Category)
                .Include(c => c.ProductCombined.Product.SubCategories)
                .Include(c => c.ProductCombined.SubCategory.Product.Category)
                .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate)
                .Include(c => c.ProductCombined.CantAggregates)
                .Where(c => c.User == user)
                .OrderByDescending(c => c.DateCreated)
                .ToListAsync()
                .ConfigureAwait(false);
                if (!all_products_in_carshop.Any())
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CarShopNotProducts;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CarShopNotProducts);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetAllProductOfCartShopDto
                {
                    GetUserDto = new Common.Dtos.Users.GetUserDto
                    {
                        Address = user.Address,
                        FirstName = user.FirstName,
                        Id = user.Id,
                        ImageUserProfile = user.ImageUserProfile,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                    },
                    GetCarShopProducts = all_products_in_carshop.Select(c => new GetCarShopProductDto 
                    {
                      ProductCombinedId = c.ProductCombined.Id,
                      ProductId = c.ProductCombined.Product.Id,
                      Average = c.ProductCombined.Product.Average,
                      SupportAggregate = c.ProductCombined.Product.SupportAggregate,
                      Name = c.ProductCombined.Product.Name,
                      Price = c.ProductCombined.Product.Price,
                      Description = c.ProductCombined.Product.Description,
                      IsAvailabe = c.ProductCombined.Product.IsAvailabe,
                      Quantity = c.ProductCombined.Quantity,
                      SubCategory = this._mapper.Map<GetSubCategorieDto>(c.ProductCombined.SubCategory),
                      Category = new Common.Dtos.Categorie.GetCategorieDto
                      {
                          Id = c.ProductCombined.Product.Category.Id,
                          Name = c.ProductCombined.Product.Category.Name,
                      },
                      CantAggregates = c.ProductCombined.CantAggregates.Select(x => new GetCantAggregateDto 
                      { 
                         Id = x.Aggregate.Id,
                         Name = x.Aggregate.Name,
                         Price = x.Price,
                         Quantity = x.Quantity,
                      }).ToList(),
                      DateCreated = c.DateCreated,
                   }).ToList(), 
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Elimina un producto del carrito de compras
        /// </summary>
        /// <param name="ProductCombinedId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> RemoveProductOfCartShopAsync(int ProductCombinedId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                var carshop = await this._serviceGenericCartShopHelper
                .WhereFirstEntityAsync(c => c.User == user
                && c.ProductCombined.Id == ProductCombinedId,
                c => c.ProductCombined.CantAggregates, c => c.User)
                .ConfigureAwait(false);
                if (carshop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotExistInCarShop;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotExistInCarShop);
                    return serviceResponse;
                }
                //Elimina la cantidad de agregados que tiene este producto combinado como referencia
                var all_cantaggregate_of_productcombined = await this._serviceGenericCantAggregateHelper
                .WhereListEntityAsync(c => c.ProductCombined == carshop.ProductCombined, c => c.ProductCombined)
                .ConfigureAwait(false);
                if (all_cantaggregate_of_productcombined.Any())
                {
                    this._serviceGenericCantAggregateHelper.RemoveRangeEntity(all_cantaggregate_of_productcombined);
                    await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync().ConfigureAwait(false);
                }
                //Elimina el producto combinado
                this._serviceGenericProductCombinedHelper.RemoveEntity(carshop.ProductCombined);
                await this._serviceGenericProductCombinedHelper
                .SaveChangesBDAsync().ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                //Elimina la relación del producto combinado con el carrito
                this._serviceGenericCartShopHelper.RemoveEntity(carshop);
                await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ExceptionDeleteEntity;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.ExceptionDeleteEntity);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Actualiza la subcategoria de un producto.
        /// </summary>
        /// <param name="updateSubCategoryProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdateSubCategoryAsync(UpdateSubCategoryProductDto updateSubCategoryProduct)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                if (updateSubCategoryProduct == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica que la subcategoria exista y este disponible
                var subcategory = await this._serviceGenericSubCategoryHelper
                .GetLoadAsync(c => c.Id == updateSubCategoryProduct.SubCategoryId && c.IsAvailable == true, c => c.Product.SubCategories)
                .ConfigureAwait(false);
                if (subcategory == null)
                {
                   serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SubCategoryNotIsAvailable;
                   serviceResponse.Data = false;
                   serviceResponse.Success = false;
                   serviceResponse.Message = GetValueResourceFile
                   .GetValueResourceString(GetValueResourceFile.KeyResource.SubCategoryNotIsAvailable);
                   return serviceResponse;
                }
                //Verifica si el producto está en el carrito del usuario y 
                //si el mismo no tiene asignada la subcategoria que se quiere actualizar. 
                var context_cart_shop = this._serviceGenericCartShopHelper._context;
                //Define una entidad consultable
                IQueryable<CartShop> entity_carshop = context_cart_shop.AsQueryable();
                //Agrega los Include y ThenInclude necesarios
                entity_carshop = entity_carshop.Include(c => c.User)
                .Include(c => c.ProductCombined.Product.SubCategories).ThenInclude(c => c.Product.Category)
                .Include(c => c.ProductCombined.Product.Category)
                .Include(c => c.ProductCombined.SubCategory)
                .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate);
                //Agrega los filtros restantes
                entity_carshop = entity_carshop
                .Where(c => c.User == user)
                .Where(c => c.ProductCombined.Id == updateSubCategoryProduct.ProductCombinedId)
                .Where(c => c.ProductCombined.SubCategory != subcategory);
                //Ejecuta la consulta(Verifica si en el carrito del usuario existe un producto de este tipo)
                var product_in_carshop = await entity_carshop.FirstOrDefaultAsync().ConfigureAwait(false);
                //El producto no se encuentra en el carrito o ya tiene la subcategoria.
                if (product_in_carshop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductInCartHaveSubCategory;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductInCartHaveSubCategory);
                    return serviceResponse;
                }
                //Verifica si la subcategoria pertenece al producto base
                if (product_in_carshop.ProductCombined.Product.SubCategories.Any(c => c.Id == subcategory.Id) == false)
                {
                   serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SubCategoryNotIsProduct;
                   serviceResponse.Data = false;
                   serviceResponse.Success = false;
                   serviceResponse.Message = GetValueResourceFile
                   .GetValueResourceString(GetValueResourceFile.KeyResource.SubCategoryNotIsProduct);
                    return serviceResponse;
                }
                //Actualiza el producto con la nueva subcategoria.
                product_in_carshop.ProductCombined.SubCategory = subcategory;
                //Verifica si el nuevo producto formado ya se encuentra en el carrito del usuario para solo actualizar las cantidades
                //y eliminar las referencias a este producto.
                entity_carshop = context_cart_shop.AsQueryable();
                entity_carshop = entity_carshop.Include(c => c.User)
                .Include(c => c.ProductCombined.Product.SubCategories).ThenInclude(c => c.Product)
                .Include(c => c.ProductCombined.SubCategory)
                .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate);
                //Crea los include en caso de que el producto tenga agregados.
                if (product_in_carshop.ProductCombined.CantAggregates.Any())
                {
                    //Crea la consulta con los posibles agregados, para luego verificar si el producto
                    //combinado los posee
                    foreach (CantAggregate cantAggregate in product_in_carshop.ProductCombined.CantAggregates)
                    {
                        entity_carshop = entity_carshop.Where(c => c.ProductCombined.CantAggregates
                       .Select(c => c.Aggregate).Contains(cantAggregate.Aggregate) == true);
                    }
                }
                entity_carshop = entity_carshop
                .Where(c => c.User == user)
                .Where(c => c.ProductCombined.Product == product_in_carshop.ProductCombined.Product)
                .Where(c => c.ProductCombined.CantAggregates.Count == product_in_carshop.ProductCombined.CantAggregates.Count)
                .Where(c => c.ProductCombined.SubCategory == subcategory);
                //Ejecuta la consulta(Verifica si en el carrito del usuario existe un producto de este tipo)
                var exist_product_in_carshop = await entity_carshop.SingleOrDefaultAsync().ConfigureAwait(false);
                //El producto no existe por lo que solo actualiza el actual
                if (exist_product_in_carshop == null)
                {
                    this._serviceGenericCartShopHelper.UpdateEntity(product_in_carshop);
                    await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                }
                //El producto existe por lo que solo actualiza las cantidades y elimina el actual.
                else
                {
                    //Actualiza las cantidades
                    exist_product_in_carshop.ProductCombined.Quantity += product_in_carshop.ProductCombined.Quantity;
                    //Verifica si el producto tiene agregados
                    if(exist_product_in_carshop.ProductCombined.CantAggregates.Any())
                    {
                        List<CantAggregate> updateAggregate = new List<CantAggregate>();
                        foreach (CantAggregate cantAggregate in product_in_carshop.ProductCombined.CantAggregates)
                        {
                            var update = exist_product_in_carshop.ProductCombined.CantAggregates
                            .FirstOrDefault(c => c.Aggregate.Id == cantAggregate.Aggregate.Id);
                            update.Quantity += cantAggregate.Quantity;
                            updateAggregate.Add(update);
                        }
                        //Manda a actualizar la cantidad de agregados.
                        this._serviceGenericCantAggregateHelper.UpdateRangeEntity(updateAggregate);
                        //Guarda los cambios en la base de datos.
                        await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync();
                        //Elimina las referencias a la cantidad de agregados del producto actual.
                        this._serviceGenericCantAggregateHelper
                        .RemoveRangeEntity(product_in_carshop.ProductCombined.CantAggregates.ToList());
                        //Guarda los cambios en la base de datos.
                        await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync().ConfigureAwait(false);
                    }
                    //Elimina el producto combinado
                    this._serviceGenericProductCombinedHelper.RemoveEntity(product_in_carshop.ProductCombined);
                    await this._serviceGenericProductCombinedHelper
                    //Guarda los cambios en la base de datos.
                    .SaveChangesBDAsync().ConfigureAwait(false);
                    //Elimina la relación del producto combinado con el carrito
                    this._serviceGenericCartShopHelper.RemoveEntity(product_in_carshop);
                    //Guarda los cambios en la base de datos
                    await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                    //Actualiza el producto existente
                    this._serviceGenericCartShopHelper.UpdateEntity(exist_product_in_carshop);
                    //Guarda los cambios en la base de datos
                    await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Elimina la subcategoria de un producto combinado que está en el carrito.
        /// </summary>
        /// <param name="ProductCombinedId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> RemoveSubCategoryAsync(int ProductCombinedId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Verifica si el producto está en el carrito del usuario y 
                //si el mismo tiene asignada alguna subcategoria. 
                var context_cart_shop = this._serviceGenericCartShopHelper._context;
                //Define una entidad consultable
                IQueryable<CartShop> entity_carshop = context_cart_shop.AsQueryable();
                //Agrega los Include y ThenInclude necesarios
                entity_carshop = entity_carshop.Include(c => c.User)
                .Include(c => c.ProductCombined.Product.SubCategories).ThenInclude(c => c.Product.Category)
                .Include(c => c.ProductCombined.Product.Category)
                .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate)
                .Include(c => c.ProductCombined.SubCategory);
                //Agrega los filtros
                entity_carshop = entity_carshop
                .Where(c => c.User == user)
                .Where(c => c.ProductCombined.Id == ProductCombinedId);
                //Ejecuta la consulta(Verifica si en el carrito del usuario existe un producto de este tipo)
                var product_in_carshop = await entity_carshop.FirstOrDefaultAsync().ConfigureAwait(false);
                //El producto no se encuentra en el carrito o ya tiene la subcategoria.
                if (product_in_carshop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductInCartHaveSubCategory;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductInCartHaveSubCategory);
                    return serviceResponse;
                }
                //Verifica si el producto tiene alguna subcategoria
                if (product_in_carshop.ProductCombined.SubCategory == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductCombinedNotHaveSubCategory;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductCombinedNotHaveSubCategory);
                    return serviceResponse;
                }
                //Verifica si el nuevo producto formado ya se encuentra en el carrito del usuario para solo actualizar las cantidades
                //y eliminar las referencias a este producto.
                entity_carshop = context_cart_shop.AsQueryable();
                entity_carshop = entity_carshop.Include(c => c.User)
                .Include(c => c.ProductCombined.Product.SubCategories).ThenInclude(c => c.Product)
                .Include(c => c.ProductCombined.SubCategory)
                .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate);
                //Crea los include en caso de que el producto tenga agregados.
                if (product_in_carshop.ProductCombined.CantAggregates.Any())
                {
                    //Crea la consulta con los posibles agregados, para luego verificar si el producto
                    //combinado los posee
                    foreach (CantAggregate cantAggregate in product_in_carshop.ProductCombined.CantAggregates)
                    {
                        entity_carshop = entity_carshop.Where(c => c.ProductCombined.CantAggregates
                       .Select(c => c.Aggregate).Contains(cantAggregate.Aggregate) == true);
                    }
                }
                entity_carshop = entity_carshop
                .Where(c => c.User == user)
                .Where(c => c.ProductCombined.Product == product_in_carshop.ProductCombined.Product)
                .Where(c => c.ProductCombined.CantAggregates.Count == product_in_carshop.ProductCombined.CantAggregates.Count)
                .Where(c => c.ProductCombined.SubCategory == null);
                //Ejecuta la consulta(Verifica si en el carrito del usuario existe un producto de este tipo)
                var exist_product_in_carshop = await entity_carshop.SingleOrDefaultAsync().ConfigureAwait(false);
                //El producto no existe por lo que solo actualiza el actual
                if (exist_product_in_carshop == null)
                {
                    //Asigna la subcategoria
                    product_in_carshop.ProductCombined.SubCategory = null;
                    this._serviceGenericCartShopHelper.UpdateEntity(product_in_carshop);
                    await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                }
                //El producto existe por lo que solo actualiza las cantidades y elimina el actual.
                else
                {
                    //Actualiza las cantidades
                    exist_product_in_carshop.ProductCombined.Quantity += product_in_carshop.ProductCombined.Quantity;
                    //Verifica si el producto tiene agregados
                    if (exist_product_in_carshop.ProductCombined.CantAggregates.Any())
                    {
                        List<CantAggregate> updateAggregate = new List<CantAggregate>();
                        foreach (CantAggregate cantAggregate in product_in_carshop.ProductCombined.CantAggregates)
                        {
                            var update = exist_product_in_carshop.ProductCombined.CantAggregates
                            .FirstOrDefault(c => c.Aggregate.Id == cantAggregate.Aggregate.Id);
                            update.Quantity += cantAggregate.Quantity;
                            updateAggregate.Add(update);
                        }
                        //Manda a actualizar la cantidad de agregados.
                        this._serviceGenericCantAggregateHelper.UpdateRangeEntity(updateAggregate);
                        //Guarda los cambios en la base de datos.
                        await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync();
                        //Elimina las referencias a la cantidad de agregados del producto actual.
                        this._serviceGenericCantAggregateHelper
                        .RemoveRangeEntity(product_in_carshop.ProductCombined.CantAggregates.ToList());
                        //Guarda los cambios en la base de datos.
                        await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync().ConfigureAwait(false);
                    }
                    //Elimina el producto combinado
                    this._serviceGenericProductCombinedHelper.RemoveEntity(product_in_carshop.ProductCombined);
                    //Guarda los cambios en la base de datos.
                    await this._serviceGenericProductCombinedHelper
                    .SaveChangesBDAsync().ConfigureAwait(false);
                    //Elimina la relación del producto combinado con el carrito
                    this._serviceGenericCartShopHelper.RemoveEntity(product_in_carshop);
                    //Guarda los cambios en la base de datos
                    await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                    //Actualiza el producto existente
                    this._serviceGenericCartShopHelper.UpdateEntity(exist_product_in_carshop);
                    //Guarda los cambios en la base de datos
                    await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Actualiza la cantidad de un producto.
        /// </summary>
        /// <param name="modifyQuantityProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdateQuantityProductAsync(ModifyQuantityProductDto modifyQuantityProduct)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                if (modifyQuantityProduct == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica que la cantidad se mayor que 1.
                if (modifyQuantityProduct.Quantity < 1)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Verifica si el producto está en el carrito del usuario.
                var product_in_carshop = await this._serviceGenericCartShopHelper
                .WhereFirstEntityAsync(c => c.ProductCombined.Id == modifyQuantityProduct.ProductCombinedId && 
                c.User == user, c => c.User)
                .ConfigureAwait(false);
                if (product_in_carshop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotExistInCarShop;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotExistInCarShop);
                    return serviceResponse;
                }
                //Actualiza la cantidad del producto con un nuevo valor.
                product_in_carshop.ProductCombined.Quantity = modifyQuantityProduct.Quantity;
                this._serviceGenericCartShopHelper.UpdateEntity(product_in_carshop);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Incrementa la cantidad de un producto en un valor dado.
        /// </summary>
        /// <param name="modifyQuantityProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> IncrementQuantityProductAsync(ModifyQuantityProductDto modifyQuantityProduct)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                if (modifyQuantityProduct == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica que la cantidad se mayor que 1.
                if (modifyQuantityProduct.Quantity < 1)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Verifica si el producto está en el carrito del usuario.
                var product_in_carshop = await this._serviceGenericCartShopHelper
                .WhereFirstEntityAsync(c => c.ProductCombined.Id == modifyQuantityProduct.ProductCombinedId &&
                c.User == user, c => c.ProductCombined)
                .ConfigureAwait(false);
                if (product_in_carshop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotExistInCarShop;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotExistInCarShop);
                    return serviceResponse;
                }
                //Incrementa la cantidad del producto.
                product_in_carshop.ProductCombined.Quantity += modifyQuantityProduct.Quantity;
                this._serviceGenericCartShopHelper.UpdateEntity(product_in_carshop);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Actualiza la cantidad de un agregado en un producto que está en el carrito.
        /// </summary>
        /// <param name="modifyAggregateProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdateQuantityInAggregateProductAsync(ModifyCantInAggregateProductDto modifyAggregateProduct)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                if (modifyAggregateProduct == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica que la cantidad se mayor que 1.
                if (modifyAggregateProduct.Quantity < 1)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Verifica si el producto está en el carrito del usuario y si el mismo tiene el agregado que se quiere actualizar.
                //Obtiene el contexto
                var context_cart_shop = this._serviceGenericCartShopHelper._context;
                //Define una entidad consultable
                IQueryable<CartShop> entity_carshop = context_cart_shop.AsQueryable();
                //Agrega los Include y ThenInclude necesarios
                entity_carshop = entity_carshop.Include(c => c.User)
                .Include(c => c.ProductCombined.SubCategory)
                .Include(c => c.ProductCombined.Product.Category)
                .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate);
                //Agrega los filtros restantes
                entity_carshop = entity_carshop
                .Where(c => c.User == user)
                .Where(c => c.ProductCombined.Id == modifyAggregateProduct.ProductCombinedId)
                .Where(c => c.ProductCombined.CantAggregates.Select(c => c.Aggregate.Id)
                .Any(x => x == modifyAggregateProduct.AggregateId) == true);
                //Ejecuta la consulta(Verifica si en el carrito del usuario existe un producto de este tipo)
                var product_in_carshop = await entity_carshop.FirstOrDefaultAsync().ConfigureAwait(false);
                //El producto no se encuentra en el carrito o no tiene el agregado.
                if (product_in_carshop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotHaveAggregate;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotHaveAggregate);
                    return serviceResponse;
                }
                //Obtiene el agregado y su cantidad actual
                var cantAggregate = product_in_carshop.ProductCombined.CantAggregates.Where(c => c.Aggregate.Id == modifyAggregateProduct.AggregateId).FirstOrDefault();
                //Actualiza la cantidad del agregado.
                cantAggregate.Quantity = modifyAggregateProduct.Quantity;
                this._serviceGenericCantAggregateHelper.UpdateEntity(cantAggregate);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync().ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Incrementa la cantidad de un agregado en un producto en un valor dado.
        /// </summary>
        /// <param name="modifyAggregateProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> IncrementQuantityInAggregateProductAsync(ModifyCantInAggregateProductDto modifyAggregateProduct)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                if (modifyAggregateProduct == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica que la cantidad se mayor que 1.
                if (modifyAggregateProduct.Quantity < 1)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Verifica si el producto está en el carrito del usuario y si el mismo tiene el agregado que se quiere actualizar.
                //Obtiene el contexto
                var context_cart_shop = this._serviceGenericCartShopHelper._context;
                //Define una entidad consultable
                IQueryable<CartShop> entity_carshop = context_cart_shop.AsQueryable();
                //Agrega los Include y ThenInclude necesarios
                entity_carshop = entity_carshop.Include(c => c.User)
                .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate);
                //Agrega los filtros restantes
                entity_carshop = entity_carshop
                .Where(c => c.User == user)
                .Where(c => c.ProductCombined.Id == modifyAggregateProduct.ProductCombinedId)
                .Where(c => c.ProductCombined.CantAggregates.Select(c => c.Aggregate.Id)
                .Any(x => x == modifyAggregateProduct.AggregateId) == true);
                //Ejecuta la consulta(Verifica si en el carrito del usuario existe un producto de este tipo)
                var product_in_carshop = await entity_carshop.FirstOrDefaultAsync().ConfigureAwait(false);
                //El producto no se encuentra en el carrito o no tiene el agregado.
                if (product_in_carshop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotHaveAggregate;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotHaveAggregate);
                    return serviceResponse;
                }
                //Obtiene el agregado y su cantidad actual
                var cantAggregate = product_in_carshop.ProductCombined.CantAggregates
                .Where(c => c.Aggregate.Id == modifyAggregateProduct.AggregateId).FirstOrDefault();
                //Actualiza la cantidad del agregado.
                cantAggregate.Quantity += modifyAggregateProduct.Quantity;
                this._serviceGenericCantAggregateHelper.UpdateEntity(cantAggregate);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync().ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Agrega un nuevo agregado a un producto que está en el carrito de compras.
        /// </summary>
        /// <param name="addAggregateInProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddAggregateInProductAsync(AddAggregateInProductDto addAggregateInProduct)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                if (addAggregateInProduct == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica si el agregado está disponible
                var aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(c => c.Id == addAggregateInProduct.AggregateId && c.IsAvailabe == true)
                .ConfigureAwait(false);
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotIsAvailable;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotIsAvailable);
                    return serviceResponse;
                }
                //Verifica si el producto está en el carrito del usuario
                var context_cart_shop = this._serviceGenericCartShopHelper._context;
                //Define una entidad consultable
                IQueryable<CartShop> entity_carshop = context_cart_shop.AsQueryable();
                //Agrega los Include y ThenInclude necesarios
                entity_carshop = entity_carshop.Include(c => c.User)
                .Include(c => c.ProductCombined.Product.SubCategories).ThenInclude(c => c.Product)
                .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate)
                .Include(c => c.ProductCombined.SubCategory)
                .Include(c => c.ProductCombined.Product.Category);
                //Agrega los filtros restantes
                entity_carshop = entity_carshop
                .Where(c => c.User == user)
                .Where(c => c.ProductCombined.Id == addAggregateInProduct.ProductCombinedId);
                //Ejecuta la consulta(Verifica si en el carrito del usuario existe un producto de este tipo)
                var product_in_carshop = await entity_carshop.FirstOrDefaultAsync().ConfigureAwait(false);
                //El producto no se encuentra en el carrito.
                if (product_in_carshop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotExistInCarShop;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotExistInCarShop);
                    return serviceResponse;
                }
                //Verifica si el producto tiene el agregado o no, en caso + solo se actualizan las cantidades.
                //En caso - se verifica si el nuevo producto creado se encuentra en la base de datos para solo actualizar las cantidades.
                if(product_in_carshop.ProductCombined.CantAggregates.Any(c => c.Aggregate.Id == aggregate.Id) == true)
                {
                    var update_aggregate = product_in_carshop.ProductCombined.CantAggregates
                    .Where(c => c.Aggregate.Id == aggregate.Id).FirstOrDefault();
                    update_aggregate.Quantity += addAggregateInProduct.Quantity;
                    this._serviceGenericCantAggregateHelper.UpdateEntity(update_aggregate);
                    await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync().ConfigureAwait(false);
                }
                else
                {
                    entity_carshop = context_cart_shop.AsQueryable();
                    entity_carshop = entity_carshop.Include(c => c.User)
                    .Include(c => c.ProductCombined.Product.SubCategories).ThenInclude(c => c.Product)
                    .Include(c => c.ProductCombined.SubCategory)
                    .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate);
                    //Verifica si el producto tiene otros agregados
                    if (product_in_carshop.ProductCombined.CantAggregates.Any())
                    {
                        //Crea la consulta con los posibles agregados, para luego verificar si el producto
                        //combinado los posee
                        foreach (CantAggregate cantAggregate in product_in_carshop.ProductCombined.CantAggregates)
                        {
                            entity_carshop = entity_carshop.Where(c => c.ProductCombined.CantAggregates
                           .Select(c => c.Aggregate).Contains(cantAggregate.Aggregate) == true);
                        }
                    }
                    //Agrega el nuevo agregado
                    entity_carshop = entity_carshop.Where(c => c.ProductCombined.CantAggregates
                    .Select(c => c.Aggregate).Contains(aggregate) == true);
                    entity_carshop = entity_carshop
                    .Where(c => c.User == user)
                    .Where(c => c.ProductCombined.Product == product_in_carshop.ProductCombined.Product)
                    .Where(c => c.ProductCombined.CantAggregates.Count == product_in_carshop.ProductCombined.CantAggregates.Count + 1)
                    .Where(c => c.ProductCombined.SubCategory == product_in_carshop.ProductCombined.SubCategory);
                    //Ejecuta la consulta(Verifica si en el carrito del usuario existe un producto de este tipo)
                    var exist_product_in_carshop = await entity_carshop.SingleOrDefaultAsync().ConfigureAwait(false);
                    //El producto no existe por lo que solo agrega el nuevo agregado y actualiza el carrito.
                    if (exist_product_in_carshop == null)
                    {
                        //Guarda el nuevo agregado
                        var cantAggregate = new CantAggregate
                        {
                            Aggregate = aggregate,
                            Price = aggregate.Price,
                            ProductCombined = product_in_carshop.ProductCombined,
                            Quantity = addAggregateInProduct.Quantity,
                        };
                        await this._serviceGenericCantAggregateHelper.AddEntityAsync(cantAggregate).ConfigureAwait(false);
                        //Guarda los cambios en la base de datos.
                        await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync().ConfigureAwait(false);
                        //Actualiza el carrito, agrega el nuevo agregado.
                        this._serviceGenericCartShopHelper.UpdateEntity(product_in_carshop);
                        //Guarda los cambios en la base de datos.
                        await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                    }
                    //El producto existe por lo que solo actualiza las cantidades y elimina el actual.
                    else
                    {
                        //Actualiza las cantidades
                        exist_product_in_carshop.ProductCombined.Quantity += product_in_carshop.ProductCombined.Quantity;
                        //Verifica si el producto tiene agregados
                        if (exist_product_in_carshop.ProductCombined.CantAggregates.Any())
                        {
                            List<CantAggregate> updateAggregate = new List<CantAggregate>();
                            foreach (CantAggregate cantAggregate in product_in_carshop.ProductCombined.CantAggregates)
                            {
                                var update = exist_product_in_carshop.ProductCombined.CantAggregates
                                .FirstOrDefault(c => c.Aggregate.Id == cantAggregate.Aggregate.Id);
                                update.Quantity += cantAggregate.Quantity;
                                updateAggregate.Add(update);
                            }
                            //Actualiza la cantidad del nuevo agregado
                            var new_aggregate = exist_product_in_carshop.ProductCombined.CantAggregates
                            .FirstOrDefault(c => c.Aggregate.Id == aggregate.Id);
                            new_aggregate.Quantity += addAggregateInProduct.Quantity;
                            updateAggregate.Add(new_aggregate);
                            //Manda a actualizar la cantidad de agregados.
                            this._serviceGenericCantAggregateHelper.UpdateRangeEntity(updateAggregate);
                            //Guarda los cambios en la base de datos.
                            await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync();
                            //Elimina las referencias a la cantidad de agregados del producto actual.
                            this._serviceGenericCantAggregateHelper
                            .RemoveRangeEntity(product_in_carshop.ProductCombined.CantAggregates.ToList());
                            //Guarda los cambios en la base de datos.
                            await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync().ConfigureAwait(false);
                        }
                        //Elimina el producto combinado
                        this._serviceGenericProductCombinedHelper.RemoveEntity(product_in_carshop.ProductCombined);
                        //Guarda los cambios en la base de datos.
                        await this._serviceGenericProductCombinedHelper
                        .SaveChangesBDAsync().ConfigureAwait(false);
                        //Elimina la relación del producto combinado con el carrito
                        this._serviceGenericCartShopHelper.RemoveEntity(product_in_carshop);
                        //Guarda los cambios en la base de datos
                        await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                        //Actualiza el producto existente
                        this._serviceGenericCartShopHelper.UpdateEntity(exist_product_in_carshop);
                        //Guarda los cambios en la base de datos
                        await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                    }
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Elimina un agregado de un producto del carrito de compras.
        /// </summary>
        /// <param name="ProductCombinedId"></param>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> RemoveAggregateInProductOfCartShopAsync(int ProductCombinedId, int AggregateId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Verifica si el agregado está disponible
                var aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(c => c.Id == AggregateId && c.IsAvailabe == true)
                .ConfigureAwait(false);
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotIsAvailable;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotIsAvailable);
                    return serviceResponse;
                }
                //Verifica si el producto está en el carrito.
                var context_cart_shop = this._serviceGenericCartShopHelper._context;
                //Define una entidad consultable
                IQueryable<CartShop> entity_carshop = context_cart_shop.AsQueryable();
                //Agrega los Include y ThenInclude necesarios
                entity_carshop = entity_carshop.Include(c => c.User)
                .Include(c => c.ProductCombined.Product.SubCategories).ThenInclude(c => c.Product)
                .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate)
                .Include(c => c.ProductCombined.SubCategory)
                .Include(c => c.ProductCombined.Product.Category);
                //Agrega los filtros restantes
                entity_carshop = entity_carshop
                .Where(c => c.User == user)
                .Where(c => c.ProductCombined.Id == ProductCombinedId);
                //Ejecuta la consulta(Verifica si en el carrito del usuario existe un producto de este tipo)
                var product_in_carshop = await entity_carshop.FirstOrDefaultAsync().ConfigureAwait(false);
                //El producto no se encuentra en el carrito.
                if (product_in_carshop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotExistInCarShop;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotExistInCarShop);
                    return serviceResponse;
                }
                //Verifica si el producto no tiene el agregado
                if (product_in_carshop.ProductCombined.CantAggregates.Any(c => c.Aggregate.Id == aggregate.Id) == false)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotHaveAggregate;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotHaveAggregate);
                    return serviceResponse;
                }
                else
                {
                    //Obtiene los agregados actuales del producto y elimina y el seleccionado.
                    var deleteaggregate = product_in_carshop.ProductCombined.CantAggregates
                    .FirstOrDefault(c => c.Aggregate.Id == aggregate.Id);
                    product_in_carshop.ProductCombined.CantAggregates.Remove(deleteaggregate);
                    entity_carshop = context_cart_shop.AsQueryable();
                    entity_carshop = entity_carshop.Include(c => c.User)
                    .Include(c => c.ProductCombined.Product.SubCategories).ThenInclude(c => c.Product)
                    .Include(c => c.ProductCombined.SubCategory)
                    .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate);
                    //Verifica si el producto tiene otros agregados
                    if (product_in_carshop.ProductCombined.CantAggregates.Any())
                    {
                        //Crea la consulta con los posibles agregados, para luego verificar si el producto
                        //combinado los posee
                        foreach (CantAggregate cantAggregate in product_in_carshop.ProductCombined.CantAggregates)
                        {
                            entity_carshop = entity_carshop.Where(c => c.ProductCombined.CantAggregates
                           .Select(c => c.Aggregate).Contains(cantAggregate.Aggregate) == true);
                        }
                    }
                    //Agrega el nuevo agregado
                    entity_carshop = entity_carshop.Where(c => c.ProductCombined.CantAggregates
                    .Select(c => c.Aggregate).Contains(aggregate) == true);
                    entity_carshop = entity_carshop
                    .Where(c => c.User == user)
                    .Where(c => c.ProductCombined.Product == product_in_carshop.ProductCombined.Product)
                    .Where(c => c.ProductCombined.CantAggregates.Count == product_in_carshop.ProductCombined.CantAggregates.Count)
                    .Where(c => c.ProductCombined.SubCategory == product_in_carshop.ProductCombined.SubCategory);
                    //Ejecuta la consulta(Verifica si en el carrito del usuario existe un producto de este tipo)
                    var exist_product_in_carshop = await entity_carshop.SingleOrDefaultAsync().ConfigureAwait(false);
                    //El producto no existe por lo que solo elimina el agregado de referencia de la base de datos.
                    if (exist_product_in_carshop == null)
                    {
                        this._serviceGenericCantAggregateHelper.RemoveEntity(deleteaggregate);
                        await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync().ConfigureAwait(false);
                    }
                    //El producto existe por lo que solo actualiza las cantidades y elimina el actual.
                    else
                    {
                        //Actualiza las cantidades
                        exist_product_in_carshop.ProductCombined.Quantity += product_in_carshop.ProductCombined.Quantity;
                        //Verifica si el producto tiene agregados
                        if (exist_product_in_carshop.ProductCombined.CantAggregates.Any())
                        {
                            List<CantAggregate> updateAggregate = new List<CantAggregate>();
                            foreach (CantAggregate cantAggregate in product_in_carshop.ProductCombined.CantAggregates)
                            {
                                var update = exist_product_in_carshop.ProductCombined.CantAggregates
                                .FirstOrDefault(c => c.Aggregate.Id == cantAggregate.Aggregate.Id);
                                update.Quantity += cantAggregate.Quantity;
                                updateAggregate.Add(update);
                            }
                            //Manda a actualizar la cantidad de agregados.
                            this._serviceGenericCantAggregateHelper.UpdateRangeEntity(updateAggregate);
                            //Guarda los cambios en la base de datos.
                            await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync();
                            //Elimina las referencias a la cantidad de agregados del producto actual.
                            this._serviceGenericCantAggregateHelper
                            .RemoveRangeEntity(product_in_carshop.ProductCombined.CantAggregates.ToList());
                            //Elimina el agregado de referencia.
                            this._serviceGenericCantAggregateHelper.RemoveEntity(deleteaggregate);
                            //Guarda los cambios en la base de datos.
                            await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync().ConfigureAwait(false);
                        }
                        //Elimina el producto combinado
                        this._serviceGenericProductCombinedHelper.RemoveEntity(product_in_carshop.ProductCombined);
                        //Guarda los cambios en la base de datos.
                        await this._serviceGenericProductCombinedHelper
                        .SaveChangesBDAsync().ConfigureAwait(false);
                        //Elimina la relación del producto combinado con el carrito
                        this._serviceGenericCartShopHelper.RemoveEntity(product_in_carshop);
                        //Guarda los cambios en la base de datos
                        await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                        //Actualiza el producto existente
                        this._serviceGenericCartShopHelper.UpdateEntity(exist_product_in_carshop);
                        //Guarda los cambios en la base de datos
                        await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                    }
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Elimina todos los productos del carrito de compras del usuario.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> RemoveAllCarShopAsync()
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                var all_carshop = await this._serviceGenericCartShopHelper
                .WhereListEntityAsync(c => c.User == user,
                c => c.ProductCombined.CantAggregates, c => c.User, c => c.ProductCombined)
                .ConfigureAwait(false);
                if (all_carshop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CarShopNotProducts;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CarShopNotProducts);
                    return serviceResponse;
                }
                //Elimina la cantidad de agregados que tiene cada producto combinado como referencia
                foreach(CartShop carshop in all_carshop)
                {
                    var all_cantaggregate_of_productcombined = await this._serviceGenericCantAggregateHelper
                   .WhereListEntityAsync(c => c.ProductCombined == carshop.ProductCombined, c => c.ProductCombined)
                   .ConfigureAwait(false);
                    if (all_cantaggregate_of_productcombined.Any())
                    {
                        //Elimina los agregados
                        this._serviceGenericCantAggregateHelper.RemoveRangeEntity(all_cantaggregate_of_productcombined);
                        await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync().ConfigureAwait(false);
                    }
                    //Elimina el producto combinado
                    this._serviceGenericProductCombinedHelper.RemoveEntity(carshop.ProductCombined);
                    await this._serviceGenericProductCombinedHelper
                    .SaveChangesBDAsync().ConfigureAwait(false);
                    //Elimina la relación del producto combinado con el carrito
                    this._serviceGenericCartShopHelper.RemoveEntity(carshop);
                    await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ExceptionDeleteEntity;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.ExceptionDeleteEntity);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Actualiza el carrito del usuario de forma general.
        /// </summary>
        /// <param name="updateCartShop"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdateCartShopAsync(UpdateCartShopDto updateCartShop)
        {
           ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
           try
           {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                if (updateCartShop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica si el producto combinado está disponible.
                //Obtiene el contexto
                var context_cart_shop = this._serviceGenericCartShopHelper._context;
                //Define una entidad consultable
                IQueryable<CartShop> entity_carshop = context_cart_shop.AsQueryable();
                //Agrega los Include y ThenInclude necesarios
                entity_carshop = entity_carshop.Include(c => c.User)
                .Include(c => c.ProductCombined.Product.Category)
                .Include(c => c.ProductCombined.Product.SubCategories)
                .Include(c => c.ProductCombined.SubCategory.Product.Category)
                .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate);
                //Agrega los filtros restantes
                entity_carshop = entity_carshop
                .Where(c => c.User == user)
                .Where(c => c.ProductCombined.Id == updateCartShop.ProductCombinedId);
                //Ejecuta la consulta(Verifica si en el carrito del usuario existe un producto de este tipo)
                var product_in_carshop = await entity_carshop.SingleOrDefaultAsync().ConfigureAwait(false);
                //Verifica si el producto no estaba en el carrito del usuario para agregarlo.
                if (product_in_carshop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotExistInCarShop;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotExistInCarShop);
                    return serviceResponse;
                }
                //Cambiar la subcategoria
                if(updateCartShop.SubCategoryId != null)
                {
                    //Verifica que la subcategoria exista y este disponible
                    var subcategory = await this._serviceGenericSubCategoryHelper
                    .GetLoadAsync(c => c.Id == updateCartShop.SubCategoryId && c.IsAvailable == true, c => c.Product.SubCategories)
                    .ConfigureAwait(false);
                    if (subcategory == null)
                    {
                        serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SubCategoryNotIsAvailable;
                        serviceResponse.Data = false;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile
                        .GetValueResourceString(GetValueResourceFile.KeyResource.SubCategoryNotIsAvailable);
                        return serviceResponse;
                    }
                    //Verifica si la subcategoria es valida para el producto base
                    if (product_in_carshop.ProductCombined.Product.SubCategories.Any(c => c.Id == subcategory.Id) == false)
                    {
                        serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SubCategoryNotIsProduct;
                        serviceResponse.Data = false;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile
                        .GetValueResourceString(GetValueResourceFile.KeyResource.SubCategoryNotIsProduct);
                        return serviceResponse;
                    }
                    product_in_carshop.ProductCombined.SubCategory = subcategory;
                }
                //Cambiar la cantidad del producto
                if(updateCartShop.QuantityProduct != null)
                {
                    //Verifica que la cantidad se mayor que 1.
                    if (updateCartShop.QuantityProduct < 1)
                    {
                        serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                        serviceResponse.Data = false;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile
                        .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                        return serviceResponse;
                    }
                    product_in_carshop.ProductCombined.Quantity = (int) updateCartShop.QuantityProduct;
                }
                //Actualizar agregados
                if (updateCartShop.CantAggregates != null)
                {
                    //Verifica que los agregados solicitados esten disponibles.
                    List<Models.Entities.CantAggregate> cantAggregates = new List<Models.Entities.CantAggregate>();
                    if (product_in_carshop.ProductCombined.Product.SupportAggregate == true && updateCartShop.CantAggregates != null)
                    {
                       foreach (string key in updateCartShop.CantAggregates.Keys)
                        {
                            int parse_key;
                            if (!int.TryParse(key, out parse_key))
                            {
                                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.FormatAggregateNotSupport;
                                serviceResponse.Data = false;
                                serviceResponse.Success = false;
                                serviceResponse.Message = GetValueResourceFile
                                .GetValueResourceString(GetValueResourceFile.KeyResource.FormatAggregateNotSupport);
                                return serviceResponse;
                            }
                            if (parse_key < 1)
                            {
                                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                                serviceResponse.Data = false;
                                serviceResponse.Success = false;
                                serviceResponse.Message = GetValueResourceFile
                                .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                                return serviceResponse;
                            }
                            var quantity = updateCartShop.CantAggregates.GetValueOrDefault(key);
                            if (quantity < 1)
                            {
                                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                                serviceResponse.Data = false;
                                serviceResponse.Success = false;
                                serviceResponse.Message = GetValueResourceFile
                                .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                                return serviceResponse;
                            }
                            var aggregate = await this._serviceGenericAggregateHelper
                            .WhereFirstEntityAsync(c => c.Id == parse_key && c.IsAvailabe == true)
                            .ConfigureAwait(false);
                            if (aggregate == null)
                            {
                                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotIsAvailable;
                                serviceResponse.Data = false;
                                serviceResponse.Success = false;
                                serviceResponse.Message = GetValueResourceFile
                                .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotIsAvailable);
                                return serviceResponse;
                            }
                            //TODO: Verifica que la cantidad de agregados solicitados este disponible
                            var new_aggregate = new Models.Entities.CantAggregate
                            {
                                Aggregate = aggregate,
                                Price = aggregate.Price,
                                Quantity = quantity,
                            };
                            cantAggregates.Add(new_aggregate);
                        }
                    }
                    foreach (CantAggregate cantaggregate in product_in_carshop.ProductCombined.CantAggregates)
                    {
                        var update_cantaggregate = cantAggregates.FirstOrDefault(c => c.Aggregate.Id == cantaggregate.Aggregate.Id);
                        cantaggregate.Quantity += update_cantaggregate.Quantity;
                    }
                }
                //Verifica si el producto resultante ya se encuentra en la base de datos.
                entity_carshop = context_cart_shop.AsQueryable();
                //Agrega los Include y ThenInclude necesarios
                entity_carshop = entity_carshop.Include(c => c.User)
                .Include(c => c.ProductCombined.Product.Category)
                .Include(c => c.ProductCombined.Product.SubCategories)
                .Include(c => c.ProductCombined.SubCategory.Product.Category)
                .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate);
                //Verifica si hay agregados para agregar el filtro para los agregados.
                if (product_in_carshop.ProductCombined.CantAggregates.Any())
                {
                    //Crea la consulta con los posibles agregados, para luego verificar si el producto
                    //Combinado los posee
                    foreach (CantAggregate cantAggregate in product_in_carshop.ProductCombined.CantAggregates)
                    {
                        entity_carshop = entity_carshop.Where(c => c.ProductCombined.CantAggregates
                       .Select(c => c.Aggregate).Contains(cantAggregate.Aggregate) == true);
                    }
                }
                //Agrega los filtros restantes
                entity_carshop = entity_carshop
                .Where(c => c.User == user)
                .Where(c => c.ProductCombined.Product == product_in_carshop.ProductCombined.Product)
                .Where(c => c.ProductCombined.CantAggregates.Count == product_in_carshop.ProductCombined.CantAggregates.Count)
                .Where(c => c.ProductCombined.SubCategory == product_in_carshop.ProductCombined.SubCategory);
                //Ejecuta la consulta(Verifica si en el carrito del usuario existe un producto de este tipo)
                var temp_product_in_carshop = await entity_carshop.SingleOrDefaultAsync().ConfigureAwait(false);
                //Verifica si este tipo de producto no estaba en el carrito del usuario para agregarlo.
                if (temp_product_in_carshop == null)
                {
                    //Actualiza el producto
                    this._serviceGenericCartShopHelper.UpdateEntity(temp_product_in_carshop);
                    //Actualiza la base de datos.
                    await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                }
                //Si el usuario tiene ese producto combinado solo se actualizan las cantidades.
                else
                {
                    //Verifica si es el mismo producto, esto puede suceder si solo se cambia la cantidad.
                    if (temp_product_in_carshop.ProductCombined.Id == product_in_carshop.ProductCombined.Id)
                    {
                        //Actualiza el producto
                        this._serviceGenericCartShopHelper.UpdateEntity(temp_product_in_carshop);
                        //Actualiza la base de datos.
                        await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        List<CantAggregate> UpdateCanAggregate = new List<CantAggregate>();
                        foreach (CantAggregate cantaggregate in product_in_carshop.ProductCombined.CantAggregates)
                        {
                            var update_cantaggregate = temp_product_in_carshop.ProductCombined
                            .CantAggregates.FirstOrDefault(c => c.Aggregate.Id == cantaggregate.Aggregate.Id);
                            cantaggregate.Quantity += update_cantaggregate.Quantity;
                            UpdateCanAggregate.Add(cantaggregate);
                        }
                        //Actualiza las cantidades de los agregados
                        this._serviceGenericCantAggregateHelper.UpdateRangeEntity(UpdateCanAggregate);
                        await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync().ConfigureAwait(false);
                        //Actualiza la cantidad del producto
                        temp_product_in_carshop.ProductCombined.Quantity += product_in_carshop.ProductCombined.Quantity;
                        this._serviceGenericCartShopHelper.UpdateEntity(temp_product_in_carshop);
                        //Guarda los cambios en la base de datos
                        await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                    }
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
           }
           catch 
           {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
           }
        }
    }
}
