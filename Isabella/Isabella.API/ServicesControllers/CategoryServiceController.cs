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
    using RepositorysModels;
    using Models;
    
    /// <summary>
    /// Servicio para el controlador de las categorias de los productos.
    /// </summary>
    public class CategoryServiceController : ICategoryRepositoryDto
    {
        private readonly ICategoryRepositoryModel _categoryRepositoryModel;

        /// <summary>
        /// Categorias
        /// </summary>
        /// <param name="categoryRepositoryModel"></param>
        public CategoryServiceController(ICategoryRepositoryModel categoryRepositoryModel)
        {
            this._categoryRepositoryModel = categoryRepositoryModel;
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
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Verifica que la categoria es valida
                var category = await this._categoryRepositoryModel
                .GetCategoryForNameAsync(addCategory.Name)
                .ConfigureAwait(false);
                if (category != null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeCategory_Exist;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeCategory_Exist);
                    return serviceResponse;
                }
                var new_category = new Category
                {
                   Name = addCategory.Name,
                };
                await this._categoryRepositoryModel
                .AddCategoryAsync(new_category)
                .ConfigureAwait(false);
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
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
                //Verifica que la categoria es valida
                var category = await this._categoryRepositoryModel
                .GetAllCategoryAsync()
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeCategory_NotAllFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeCategory_NotAllFound);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = category.Select(c => new GetCategoryDto 
                { 
                   Id = c.Id,
                   Name = c.Name
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
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
                var category = await this._categoryRepositoryModel
                .GetCategoryForIdAsync(Id)
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeCategory_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeCategory_NotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = new GetCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name
                };
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
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
                var category = await this._categoryRepositoryModel
                .GetCategoryForNameAsync(Name)
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeCategory_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeCategory_NotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = new GetCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name
                };
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }
    }
}
