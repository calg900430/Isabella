namespace Isabella.Web.ServicesControllers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    using Common.RepositorysDtos;
    using Common;
    using Common.Dtos.Categorie;
    using Models.Entities;
    using Resources;
    using Helpers;
    using Helpers.RepositoryHelpers;
    
    /// <summary>
    /// Servicio para el controlador de las categorias de los productos.
    /// </summary>
    public class CategorieServiceController : ICategoryRepositoryDto
    {
        private readonly ServiceGenericHelper<Category> _serviceGenericCategoryHelper;
        private readonly ServiceGenericHelper<Product> _serviceGenericProductHelper;

        /// <summary>
        /// Categorias
        /// </summary>
        /// <param name="serviceGenericCategoryHelper"></param>
        /// <param name="serviceGenericProductHelper"></param>
        public CategorieServiceController(ServiceGenericHelper<Category> serviceGenericCategoryHelper,
        ServiceGenericHelper<Product> serviceGenericProductHelper)
        {
            this._serviceGenericCategoryHelper = serviceGenericCategoryHelper;
            this._serviceGenericProductHelper = serviceGenericProductHelper;
        }

        /// <summary>
        /// Agrega una nueva categoria.
        /// </summary> 
        /// <param name="addCategory"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddCategoryAsync(AddCategorieDto addCategory)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (addCategory == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CategoryExist;
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
                //Guarda los cambios en la base de datos.
                await this._serviceGenericCategoryHelper
                .SaveChangesBDAsync().ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Devuelve las categorias de los productos que esten disponibles. 
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetCategorieDto>>> GetAllCategoryIsProductIsAvailableAsync()
        {
            ServiceResponse<List<GetCategorieDto>> serviceResponse = new ServiceResponse<List<GetCategorieDto>>();
            try
            {
                //Obtiene el contexto de datos.
                var _context = this._serviceGenericProductHelper._context;
                IQueryable<Product> query = _context.AsQueryable();
                //Obtiene todas las categorias disponibles 
                var all_categories_availables = query
                .Include(c => c.Category)
                .Where(c => c.IsAvailabe == true)
                .Select(c => c.Category)
                .ToHashSet();
                if (!all_categories_availables.Any())
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductAllNotIsAvailable;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductAllNotIsAvailable);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                //Ordena las categorias por Id
                serviceResponse.Data = all_categories_availables.Select(c => new GetCategorieDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).OrderBy(c => c.Id).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
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
        /// Elimina una categoria
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> DeleteCategoryAsync(int CategoryId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var category = await this._serviceGenericCategoryHelper
                .GetLoadAsync(c => c.Id == CategoryId)
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CategoryNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CategoryNotFound);
                    return serviceResponse;
                }
                //Elimina la categoria
                this._serviceGenericCategoryHelper.RemoveEntity(category);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericCategoryHelper
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
        /// Devuelve las categorias disponibles.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetCategorieDto>>> GetAllCategoryAsync()
        {
            ServiceResponse<List<GetCategorieDto>> serviceResponse = new ServiceResponse<List<GetCategorieDto>>();
            try
            {
                var category = await this._serviceGenericCategoryHelper
                .GetLoadAsync()
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CategoryNotAllFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CategoryNotAllFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = category.Select(c => new GetCategorieDto 
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
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
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
        public async Task<ServiceResponse<GetCategorieDto>> GetCategoryForIdAsync(int Id)
        {
            ServiceResponse<GetCategorieDto> serviceResponse = new ServiceResponse<GetCategorieDto>();
            try
            {
                //Verifica que la categoria es valida
                var category = await this._serviceGenericCategoryHelper
                .WhereFirstEntityAsync(c => c.Id == Id)
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CategoryNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CategoryNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetCategorieDto
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
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
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
        public async Task<ServiceResponse<GetCategorieDto>> GetCategoryForNameAsync(string Name)
        {
            ServiceResponse<GetCategorieDto> serviceResponse = new ServiceResponse<GetCategorieDto>();
            try
            {
                //Verifica que la categoria es valida
                var category = await this._serviceGenericCategoryHelper
                .WhereSingleEntityAsync(c => c.Name == Name)
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CategoryNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CategoryNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetCategorieDto
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
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Actualiza una categoria.
        /// </summary>
        /// <param name="updateCategory"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdateCategoryAsync(UpdateCategoryDto updateCategory)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que la categoria es valida
                var category = await this._serviceGenericCategoryHelper
                .WhereSingleEntityAsync(c => c.Id == updateCategory.Id)
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CategoryNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CategoryNotFound);
                    return serviceResponse;
                }
                if (updateCategory.Name != null)
                category.Name = updateCategory.Name;
                this._serviceGenericCategoryHelper.UpdateEntity(category);
                await this._serviceGenericCategoryHelper.SaveChangesBDAsync().ConfigureAwait(false);
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
