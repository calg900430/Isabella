namespace Isabella.API.ServicesControllers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;

    using Common;
    using Common.Dtos.Users;
    using Common.RepositorysDtos;
    using Isabella.Common.Extras;
    using Extras;
    using RepositorysModels;
    using Models;
   
    /// <summary>
    /// Servicio para el controlador de los usuarios.
    /// </summary>
    public class UserServiceController : IUserRepositoryDto
    {
        private readonly IUserRepositoryModel _userService;

        /// <summary>
        /// Claims del usuario.
        /// </summary>
        public ClaimsPrincipal ClaimsPrincipal { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userRepositoryModelService"></param>
        public UserServiceController(IUserRepositoryModel userRepositoryModelService)
        {
            this._userService = userRepositoryModelService;
        }

        /// <summary>
        /// Devuelve una entidad del modelo User sin mapear dado su correo electrónico.
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public async Task<User> GetEntityUserForEmailAsync(string Email)
        {
            var user = await this._userService.GetUserByEmailAsync(Email).ConfigureAwait(false);
            if (user == null)
            return null;
            return user;   
        }

        /// <summary>
        /// Agrega un nuevo usuario a la base de datos.
        /// </summary>
        /// <param name="newuser"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddUserAsync(RegisterUserDto newuser)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que los datos enviados por el usuario sean correctos.
                if (newuser == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Verifica si el role(cliente en este caso) que se desea dar al usuario está disponible en la base de datos.
                var role_existing = await this._userService.VerifyRoleAsync(Constants.RolesOfSystem[0]).ConfigureAwait(false);
                if (role_existing == false)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeRole_BadRole;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeRole_BadRole);
                    return serviceResponse;
                }
                //Verifica que el email dado no este en uso.
                var email_existing = await this._userService.VerifyEmailAsync(newuser.Email).ConfigureAwait(false);
                if (email_existing)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_BadEmail;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_BadEmail);
                    return serviceResponse;
                }
                //Verifica que la cuenta de usuario dada no este disponible.
                var username_existing = await this._userService.VerifyUserNameAsync(newuser.UserName).ConfigureAwait(false);
                if (username_existing)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_BadUserName;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_BadUserName);
                    return serviceResponse;
                }
                //Crea el nuevo usuario con los parametros especificados por el usuario(Mapea de RegisterUserDto a User)
                var new_registeruser = new User
                {
                    Email = newuser.Email,
                    UserName = newuser.UserName,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    LastDateConnected = DateTime.UtcNow,
                    IdForClaim = Guid.NewGuid(),
                    RecoverPassword = false,
                    EmailConfirmed = false,
                };
                //Guarda el usuario con una contraseña definida.
                var user = await this._userService.AddUserAsync(new_registeruser, newuser.Password).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_DataBase;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_DataBase);
                    return serviceResponse;
                }
                //Asigna el role al usuario
                var role = await this._userService.AddRoleForUserAsync(user, Constants.RolesOfSystem[1]).ConfigureAwait(false);
                if (!role)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_DataBase;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_DataBase);
                    return serviceResponse;
                }
                //Envia un correo al usuario con el código de verificación.
                var send_mail = await this._userService.SendEmailWithCodeConfirmRegisterAsync(user).ConfigureAwait(false);
                if (!send_mail)
                {
                    serviceResponse.Code = CodeMessage.Code.EmailNotSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.EmailNotSend);
                    return serviceResponse;
                }
                //Mapea de User a GetUserDto
                serviceResponse.Data = true;
                serviceResponse.Code = CodeMessage.Code.EmailRegisterConfirmation;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.EmailRegisterConfirmation);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }


        /// <summary>
        /// Confirma el registro de un usuario en el sistema con el código enviado a su correo electrónico.
        /// </summary>
        /// <param name="confirmEmail"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> ConfirmEmailUserAsync(ConfirmEmailDto confirmEmail)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (confirmEmail == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado
                var user = await this._userService.GetUserByEmailAsync(confirmEmail.Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Verifica que el usuario no haya confirmado el registro anteriormente.
                var verify_confirm_register = await this._userService.VerifyConfirmRegisterUserAsync(user).ConfigureAwait(false);
                if (verify_confirm_register)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_YesConfirmRegister;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_YesConfirmRegister);
                    return serviceResponse;
                }
                //Confirma el registro
                var confirm_register = await this._userService.ConfirmRegisterUserAsync(user, confirmEmail.Token).ConfigureAwait(false);
                if (!confirm_register)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_CodeVerificationNotCorrect;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_CodeVerificationNotCorrect);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;               
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetUserDto>>> GetAllUserAsync()
        {
            ServiceResponse<List<GetUserDto>> serviceResponse = new ServiceResponse<List<GetUserDto>>();
            try
            {
                var list_users = await this._userService.GetAllUserAsync().ConfigureAwait(false);
                if (list_users == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_AllNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_AllNotFound);
                    return serviceResponse;
                }
                //Mapea de una lista de User a una lista de GetUserDto.
                serviceResponse.Data = list_users.Select(c => new GetUserDto
                {
                    Address = c.Address,
                    FirstName = c.FirstName,
                    Id = c.Id,
                    LastName = c.LastName,
                    PhoneNumber = c.PhoneNumber,
                    ImageUserProfile = c.ImageUserProfile,
                }).ToList();
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;    
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene el Id del último usuario que se registró en el sistema.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<int>> GetIdOfLastUserAsync()
        {
            ServiceResponse<int> serviceResponse = new ServiceResponse<int>();
            try
            {
                var Id = await this._userService.GetIdOfLastUserAsync().ConfigureAwait(false);
                if (Id == -1)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_AllNotFound;
                    serviceResponse.Data = -1;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_AllNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = Id;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;

            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = -1;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene un usuario dado su Id.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetUserDto>> GetUserByIdAsync(int UserId)
        {
            ServiceResponse<GetUserDto> serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                var user = await this._userService.GetUserByIdAsync(UserId).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                //Mapea de User a GetUserDto.
                serviceResponse.Data = new GetUserDto
                {
                    Id = user.Id,
                    Address = user.Address,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    ImageUserProfile = user.ImageUserProfile,
                };
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene un usuario dada su cuenta de usuario.
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetUserDto>> GetUserByUserNameAsync(string UserName)
        {
            ServiceResponse<GetUserDto> serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                var user = await this._userService.GetUserByUserNameAsync(UserName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                //Mapea de User a GetUserDto.
                serviceResponse.Data = new GetUserDto
                {
                   Id = user.Id,
                   Address = user.Address,
                   FirstName = user.FirstName,
                   LastName = user.LastName,
                   PhoneNumber = user.PhoneNumber,
                   ImageUserProfile = user.ImageUserProfile,
                };
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse; 
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene un usuario dado su email.
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetUserDto>> GetUserByEmailAsync(string Email)
        {
            ServiceResponse<GetUserDto> serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                var user = await this._userService.GetUserByEmailAsync(Email).ConfigureAwait(false);
                if (user == null)
                {
                   serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                   serviceResponse.Data = null;
                   serviceResponse.Success = false;
                   serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                   return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                //Mapea de User a GetUserDto.
                serviceResponse.Data = new GetUserDto
                {
                   Id = user.Id,
                   Address = user.Address,
                   FirstName = user.FirstName,
                   LastName = user.LastName,
                   PhoneNumber = user.PhoneNumber,
                   ImageUserProfile = user.ImageUserProfile,
                };
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Realiza el login del usuario y obtiene el token de acceso,el tiempo de expiración y otros datos del usuario.
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<ResponseLoginTokenDto>> LoginUserAsync(LoginUserWithUserNameDto loginUser)
        {
            ServiceResponse<ResponseLoginTokenDto> serviceResponse = new ServiceResponse<ResponseLoginTokenDto>();
            try
            {
                //Verifica que los datos enviados por el usuario sean correctos.
                if (loginUser == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                if (loginUser.Email == null || loginUser.Email == string.Empty)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotEmail;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotEmail);
                    return serviceResponse;
                }
                //Verifica que el usuario este registrado.
                var user = await this._userService.GetUserByEmailAsync(loginUser.Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Verifica que el usuario haya confirmado el registro anteriormente.
                var confirm_register = await this._userService.VerifyConfirmRegisterUserAsync(user).ConfigureAwait(false);
                if (!confirm_register)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotConfirmRegister;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotConfirmRegister);
                    return serviceResponse;
                }
                //Verifica que la contraseña dada por el usuario sea correcta.
                var password_correct = await this._userService.VerifyPasswordUserAsync(user, loginUser.Password).ConfigureAwait(false);
                if (!password_correct)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_VerifyPasswordAndUserAccount;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_VerifyPasswordAndUserAccount);
                    return serviceResponse;
                }
                //Genera el Token del usuario.
                var token_datetime_expires_roles = await this._userService.CreateTokenAsync(user).ConfigureAwait(false);
                if (token_datetime_expires_roles == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_ErrorGenerateToken;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_ErrorGenerateToken);
                    return serviceResponse;
                }
                //Genera los datos del token y otros datos del usuario.
                var getuser = new GetUserDto
                {
                    Id = user.Id,
                    Address = user.Address,
                    FirstName = user.FirstName,
                    ImageUserProfile = user.ImageUserProfile,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                };
                ResponseLoginTokenDto responseLoginToken = new ResponseLoginTokenDto
                {
                    Roles = token_datetime_expires_roles.UserRoles,
                    DateExpiration = token_datetime_expires_roles.DateTime,
                    Token = token_datetime_expires_roles.Token,
                    GetUser = getuser,
                    Email = user.Email,
                    UserName = user.UserName,
                };
                serviceResponse.Code = CodeMessage.Code.CodeUser_LoginTokenUser;
                serviceResponse.Data = responseLoginToken;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_LoginTokenUser);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene un número determinado de usuarios dado un Id y la cantidad deseada.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="CantUsers"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetUserDto>>> GetCantUserAsync(int UserId, int CantUsers)
        {
            ServiceResponse<List<GetUserDto>> serviceResponse = new ServiceResponse<List<GetUserDto>>();
            try
            {
                if (CantUsers < 1)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_ValueNotValide;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_ValueNotValide);
                    return serviceResponse;
                }
                //Obtiene todos los usuarios disponibles
                var list_users = await this._userService.GetAllUserAsync().ConfigureAwait(false);
                if (list_users == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_AllNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_AllNotFound);
                    return serviceResponse;
                }
                //Obtiene el usuario de referencia
                var user_reference = list_users.Where(c => c.Id == UserId).FirstOrDefault();
                if (user_reference == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Verifica si hay usuarios a partir del usuario de referencia
                var begin_found_user = list_users.FirstOrDefault(c => c.Id == user_reference.Id + 1);
                if (begin_found_user == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotNewUser;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotNewUser);
                    return serviceResponse;
                }
                //Obtiene el index del usuario por el cual se debe comenzar a buscar los demas.
                var index_user = list_users.LastIndexOf(begin_found_user);
                //Obtiene el Index del ultimo usuario.
                var last_index = list_users.LastIndexOf(list_users.LastOrDefault());
                //Obtiene la cantidad de usuarios disponibles a partir del usuario de referencia.
                var users_available = last_index - index_user + 1;
                //Envia la cantidad de usuarios disponibles.
                if (CantUsers >= users_available)
                {
                    //Toma una cantidad de elementos contiguos a partir de un index de referencia.
                    var list_user_to_send = list_users.GetRange(index_user, users_available);
                    var list_getusers = list_user_to_send.Select(c => new GetUserDto
                    {
                        ImageUserProfile = c.ImageUserProfile,
                        Address = c.Address,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        PhoneNumber = c.PhoneNumber,
                        Id = c.Id,
                    }).ToList();
                    serviceResponse.Data = list_getusers;
                    serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                    serviceResponse.Success = true;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                    return serviceResponse;
                }
                //Envia las publicaciones solicitadas por el usuario
                else
                {
                    //Toma una cantidad de elementos contiguos a partir de un index de referencia.
                    var list_user_to_send = list_users.GetRange(index_user, CantUsers);
                    var list_getusers = list_user_to_send.Select(c => new GetUserDto
                    {
                        ImageUserProfile = c.ImageUserProfile,
                        Address = c.Address,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        PhoneNumber = c.PhoneNumber,
                        Id = c.Id
                    }).ToList();
                    serviceResponse.Data = list_getusers;
                    serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                    serviceResponse.Success = true;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                    return serviceResponse;
                }
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Solicita la recuperación de la contraseña.
        /// </summary>
        /// <param name="resetPassword"></param>
        public async Task<ServiceResponse<bool>> ResetPasswordUserAsync(ResetPasswordDto resetPassword)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (resetPassword == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                var user = await this._userService.GetUserByEmailAsync(resetPassword.Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Envia un correo para la recuperación de la contraseña.
                var send_email = await this._userService.SendEmailForResetPasswordUserAsync(user).ConfigureAwait(false);
                if (!send_email)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.EmailNotSend;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.EmailNotSend);
                    return serviceResponse;
                }
                serviceResponse.Data = true;
                serviceResponse.Code = CodeMessage.Code.CodeRecoverPassword;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeRecoverPassword);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Envia un nuevo correo al usuario con el código para la confirmación del registro.
        /// </summary>
        /// <param name="sendToNewCodeConfirmationRegister"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> SendToNewCodeConfirmationEmailAsync(SendToNewCodeConfirmationRegisterDto sendToNewCodeConfirmationRegister)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {

                if (sendToNewCodeConfirmationRegister == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado
                var user = await this._userService.GetUserByEmailAsync(sendToNewCodeConfirmationRegister.Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Verifica que el usuario no haya confirmado el registro anteriormente.
                var verify_confirm_register = await this._userService.VerifyConfirmRegisterUserAsync(user).ConfigureAwait(false);
                if (verify_confirm_register)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_YesConfirmRegister;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_YesConfirmRegister);
                    return serviceResponse;
                }
                //Envia un nuevo correo con el código de confirmación del registro.
                var send_mail = await this._userService.SendEmailWithCodeConfirmRegisterAsync(user).ConfigureAwait(false);
                if (!send_mail)
                {
                    serviceResponse.Code = CodeMessage.Code.EmailNotSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.EmailNotSend);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.EmailRegisterConfirmation;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.EmailRegisterConfirmation);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Ejecuta la función que recupera la contraseña del usuario con el código enviado a su correo electronico.
        /// </summary>
        /// <param name="recoverPassword"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> RecoverPasswwordUserAsync(RecoverPasswordDto recoverPassword)
        => await ExecuteRecoverPasswwordUserAsync(recoverPassword,ClaimsPrincipal).ConfigureAwait(false);

        /// <summary>
        /// Ejecuta la función que actualiza al usuario.
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetUserDto>> UpdateUserAsync(UpdateUserDto updateUser)
        => await ExecuteUpdateUserAsync(updateUser, ClaimsPrincipal).ConfigureAwait(false);

        /// <summary>
        /// Ejecuta la función para borrar la imagen de perfil del usuario especificado.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> DeleteImageProfileUserAsync()
        => await ExecuteDeleteImageProfileUserAsync(ClaimsPrincipal).ConfigureAwait(false);

        /// <summary>
        /// Ejecuta la función para cambiar la contraseña de un usuario.
        /// </summary>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdatePasswordAsync(ChangePasswordUserDto changePassword)
        => await ExecuteUpdatePasswordAsync(changePassword, ClaimsPrincipal).ConfigureAwait(false);

        /// <summary>
        /// Borra la imagen de perfil del usuario.
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        private async Task<ServiceResponse<bool>> ExecuteDeleteImageProfileUserAsync(ClaimsPrincipal claimsPrincipal)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var userName = claimsPrincipal.Identity.Name;
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                var delete_image = await this._userService.DeleteImageProfileUserAsync(user).ConfigureAwait(false);
                if (!delete_image)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_DataBase;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_DataBase);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Recupera la contraseña del usuario con el código de recuperación enviado a su correo.
        /// </summary>
        /// <param name="recoverPassword"></param>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        private async Task<ServiceResponse<bool>> ExecuteRecoverPasswwordUserAsync(RecoverPasswordDto recoverPassword, ClaimsPrincipal claimsPrincipal)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (recoverPassword == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                var userName = claimsPrincipal.Identity.Name;
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                if (!this._userService.VerifyUserRecoverPasswordAsync(user))
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotRecoverPassword;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotRecoverPassword);
                    return serviceResponse;
                }
                //Recupera la contraseña
                var recover_password = await this._userService.RecoverPasswwordUserAsync(user, recoverPassword.Token, recoverPassword.NewPassword).ConfigureAwait(false);
                if (!recover_password)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_CodeVerificationNotCorrect;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_CodeVerificationNotCorrect);
                    return serviceResponse;
                }
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Actualiza el usuario.
        /// </summary>
        /// <param name="updateUser"></param>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        private async Task<ServiceResponse<GetUserDto>> ExecuteUpdateUserAsync(UpdateUserDto updateUser, ClaimsPrincipal claimsPrincipal)
        {
            ServiceResponse<GetUserDto> serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                //Verifica que el usuario haya enviado los datos correctamentes.
                if (updateUser == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Verifica que la imagen sea valida
                if (updateUser.ImageProfile != null)
                {
                    if (updateUser.ImageProfile.Length <= 0)
                    {
                        serviceResponse.Code = CodeMessage.Code.CodeImage_ImageErrorValue;
                        serviceResponse.Data = null;
                        serviceResponse.Success = false;
                        serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ImageErrorValue);
                        return serviceResponse;
                    }
                    if (updateUser.ImageProfile.Length > Constants.MAX_LENTHG_IMAGE_PROFILE_USER)
                    {
                        serviceResponse.Code = CodeMessage.Code.CodeImage_ImageUserNotValide;
                        serviceResponse.Data = null;
                        serviceResponse.Success = false;
                        serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ImageUserNotValide);
                        return serviceResponse;
                    }
                }
                //Obtiene la cuenta del usuario.
                var userName = claimsPrincipal.Identity.Name;
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Actualiza los campos del usuario
                if (updateUser.FirstName != null)
                user.FirstName = updateUser.FirstName;
                if (updateUser.LastName != null)
                user.LastName = updateUser.LastName;
                if (updateUser.PhoneNumber != null)
                user.PhoneNumber = updateUser.PhoneNumber.ToString();
                if (updateUser.Address != null)
                user.Address = updateUser.Address;
                if (updateUser.ImageProfile != null)
                user.ImageUserProfile = updateUser.ImageProfile;
                user.DateUpdated = DateTime.UtcNow;
                //Manda a actualizar el usuario.
                var user_update = await this._userService.UpdateUserAsync(user).ConfigureAwait(false);
                if (user_update == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_DataBase;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_DataBase);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = new GetUserDto
                {
                    Address = user_update.Address,
                    FirstName = user_update.FirstName,
                    Id = user_update.Id,
                    ImageUserProfile = user_update.ImageUserProfile,
                    LastName = user_update.LastName,
                    PhoneNumber = user_update.PhoneNumber
                };
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Cambia la contraseña del usuario.
        /// </summary>
        /// <param name="changePassword"></param>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        private async Task<ServiceResponse<bool>> ExecuteUpdatePasswordAsync(ChangePasswordUserDto changePassword, ClaimsPrincipal claimsPrincipal)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que el usuario no haya enviado un dato nulo.
                if (changePassword == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Obtiene la cuenta de usuario.
                var userName = claimsPrincipal.Identity.Name;
                //Obtiene al usuario.
                var user = await this._userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Verifica que la contraseña antigua sea valida.
                var verify_password = await this._userService.VerifyPasswordUserAsync(user, changePassword.PasswordOld).ConfigureAwait(false);
                if (!verify_password)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_PasswordNotCorrect;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_PasswordNotCorrect);
                    return serviceResponse;
                }
                //Cambia la contraseña 
                var change_password_correct = await this._userService.UpdatePasswordAsync(user, changePassword.PasswordOld, changePassword.PasswordNew).ConfigureAwait(false);
                if (!change_password_correct)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_DataBase;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_DataBase);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Agrega o actualiza una imagen de usuario usando IFormFile
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddOrUpdateImageProfileUser(IFormFile formFile, ClaimsPrincipal claimsPrincipal)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que la imagen, cumpla con los reguerimientos para poderla almacenar en la base de datos.
                if (formFile == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                if (formFile.Length <= 0)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                if (formFile.Length > Constants.MAX_LENTHG_IMAGE_PROFILE_USER)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ImageUserNotValide;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ImageUserNotValide);
                    return serviceResponse;
                }
                //Obtiene la cuenta de usuario.
                var userName = claimsPrincipal.Identity.Name;
                //Obtiene al usuario.
                var user = await this._userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                var add_image = await this._userService.AddOrUpdateImageProfileUserAsync(user, formFile).ConfigureAwait(false);
                if (!add_image)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ImageErrorValue;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ImageErrorValue);
                    return serviceResponse;
                }
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Login de usuarios con proveedores externos(Google, Facebook, Twitter y Apple). 
        /// Si el usuario no existe lo registra en la aplicación.
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public async Task LoginExternProviderForAsync(string scheme)
        {
            //Se implementa en el controlador.
        }

    }
}
