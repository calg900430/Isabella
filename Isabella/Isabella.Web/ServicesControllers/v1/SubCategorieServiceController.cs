namespace Isabella.Web.ServicesControllers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Common.RepositorysDtos;
    using Common;
    using Common.Dtos.SubCategorie;
    using Models.Entities;
    using Helpers;
    using Helpers.RepositoryHelpers;
    using Resources;
    using Isabella.Common.Dtos.v2.SubCategorie;

    /// <summary>
    /// Servicio para el controlador de las subcategorias.
    /// </summary>
    public class SubCategorieServiceController : ISubCategoryRepositoryDto
    {
        private readonly ServiceGenericHelper<SubCategorie> _serviceGenericSubCategorieHelper;
        private readonly ServiceGenericHelper<Product> _serviceGenericProductHelper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceGenericSubCategoryHelper"></param>
        /// <param name="serviceGenericProductHelper"></param>
        public SubCategorieServiceController(ServiceGenericHelper<SubCategorie> serviceGenericSubCategoryHelper, 
        ServiceGenericHelper<Product> serviceGenericProductHelper)
        {
            this._serviceGenericSubCategorieHelper = serviceGenericSubCategoryHelper;
            this._serviceGenericProductHelper = serviceGenericProductHelper;
        }

        /// <summary>
        /// Agrega una nueva subcategoria a un producto.
        /// </summary>
        /// <param name="addSubCategoryProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddSubCategorieAsync(AddSubCategorieDto addSubCategoryProduct)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (addSubCategoryProduct == null)
                {
                    serviceResponse.Code = (int) GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (addSubCategoryProduct.Price < 1)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Verifica que el producto existe.
                var product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Id == addSubCategoryProduct.ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                var new_subcategory = new SubCategorie
                {
                    Product = product,
                    Name = addSubCategoryProduct.Name,
                    Price = addSubCategoryProduct.Price,
                    IsAvailable = addSubCategoryProduct.IsAvailable,
                    Description = addSubCategoryProduct.Description,
                };
                await this._serviceGenericSubCategorieHelper
                .AddEntityAsync(new_subcategory)
                .ConfigureAwait(false);
                await this._serviceGenericSubCategorieHelper
                .SaveChangesBDAsync()
                .ConfigureAwait(false);
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
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Elimina una subcategoria
        /// </summary>
        /// <param name="SubCategoryId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> DeleteSubCategorieAsync(int SubCategoryId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var category = await this._serviceGenericSubCategorieHelper
                .GetLoadAsync(c => c.Id == SubCategoryId)
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SubCategoryNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.SubCategoryNotFound);
                    return serviceResponse;
                }
                //Elimina la categoria
                this._serviceGenericSubCategorieHelper.RemoveEntity(category);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericSubCategorieHelper
                .SaveChangesBDAsync().ConfigureAwait(false);
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
        /// Obtiene todas las subcategorias.
        /// </summary>
        public async Task<ServiceResponse<List<GetSubCategorieDto>>> GetAllSubCategorieAsync()
        {
            ServiceResponse<List<GetSubCategorieDto>> serviceResponse = new ServiceResponse<List<GetSubCategorieDto>>();
            try
            {
                var all_subcategories = await this._serviceGenericSubCategorieHelper
                .GetLoadAsync(c => c.Product)
                .ConfigureAwait(false);
                if (all_subcategories == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SubCategoryNotAllFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.SubCategoryNotAllFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = all_subcategories.Select(c => new GetSubCategorieDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Price = c.Price, 
                    ProductId = c.Product.Id,
                    IsAvailable = c.IsAvailable,
                    Description = c.Description,
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todas las subcategorias.
        /// </summary>
        public async Task<ServiceResponse<List<GetSubCategoriev2Dto>>> GetAllSubCategorieWithProductAsync()
        {
            ServiceResponse<List<GetSubCategoriev2Dto>> serviceResponse = new ServiceResponse<List<GetSubCategoriev2Dto>>();
            try
            {
                var all_subcategories = await this._serviceGenericSubCategorieHelper
                .GetLoadAsync(c => c.Product)
                .ConfigureAwait(false);
                if (all_subcategories == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SubCategoryNotAllFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.SubCategoryNotAllFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = all_subcategories.Select(c => new GetSubCategoriev2Dto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Price = c.Price,
                    ProductId = c.Product.Id,
                    IsAvailable = c.IsAvailable,
                    Description = c.Description,
                    GetProductDto = new Common.Dtos.Product.GetProductDto
                    {
                        Name = c.Product.Name,
                        Id = c.Product.Id,
                        Description = c.Product.Description,
                        Price = c.Price,
                    }
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todas las subcategorias que están disponibles.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetSubCategorieDto>>> GetAllSubCategoryIsAvailableAsync()
        {
            ServiceResponse<List<GetSubCategorieDto>> serviceResponse = new ServiceResponse<List<GetSubCategorieDto>>();
            try
            {
                var all_subcategories = await this._serviceGenericSubCategorieHelper
                .WhereListEntityAsync(c => c.IsAvailable == true, c => c.Product)
                .ConfigureAwait(false);
                if (all_subcategories == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SubCategoryNotAllFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.SubCategoryNotAllFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = all_subcategories.Select(c => new GetSubCategorieDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Price = c.Price,
                    ProductId = c.Product.Id,
                    IsAvailable = c.IsAvailable,
                    Description = c.Description,
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todas las subcategorias que no están disponible
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetSubCategorieDto>>> GetAllSubCategoryIsNotAvailableAsync()
        {
            ServiceResponse<List<GetSubCategorieDto>> serviceResponse = new ServiceResponse<List<GetSubCategorieDto>>();
            try
            {
                var all_subcategories = await this._serviceGenericSubCategorieHelper
                .WhereListEntityAsync(c => c.IsAvailable == false, c => c.Product)
                .ConfigureAwait(false);
                if (all_subcategories == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SubCategoryNotAllFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.SubCategoryNotAllFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = all_subcategories.Select(c => new GetSubCategorieDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Price = c.Price,
                    ProductId = c.Product.Id,
                    IsAvailable = c.IsAvailable,
                    Description = c.Description,
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene una subcategoria por su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetSubCategorieDto>> GetSubCategoryForIdAsync(int Id)
        {
            ServiceResponse<GetSubCategorieDto> serviceResponse = new ServiceResponse<GetSubCategorieDto>();
            try
            {
                //Verifica que la categoria es valida
                var subcategory = await this._serviceGenericSubCategorieHelper
                .GetLoadAsync(c => c.Id == Id, c => c.Product)
                .ConfigureAwait(false);
                if (subcategory == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SubCategoryNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.SubCategoryNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetSubCategorieDto
                {
                    Id = subcategory.Id,
                    Name = subcategory.Name,
                    Price = subcategory.Price,
                    ProductId = subcategory.Product.Id,
                    IsAvailable = subcategory.IsAvailable,
                    Description = subcategory.Description,
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene una subcategoria por su nombre.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetSubCategorieDto>> GetSubCategoryForNameAsync(string Name)
        {
            ServiceResponse<GetSubCategorieDto> serviceResponse = new ServiceResponse<GetSubCategorieDto>();
            try
            {
                //Verifica que la categoria es valida
                var subcategory = await this._serviceGenericSubCategorieHelper
                .WhereSingleEntityAsync(c => c.Name == Name, c => c.Product)
                .ConfigureAwait(false);
                if (subcategory == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SubCategoryNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.SubCategoryNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetSubCategorieDto
                {
                    Id = subcategory.Id,
                    Name = subcategory.Name,
                    Price = subcategory.Price,
                    ProductId = subcategory.Product.Id,
                    IsAvailable = subcategory.IsAvailable,
                    Description = subcategory.Description,
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Actualiza una subcategoria.
        /// </summary>
        /// <param name="updateSubCategoryDto"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdateSubCategoryAsync(UpdateSubCategorieDto updateSubCategoryDto)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que la categoria es valida
                var subcategory = await this._serviceGenericSubCategorieHelper
                .WhereFirstEntityAsync(c => c.Id == updateSubCategoryDto.Id, c => c.Product)
                .ConfigureAwait(false);
                if (subcategory == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SubCategoryNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.SubCategoryNotFound);
                    return serviceResponse;
                }
                //Verifica que el producto existe.
                var product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Id == updateSubCategoryDto.ProductId)
                .ConfigureAwait(false);
                if(product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                if(updateSubCategoryDto.Price != null)
                {
                    if (updateSubCategoryDto.Price < 1)
                    {
                        serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                        serviceResponse.Data = false;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile
                        .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                        return serviceResponse;
                    }
                    subcategory.Price = (decimal)updateSubCategoryDto.Price; 
                }
                if(updateSubCategoryDto.ProductId != null)
                subcategory.Product = product;
                if(updateSubCategoryDto.Name != null)
                subcategory.Name = updateSubCategoryDto.Name;
                if (updateSubCategoryDto.IsAvailable != null)
                subcategory.IsAvailable = (bool) updateSubCategoryDto.IsAvailable;
                if (updateSubCategoryDto.Description != null)
                subcategory.Description = updateSubCategoryDto.Description;
                this._serviceGenericSubCategorieHelper.UpdateEntity(subcategory);
                await this._serviceGenericSubCategorieHelper.SaveChangesBDAsync().ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }
    }
}
