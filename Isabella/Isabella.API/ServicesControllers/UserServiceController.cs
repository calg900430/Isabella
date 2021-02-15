namespace Isabella.API.ServicesControllers
{
    using System;
    using System.Linq;
    using System.IO;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Common;
    using Common.Dtos.Users;
    using Common.RepositorysDtos;
    using Isabella.Common.Extras;
    using Models.Entities;
    using Helpers.RepositoryHelpers;
    using Helpers;
    
    /// <summary>
    /// Servicio para el controlador de los usuarios.
    /// </summary>
    public class UserServiceController : IUserRepositoryDto
    {
        private readonly IUserRepositoryHelper _userService;
        private readonly MailHelper _mailHelper;
        private readonly ServiceGenericHelper<User> _serviceGenericUserHelper;

        /// <summary>
        /// Claims del usuario.
        /// </summary>
        public ClaimsPrincipal ClaimsPrincipal { get; set; }

        /// <summary>
        /// Url del controlador.
        /// </summary>
        public IUrlHelper Url { get; set; }

        /// <summary>
        /// Solicitud Http
        /// </summary>
        public HttpRequest HttpRequest { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="mailHelper"></param>
        /// <param name="serviceGenericUserHelper"></param>
        public UserServiceController(IUserRepositoryHelper userRepository, MailHelper mailHelper,
        ServiceGenericHelper<User> serviceGenericUserHelper)
        {
            this._userService = userRepository;
            this._mailHelper = mailHelper;
            this._serviceGenericUserHelper = serviceGenericUserHelper;
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
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica si el role(admin en este caso) que se desea dar al usuario está disponible en la base de datos.
                var role_existing = await this._userService.VerifyRoleAsync(Constants.RolesOfSystem[0]).ConfigureAwait(false);
                if (role_existing == false)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.BadRole;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.BadRole);
                    return serviceResponse;
                }
                //Verifica que el email dado no este en uso.
                var email_existing = await this._userService.VerifyEmailAsync(newuser.Email).ConfigureAwait(false);
                if (email_existing)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.BadEmail;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.BadEmail);
                    return serviceResponse;
                }
                //Verifica que la cuenta de usuario dada no este disponible, que es lo mismo que el email.
                var username_existing = await this._userService.VerifyUserNameAsync(newuser.Email).ConfigureAwait(false);
                if (username_existing)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.BadEmail;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.BadEmail);
                    return serviceResponse;
                }
                //Crea el nuevo usuario con los parametros especificados por el usuario(Mapea de RegisterUserDto a User)
                var new_registeruser = new User
                {
                    Email = newuser.Email,
                    UserName = newuser.Email,
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
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity);
                    return serviceResponse;
                }
                //Asigna el role al usuario
                var role = await this._userService.AddRoleForUserAsync(user, Constants.RolesOfSystem[0]).ConfigureAwait(false);
                if (!role)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity);
                    return serviceResponse;
                }
                //Obtiene el Token de confirmación de registro del usuario.
                var token = await this._userService.GenerateTokenForConfirmRegisterAsync(user).ConfigureAwait(false);
                //Crea un Link que contiene el Token de confirmación.
                var tokenLink = this.Url.Action("ConfirmRegisterUserAsync", "Users", new
                {  
                   userid = user.Id,
                   token = token,
                }, protocol: HttpRequest.Scheme); 
                var body_message = $"<h1>Email Confirmation</h1>" +
                 "Hello you have registered in the Isabella application," +
                 $"to allow the user, plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>";
                //Envia un correo al usuario con el código de verificación.
                var send_mail = this._mailHelper.SendMail(user.Email, "Email confirmation", body_message);
                if (!send_mail)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EmailNotSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EmailNotSend);
                    return serviceResponse;
                }
                //Mapea de User a GetUserDto
                serviceResponse.Data = true;
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EmailRegisterConfirmation;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EmailRegisterConfirmation);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Confirma el registro de un usuario en el sistema con el código enviado a su correo electrónico.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> ConfirmEmailUserAsync(string Id, string Token)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica si el usuario está registrado
                var user = await this._userService.GetUserByIdAsync(int.Parse(Id)).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Verifica que el usuario no haya confirmado el registro anteriormente.
                var verify_confirm_register = await this._userService.VerifyConfirmRegisterUserAsync(user).ConfigureAwait(false);
                if (verify_confirm_register)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserConfirmRegister;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserConfirmRegister);
                    return serviceResponse;
                }
                //Confirma el registro
                var confirm_register = await this._userService.ConfirmRegisterUserAsync(user, Token).ConfigureAwait(false);
                if (confirm_register == false)
                {
                    serviceResponse.Data = false;
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.TokeConfirmRegisterBad;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.TokeConfirmRegisterBad);
                    return serviceResponse;
                }
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;               
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
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
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserAllNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserAllNotFound);
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
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;    
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
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
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserAllNotFound;
                    serviceResponse.Data = -1;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserAllNotFound);
                    return serviceResponse;
                }
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = Id;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;

            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = -1;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
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
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
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
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
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
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
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
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse; 
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
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
                   serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserNotFound;
                   serviceResponse.Data = null;
                   serviceResponse.Success = false;
                   serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                   return serviceResponse;
                }
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
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
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Realiza el login del usuario y obtiene el token de acceso,el tiempo de expiración y otros datos del usuario.
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetDataUserForLoginDto>> LoginUserAsync(LoginUserWithUserNameDto loginUser)
        {
            ServiceResponse<GetDataUserForLoginDto> serviceResponse = new ServiceResponse<GetDataUserForLoginDto>();
            try
            {
                //Verifica que los datos enviados por el usuario sean correctos.
                if (loginUser == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (loginUser.Email == null || loginUser.Email == string.Empty)
                {
                    serviceResponse.Data = null;
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.RequiredEmailOfUser;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.RequiredEmailOfUser);
                    return serviceResponse;
                }
                //Verifica que el usuario este registrado.
                var user = await this._userService.GetUserByEmailAsync(loginUser.Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Verifica que el usuario haya confirmado el registro anteriormente.
                var confirm_register = await this._userService.VerifyConfirmRegisterUserAsync(user).ConfigureAwait(false);
                if (!confirm_register)
                {
                    serviceResponse.Data = null;
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.NotConfirmRegister;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.NotConfirmRegister);
                    return serviceResponse;
                }
                //Verifica que la contraseña dada por el usuario sea correcta.
                var password_correct = await this._userService.VerifyPasswordUserAsync(user, loginUser.Password).ConfigureAwait(false);
                if (!password_correct)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.VerifyPasswordAndEmail;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.VerifyPasswordAndEmail);
                    return serviceResponse;
                }
                //Genera el Token del usuario.
                var token_datetime_expires = await this._userService.CreateTokenAsync(user).ConfigureAwait(false);
                if (token_datetime_expires.Item1 == null || token_datetime_expires.Item2 == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.ErrorGenerateToken;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGenerateToken);
                    return serviceResponse;
                }
                //Genera los datos del token y otros datos del usuario.
                var getdatauserlogin = new GetDataUserForLoginDto
                {
                    Id = user.Id,
                    Address = user.Address,
                    FirstName = user.FirstName,
                    ImageUserProfile = user.ImageUserProfile,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    DateExpirationToken = token_datetime_expires.Item1,
                    Token = token_datetime_expires.Item2,
                    Email = user.Email,
                };
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.LoginUserSuccess;
                serviceResponse.Data = getdatauserlogin;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.LoginUserSuccess);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
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
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Obtiene el usuario.
                var user = await this._serviceGenericUserHelper
                .GetLoadAsync(c => c.Id == UserId)
                .ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                var all_cant_user = await this._serviceGenericUserHelper.GetLoadAsync(user, CantUsers)
                .ConfigureAwait(false);
                if (all_cant_user == null || all_cant_user.Count <= 0)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserNotNew;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotNew);
                    return serviceResponse;
                }
                serviceResponse.Data = all_cant_user.Select(c => new GetUserDto
                {
                    Id = c.Id,
                    Address = c.Address,
                    FirstName = c.FirstName,
                    ImageUserProfile = c.ImageUserProfile,
                    LastName = c.LastName,
                    PhoneNumber = c.PhoneNumber,
                }).ToList();
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
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
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                var user = await this._userService.GetUserByEmailAsync(resetPassword.Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Genera el Token para la confirmación de la contraseña.
                var token = await this._userService.GenerateTokenForRecoverPasswordAsync(user).ConfigureAwait(false);
                //Crea un Link que contiene el Token de confirmación y la nueva contraseña.
                var tokenLink = this.Url.Action("ConfirmRecoverPasswordUserAsync", "Users", new
                {
                    userid = user.Id,
                    token = token,
                    newpassword = resetPassword.NewPassword,
                }, protocol: HttpRequest.Scheme);
                var body_message = $"<h1>Email Confirmation</h1>" +
                 "Hello you have requested a code for the recovery of your password in the Isabella application," +
                 $"to recover you password, plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>";
                //Envia un correo al usuario con el código de verificación.
                var send_mail = this._mailHelper.SendMail(user.Email, "Email confirmation", body_message);
                if (!send_mail)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EmailNotSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EmailNotSend);
                    return serviceResponse;
                }
                serviceResponse.Data = true;
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.CodeRecoverPassword;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.CodeRecoverPassword);
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
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado
                var user = await this._userService.GetUserByEmailAsync(sendToNewCodeConfirmationRegister.Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Verifica que el usuario no haya confirmado el registro anteriormente.
                var verify_confirm_register = await this._userService.VerifyConfirmRegisterUserAsync(user).ConfigureAwait(false);
                if (verify_confirm_register)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserConfirmRegister;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserConfirmRegister);
                    return serviceResponse;
                }
                //Obtiene el Token de confirmación de registro del usuario.
                var token = await this._userService.GenerateTokenForConfirmRegisterAsync(user).ConfigureAwait(false);
                //Crea un Link que contiene el Token de confirmación.
                var tokenLink = this.Url.Action("ConfirmRegisterUserAsync", "Users", new
                {
                    userid = user.Id,
                    token = token,
                }, protocol: HttpRequest.Scheme);
                var body_message = $"<h1>Email Confirmation</h1>" +
                 "Hello you have registered in the Isabella application," +
                 $"to allow the user, plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>";
                //Envia un correo al usuario con el código de verificación.
                var send_mail = this._mailHelper.SendMail(user.Email, "Email confirmation", body_message);
                if (!send_mail)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EmailNotSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EmailNotSend);
                    return serviceResponse;
                }
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EmailRegisterConfirmation;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EmailRegisterConfirmation);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Ejecuta la función que recupera la contraseña del usuario con el código enviado a su correo electronico.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> RecoverPasswwordUserAsync(string Id, string Token, string newPassword)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userService.GetUserByIdAsync(int.Parse(Id)).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Recupera la contraseña
                var recover_password = await this._userService
                .RecoverPasswwordUserAsync(user, Token, newPassword)
                .ConfigureAwait(false);
                if (!recover_password)
                {
                    serviceResponse.Data = false;
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.TokeConfirmRegisterBad;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.TokeConfirmRegisterBad);
                    return serviceResponse;
                }
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }
       
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
                if(userName == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                var delete_image = await this._userService.DeleteImageProfileUserAsync(user).ConfigureAwait(false);
                if (!delete_image)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity);
                    return serviceResponse;
                }
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
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
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica que la imagen sea valida
                if (updateUser.ImageProfile != null)
                {
                    if (updateUser.ImageProfile.Length <= 0)
                    {
                        serviceResponse.KeyResource = GetValueResourceFile.KeyResource.ImageErrorValue;
                        serviceResponse.Data = null;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImageErrorValue);
                        return serviceResponse;
                    }
                    if (updateUser.ImageProfile.Length > Constants.MAX_LENTHG_IMAGE_PROFILE_USER)
                    {
                        serviceResponse.KeyResource = GetValueResourceFile.KeyResource.ImageUserNotValide;
                        serviceResponse.Data = null;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImageUserNotValide);
                        return serviceResponse;
                    }
                }
                var userName = claimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
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
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity);
                    return serviceResponse;
                }
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
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
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
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
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Obtiene la cuenta de usuario.
                var userName = claimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Obtiene al usuario.
                var user = await this._userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Verifica que la contraseña antigua sea valida.
                var verify_password = await this._userService.VerifyPasswordUserAsync(user, changePassword.PasswordOld).ConfigureAwait(false);
                if (!verify_password)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.PasswordNotCorrect;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.PasswordNotCorrect);
                    return serviceResponse;
                }
                //Cambia la contraseña 
                var change_password_correct = await this._userService.UpdatePasswordAsync(user, changePassword.PasswordOld, changePassword.PasswordNew).ConfigureAwait(false);
                if (!change_password_correct)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity);
                    return serviceResponse;
                }
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
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
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (formFile.Length <= 0)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (formFile.Length > Constants.MAX_LENTHG_IMAGE_PROFILE_USER)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.ImageUserNotValide;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImageUserNotValide);
                    return serviceResponse;
                }
                //Obtiene la cuenta de usuario.
                var userName = claimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Obtiene al usuario.
                var user = await this._userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Nombre de la imagen
                var file = $"{Guid.NewGuid()}.jpg";
                //Ruta temporal donde la guardaremos antes de enviarla a la base de datos.
                var path = Path.Combine(Directory.GetCurrentDirectory(), file);
                //Crea el archivo de la imagen que se encuentra en memoria RAM y lo guarda en la ruta seleccionada.
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream).ConfigureAwait(false);
                };
                var arraybyte_image = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
                user.ImageUserProfile = arraybyte_image;
                //Guarda la imagen del producto.
                this._serviceGenericUserHelper
                .UpdateEntity(user);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericUserHelper
                .SaveChangesBDAsync()
                .ConfigureAwait(false);
                //Borra el archivo de imagen temporal
                File.Delete(path);
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
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
            const string callbackScheme = "xamarinessentials";
            try
            {
                if (scheme != "Google")
                return;
                if (HttpRequest == null)
                return;
                //Verifica si los datos enviados por el proveedor externo son correctos.
                var auth = await HttpRequest.HttpContext.AuthenticateAsync(scheme);
                if (!auth.Succeeded || auth?.Principal == null
                || !auth.Principal.Identities.Any(id => id.IsAuthenticated)
                || string.IsNullOrEmpty(auth.Properties.GetTokenValue("access_token")))
                {
                    //Not authenticated, challenge
                    await HttpRequest.HttpContext.ChallengeAsync(scheme);
                    return;
                }
                else
                {
                    var claims = auth.Principal.Identities.FirstOrDefault()?.Claims;
                    var email = string.Empty;
                    //var mobilePhone = string.Empty;
                    email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
                    //mobilePhone = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.MobilePhone)?.Value;
                    //TODO: Resolver el login externo en caso de que el usuario tenga telefono en vez de email(Facebook o Twitter)
                    //Verifica que el usuario este registrado, si no está registrado lo registra.
                    var user = await this._userService.GetUserByEmailAsync(email).ConfigureAwait(false);
                    //Registra el nuevo usuario
                    if (user == null)
                    {
                        user = new User()
                        {
                            Email = email,
                            UserName = email,
                            DateCreated = DateTime.UtcNow,
                            DateUpdated = DateTime.UtcNow,
                            LastDateConnected = DateTime.UtcNow,
                            IdForClaim = Guid.NewGuid(),
                        };
                        var register_user = await this._userService
                        .AddUserAsync(user).ConfigureAwait(false);
                        if (register_user == null)
                        {
                            await HttpRequest.HttpContext.ChallengeAsync(scheme);
                            return;
                        }
                        //Asigna el role al usuario
                        var role_user = await this._userService.AddRoleForUserAsync(user, Constants.RolesOfSystem[0]).ConfigureAwait(false);
                        if (!role_user)
                        {
                            await HttpRequest.HttpContext.ChallengeAsync(scheme);
                            return;
                        }
                    }
                    //Genera un Token Bearer para el usuario
                    var token = await this._userService.CreateTokenAsync(user).ConfigureAwait(false);
                    if (token.Item1 == null || token.Item2 == null)
                    {
                        await HttpRequest.HttpContext.ChallengeAsync(scheme);
                        return;
                    }
                    //Verifica si el usuario tiene foto de perfil
                    var image = string.Empty;
                    if (user.ImageUserProfile != null)
                    image = Convert.ToBase64String(user.ImageUserProfile);
                    //Verifica los roles del usuario
                    var role = string.Empty;
                    //Crea los parametros que devolveremos a la url back
                    var qs = new Dictionary<string, string>
                    {
                      { "access_token", auth.Properties.GetTokenValue("access_token") },
                      { "refresh_token", auth.Properties.GetTokenValue("refresh_token") ?? string.Empty },
                      { "expires", (auth.Properties.ExpiresUtc?.ToUnixTimeSeconds() ?? -1).ToString() },
                      { "token_bearer", token.Item2},
                      { "expires_token_bearer", token.Item1.ToString()},
                      { "email", user.Email},
                      { "roles", role},
                      { "imageUserProfile", image ?? string.Empty},
                      { "userName", user.UserName ?? string.Empty},
                      { "firstName", user.FirstName ?? string.Empty },
                      { "lastName", user.LastName ?? string.Empty},
                      { "fullName", user.FullName ?? string.Empty},
                      { "id", user.Id.ToString() ?? string.Empty},
                      { "address", user.Address ?? string.Empty},
                      { "phoneNumber", user.PhoneNumber ?? string.Empty},
                    };
                    // Build the result url
                    var url = callbackScheme + "://#" + string.Join("&",
                    qs.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                    // Redirect to final url
                    HttpRequest.HttpContext.Response.Redirect(url);
                    return;
                }
            }
            catch (Exception)
            {
                await HttpRequest.HttpContext.ChallengeAsync(scheme);
                return;
            }
        }

    }
}
