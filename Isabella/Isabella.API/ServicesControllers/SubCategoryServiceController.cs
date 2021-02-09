namespace Isabella.API.ServicesControllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Common.RepositorysDtos;
    using Common;
    using Common.Dtos.SubCategory;
    using Common.Extras;
    using RepositorysModels;
    using Models;
    using System.Linq;
   
    /// <summary>
    /// Servicio para el controlador de las subcategorias.
    /// </summary>
    public class SubCategoryServiceController : ISubCategoryRepositoryDto
    {
        private readonly ISubCategoryRepositoryModel _subCategoryRepositoryModel;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="categoryRepositoryModel"></param>
        public SubCategoryServiceController(ISubCategoryRepositoryModel categoryRepositoryModel)
        {
            this._subCategoryRepositoryModel = categoryRepositoryModel;

        }

        /// <summary>
        /// Agrega una nueva subcategoria.
        /// </summary>
        /// <param name="addSubCategoryProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddSubCategoryAsync(AddSubCategoryDto addSubCategoryProduct)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (addSubCategoryProduct == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Verifica que la categoria es valida
                var category = await this._subCategoryRepositoryModel
                .GetSubCategoryForNameAsync(addSubCategoryProduct.Name)
                .ConfigureAwait(false);
                if (category != null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeCategory_Exist;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeCategory_Exist);
                    return serviceResponse;
                }
                var new_category = new SubCategory
                {
                    Name = addSubCategoryProduct.Name,
                    Price = addSubCategoryProduct.Price,
                };
                await this._subCategoryRepositoryModel
                .AddSubCategoryAsync(new_category)
                .ConfigureAwait(false);
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todas las subcategorias disponibles.
        /// </summary>
        public async Task<ServiceResponse<List<GetSubCategoryDto>>> GetAllSubCategoryAsync()
        {
            ServiceResponse<List<GetSubCategoryDto>> serviceResponse = new ServiceResponse<List<GetSubCategoryDto>>();
            try
            {
                //Verifica que la categoria es valida
                var category = await this._subCategoryRepositoryModel
                .GetAllSubCategoryAsync()
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
                serviceResponse.Data = category.Select(c => new GetSubCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Price = c.Price,
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
        /// Obtiene una subcategoria por su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetSubCategoryDto>> GetSubCategoryForIdAsync(int Id)
        {
            ServiceResponse<GetSubCategoryDto> serviceResponse = new ServiceResponse<GetSubCategoryDto>();
            try
            {
                //Verifica que la categoria es valida
                var category = await this._subCategoryRepositoryModel
                .GetSubCategoryProductForIdAsync(Id)
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
                serviceResponse.Data = new GetSubCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Price = category.Price
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
        /// Obtiene una subcategoria por su nombre.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetSubCategoryDto>> GetSubCategoryForNameAsync(string Name)
        {
            ServiceResponse<GetSubCategoryDto> serviceResponse = new ServiceResponse<GetSubCategoryDto>();
            try
            {
                //Verifica que la categoria es valida
                var category = await this._subCategoryRepositoryModel
                .GetSubCategoryForNameAsync(Name)
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
                serviceResponse.Data = new GetSubCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Price = category.Price
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
