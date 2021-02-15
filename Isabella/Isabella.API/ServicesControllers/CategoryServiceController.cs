namespace Isabella.API.ServicesControllers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Common.RepositorysDtos;
    using Common;
    using Common.Dtos.Category;
    using Common.Extras;
    using Models.Entities;
    using Helpers;
    using Helpers.RepositoryHelpers;

    /// <summary>
    /// Servicio para el controlador de las categorias de los productos.
    /// </summary>
    public class CategoryServiceController : ICategoryRepositoryDto
    {
        private readonly ServiceGenericHelper<Category> _serviceGenericCategoryHelper;

        /// <summary>
        /// Categorias
        /// </summary>
        /// <param name="serviceGenericCategoryHelper"></param>
        public CategoryServiceController(ServiceGenericHelper<Category> serviceGenericCategoryHelper)
        {
            this._serviceGenericCategoryHelper = serviceGenericCategoryHelper;
        }

        /// <summary>
        /// Agrega una nueva categoria.
        /// </summary> 
        /// <param name="addCategory"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddCategoryAsync(AddCategoryDto addCategory)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (addCategory == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica que la categoria es valida
                var category = await this._serviceGenericCategoryHelper
                .WhereSingleEntityAsync(c => c.Name == addCategory.Name)
                .ConfigureAwait(false);
                if (category != null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.CategoryExist;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CategoryExist);
                    return serviceResponse;
                }
                var new_category = new Category
                {
                   Name = addCategory.Name,
                };
                await this._serviceGenericCategoryHelper
                .AddEntityAsync(new_category)
                .ConfigureAwait(false);
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Devuelve las categorias disponibles.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetCategoryDto>>> GetAllCategoryAsync()
        {
            ServiceResponse<List<GetCategoryDto>> serviceResponse = new ServiceResponse<List<GetCategoryDto>>();
            try
            {
                var category = await this._serviceGenericCategoryHelper
                .GetLoadAsync()
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.CategoryNotAllFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CategoryNotAllFound);
                    return serviceResponse;
                }
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = category.Select(c => new GetCategoryDto 
                { 
                   Id = c.Id,
                   Name = c.Name
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Devuelve una categoria por su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetCategoryDto>> GetCategoryForIdAsync(int Id)
        {
            ServiceResponse<GetCategoryDto> serviceResponse = new ServiceResponse<GetCategoryDto>();
            try
            {
                //Verifica que la categoria es valida
                var category = await this._serviceGenericCategoryHelper
                .WhereFirstEntityAsync(c => c.Id == Id)
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.CategoryNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CategoryNotFound);
                    return serviceResponse;
                }
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Devuelve una categoria dado su nombre.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetCategoryDto>> GetCategoryForNameAsync(string Name)
        {
            ServiceResponse<GetCategoryDto> serviceResponse = new ServiceResponse<GetCategoryDto>();
            try
            {
                //Verifica que la categoria es valida
                var category = await this._serviceGenericCategoryHelper
                .WhereSingleEntityAsync(c => c.Name == Name)
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.CategoryNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CategoryNotFound);
                    return serviceResponse;
                }
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }
    }
}
