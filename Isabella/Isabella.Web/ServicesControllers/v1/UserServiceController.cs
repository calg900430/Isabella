namespace Isabella.Web.ServicesControllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Models.Entities;
    using Common;
    using Common.Dtos.Users;
    using Common.RepositorysDtos;
    using Helpers;
    using Helpers.RepositoryHelpers;
    using Resources;
    using Isabella.Web.ViewModels.UsersViewModel;

    /// <summary>
    /// Servicio para el controlador de los usuarios.
    /// </summary>
    public class UserServiceController : IUserRepositoryDto
    {
        private readonly IUserRepositoryHelper _userService;
        private readonly MailHelper _mailHelper;
        private readonly ServiceGenericHelper<User> _serviceGenericUserHelper;
        private readonly ServiceGenericHelper<UserAdminsNotifications> _serviceGenericUserAdminsNotificationsHelper;

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
        /// <param name="serviceGenericUserAdminsNotificationsHelper"></param>
        public UserServiceController(IUserRepositoryHelper userRepository, MailHelper mailHelper,
        ServiceGenericHelper<User> serviceGenericUserHelper,
        ServiceGenericHelper<UserAdminsNotifications> serviceGenericUserAdminsNotificationsHelper)
        {
            this._userService = userRepository;
            this._mailHelper = mailHelper;
            this._serviceGenericUserHelper = serviceGenericUserHelper;
            this._serviceGenericUserAdminsNotificationsHelper = serviceGenericUserAdminsNotificationsHelper;
        }




        /// <summary>
        /// Registro rapido de usuario(Solicita el código de identificación para registrarse e iniciar sesión en la aplicación)
        /// Le crea un correo y un password al usuario, que se le devuelve para que el mismo haga el login.
        /// En este modo no tiene que confirmar el registro a través del correo.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<GetRegisterUserModeFastDto>> RegisterUserModeFastAsync()
        {
            ServiceResponse<GetRegisterUserModeFastDto> serviceResponse = new ServiceResponse<GetRegisterUserModeFastDto>();
            try
            {
                var code = Guid.NewGuid();
                //Crea el usuario y le asigna como password el codigo de verificacion
                var Email_UserName = $"{code.ToString()}@isabella.com";
                var registeruser = new User
                {
                    IdForClaim = code,
                    UserName = Email_UserName,
                    Email = Email_UserName,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    LastDateConnected = DateTime.UtcNow,
                };
                //Guarda el usuario con un password.
                var user = await this._userService.AddUserAsync(registeruser, registeruser.IdForClaim.ToString()).ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetRegisterUserModeFastDto 
                { 
                   Email = user.Email,
                   PasswordGuid = user.IdForClaim.ToString(),
                };
                //Obtiene el token para la confirmación del registro
                var token = await this._userService.GenerateTokenForConfirmRegisterAsync(user).ConfigureAwait(false);
                await this._userService.ConfirmRegisterUserAsync(user, token).ConfigureAwait(false);
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Agrega un usuario al sistema.
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica que el email dado no este en uso.
                var email_existing = await this._userService.VerifyEmailAsync(newuser.Email).ConfigureAwait(false);
                if (email_existing)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.BadEmail;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.BadEmail);
                    return serviceResponse;
                }
                //Verifica que la cuenta de usuario dada no este disponible, que es lo mismo que el email.
                var username_existing = await this._userService.VerifyUserNameAsync(newuser.Email).ConfigureAwait(false);
                if (username_existing)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.BadEmail;
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
                    EmailConfirmed = false,
                };
                //Guarda el usuario con una contraseña definida.
                var user = await this._userService.AddUserAsync(new_registeruser, newuser.Password).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity);
                    return serviceResponse;
                }
                //Obtiene el Token de confirmación de registro del usuario.
                var token = await this._userService.GenerateTokenForConfirmRegisterAsync(user).ConfigureAwait(false);
                //Crea un Link que contiene el Token de confirmación.
                var tokenLink = this.Url.Action("ConfirmRegister", "User", new
                {
                    userid = user.Id,
                    token = token,
                }, protocol: HttpRequest.Scheme);
                var body_message = $"<h1>Correo de Confirmación</h1>" +
                 "Hola se le ha creado una cuenta de usuario en el Restaurante Isabella," +
                 $"para completar el registro de esta cuenta haga click sobre el enlace:</br></br><a href = \"{tokenLink}\">Confirmar Registro</a>";
                //Envia un correo al usuario con el código de verificación.
                var send_mail = this._mailHelper.SendMail(user.Email, "Correo de Confirmación", body_message);
                if (!send_mail)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EmailNotSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EmailNotSend);
                    return serviceResponse;
                }
                serviceResponse.Data = true;
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EmailRegisterConfirmation;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EmailRegisterConfirmation);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Agrega un usuario admin al sistema.
        /// </summary>
        /// <param name="newuser"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddUserAdminAsync(AddUserViewModel newuser)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que los datos enviados por el usuario sean correctos.
                if (newuser == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica que el email dado no este en uso.
                var email_existing = await this._userService.VerifyEmailAsync(newuser.Email).ConfigureAwait(false);
                if (email_existing)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.BadEmail;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.BadEmail);
                    return serviceResponse;
                }
                //Verifica que la cuenta de usuario dada no este disponible, que es lo mismo que el email.
                var username_existing = await this._userService.VerifyUserNameAsync(newuser.Email).ConfigureAwait(false);
                if (username_existing)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.BadEmail;
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
                    EmailConfirmed = false,
                };
                //Guarda el usuario con una contraseña definida.
                var user = await this._userService.AddUserAsync(new_registeruser, newuser.Password).ConfigureAwait(false);
                if(user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity);
                    return serviceResponse;
                }
                //Asigna el role admin al nuevo usuario
                var assign_role = await this._userService
                .AddRoleForUserAsync(user, Constants.RolesOfSystem[0])
                .ConfigureAwait(false);
                if(!assign_role)
                {
                    //Borra el usuario acabado de crear al no poder asignarle el role admin
                    await this._userService.DeleteUserAsync(user).ConfigureAwait(false);
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity);
                    return serviceResponse;
                }
                //Obtiene el Token de confirmación de registro del usuario.
                var token = await this._userService.GenerateTokenForConfirmRegisterAsync(user).ConfigureAwait(false);
                //Crea un Link que contiene el Token de confirmación.
                var tokenLink = this.Url.Action("ConfirmRegister", "User", new
                {
                    userid = user.Id,
                    token = token,
                }, protocol: HttpRequest.Scheme);
                var body_message = $"<h1>Correo de Confirmación</h1>" +
                 "Hola se le ha creado una cuenta de usuario administrador para la gestión del Restaurante Isabella," +
                 $"para completar el registro de esta cuenta haga click sobre el enlace:</br></br><a href = \"{tokenLink}\">Confirmar Registro</a>";
                //Envia un correo al usuario con el código de verificación.
                var send_mail = this._mailHelper.SendMail(user.Email, "Correo de Confirmación", body_message);
                if(send_mail)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EmailNotSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EmailNotSend);
                    return serviceResponse;
                }
                serviceResponse.Data = true;
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EmailRegisterConfirmation;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EmailRegisterConfirmation);
                return serviceResponse;
            }
            catch(Exception ex)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Verifica que el usuario no haya confirmado el registro anteriormente.
                var verify_confirm_register = await this._userService.VerifyConfirmRegisterUserAsync(user).ConfigureAwait(false);
                if (verify_confirm_register)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserConfirmRegister;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.TokeConfirmRegisterBad;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.TokeConfirmRegisterBad);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;               
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserAllNotFound;
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
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;    
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserAllNotFound;
                    serviceResponse.Data = -1;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserAllNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = Id;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;

            }
            catch
            {
                serviceResponse.Code = (int) GetValueResourceFile.KeyResource.Exception;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
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
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
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
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
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
                   serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                   serviceResponse.Data = null;
                   serviceResponse.Success = false;
                   serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                   return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
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
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Realiza el login Api del usuario y obtiene el token de acceso,el tiempo de expiración y otros datos del usuario.
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetDataUserForLoginDto>> LoginUserAsync(LoginUserDto loginUser)
        {
            ServiceResponse<GetDataUserForLoginDto> serviceResponse = new ServiceResponse<GetDataUserForLoginDto>();
            try
            {
                //Verifica que los datos enviados por el usuario sean correctos.
                if (loginUser == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (loginUser.Email == null || loginUser.Email == string.Empty)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.RequiredEmailOfUser;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.RequiredEmailOfUser);
                    return serviceResponse;
                }
                //Verifica que el usuario este registrado.
                var user = await this._userService.GetUserByEmailAsync(loginUser.Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.VerifyPasswordAndEmail;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.VerifyPasswordAndEmail);
                    return serviceResponse;
                }
                //Verifica que el usuario haya confirmado el registro anteriormente.
                var confirm_register = await this._userService.VerifyConfirmRegisterUserAsync(user).ConfigureAwait(false);
                if (!confirm_register)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.NotConfirmRegister;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.NotConfirmRegister);
                    return serviceResponse;
                }
                //Verifica que la contraseña dada por el usuario sea correcta.
                var password_correct = await this._userService.VerifyPasswordUserAsync(user, loginUser.Password).ConfigureAwait(false);
                if (!password_correct)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.VerifyPasswordAndEmail;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.VerifyPasswordAndEmail);
                    return serviceResponse;
                }
                //Genera el Token del usuario.
                var token_datetime_expires = await this._userService.CreateTokenAsync(user).ConfigureAwait(false);
                if (token_datetime_expires.Item1 == null || token_datetime_expires.Item2 == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGenerateToken;
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
                    RolesOfUsers = await this._userService.GetAllRoleOfUserAsync(user).ConfigureAwait(false),
                };
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.LoginUserSuccess;
                serviceResponse.Data = getdatauserlogin;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.LoginUserSuccess);
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
        /// Realiza el login Web solo de usuarios admins.
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> LoginUserWebOnlyAdminAsync(LoginViewModel loginViewModel)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que los datos enviados por el usuario sean correctos.
                if (loginViewModel == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (string.IsNullOrEmpty(loginViewModel.Email))
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.RequiredEmailOfUser;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.RequiredEmailOfUser);
                    return serviceResponse;
                }
                //Verifica que el usuario este registrado.
                var user = await this._userService.GetUserByEmailAsync(loginViewModel.Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.VerifyPasswordAndEmail;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.VerifyPasswordAndEmail);
                    return serviceResponse;
                }
                //Verifica que el usuario haya confirmado el registro anteriormente.
                var confirm_register = await this._userService.VerifyConfirmRegisterUserAsync(user).ConfigureAwait(false);
                if (!confirm_register)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.NotConfirmRegister;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.NotConfirmRegister);
                    return serviceResponse;
                }
                //Verifica si el usuario es admin
                var is_have_role = await this._userService
                .VerifyRoleInUserAsync(user, Constants.RolesOfSystem[0])
                .ConfigureAwait(false);
                if(!is_have_role)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotAuthorizeLogin;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotAuthorizeLogin);
                    return serviceResponse;
                }
                //Realiza el login
                var result = await this._userService.SignInAsync(user, loginViewModel.Password, true).ConfigureAwait(false);
                if(result.Succeeded)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.LoginUserSuccess;
                    serviceResponse.Data = true;
                    serviceResponse.Success = true;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.LoginUserSuccess);
                    return serviceResponse;
                }
                else
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.VerifyPasswordAndEmail;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.VerifyPasswordAndEmail);
                    return serviceResponse;
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return serviceResponse;
            }
        }

        /// <summary>
        ///  Cerrar sesion Web.
        /// </summary>
        /// <returns></returns>
        public async Task LogoutUserWebAsync()
        {
           await this._userService.SignOutAsync().ConfigureAwait(false);
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotNew;
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
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                var user = await this._userService.GetUserByEmailAsync(resetPassword.Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EmailNotSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EmailNotSend);
                    return serviceResponse;
                }
                serviceResponse.Data = true;
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CodeRecoverPassword;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.CodeRecoverPassword);
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado
                var user = await this._userService.GetUserByEmailAsync(sendToNewCodeConfirmationRegister.Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Verifica que el usuario no haya confirmado el registro anteriormente.
                var verify_confirm_register = await this._userService.VerifyConfirmRegisterUserAsync(user).ConfigureAwait(false);
                if (verify_confirm_register)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserConfirmRegister;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EmailNotSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EmailNotSend);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EmailRegisterConfirmation;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EmailRegisterConfirmation);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.TokeConfirmRegisterBad;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.TokeConfirmRegisterBad);
                    return serviceResponse;
                }
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                var delete_image = await this._userService.DeleteImageProfileUserAsync(user).ConfigureAwait(false);
                if (!delete_image)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica que la imagen sea valida
                if (updateUser.ImageProfile != null)
                {
                    if (updateUser.ImageProfile.Length <= 0)
                    {
                        serviceResponse.Code = (int) GetValueResourceFile.KeyResource.ImageErrorValue;
                        serviceResponse.Data = null;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImageErrorValue);
                        return serviceResponse;
                    }
                    if (updateUser.ImageProfile.Length > Constants.MAX_LENTHG_IMAGE_PROFILE_USER)
                    {
                        serviceResponse.Code = (int) GetValueResourceFile.KeyResource.ImageUserNotValide;
                        serviceResponse.Data = null;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImageUserNotValide);
                        return serviceResponse;
                    }
                }
                var userName = claimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
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
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Obtiene la cuenta de usuario.
                var userName = claimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Obtiene al usuario.
                var user = await this._userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Verifica que la contraseña antigua sea valida.
                var verify_password = await this._userService.VerifyPasswordUserAsync(user, changePassword.PasswordOld).ConfigureAwait(false);
                if (!verify_password)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.PasswordNotCorrect;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.PasswordNotCorrect);
                    return serviceResponse;
                }
                //Cambia la contraseña 
                var change_password_correct = await this._userService.UpdatePasswordAsync(user, changePassword.PasswordOld, changePassword.PasswordNew).ConfigureAwait(false);
                if (!change_password_correct)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorDataBaseUserIdentity);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
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
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (formFile.Length <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (formFile.Length > Constants.MAX_LENTHG_IMAGE_PROFILE_USER)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImageUserNotValide;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImageUserNotValide);
                    return serviceResponse;
                }
                //Obtiene la cuenta de usuario.
                var userName = claimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Obtiene al usuario.
                var user = await this._userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
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
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
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

        /// <summary>
        /// Elimina un role a un usuario.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> RemoveRoleInUserAsync(int UserId, int RoleId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica si el usuario está disponible
                var user = await this._userService.GetUserByIdAsync(UserId).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Verfica si el role está disponible
                var role = await this._userService.VerifyRoleAsync(RoleId).ConfigureAwait(false);
                if(role == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.BadRole;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.BadRole);
                    return serviceResponse;
                }
                //Verifica si el usuario tiene el role
                var isrole = await this._userService
                .VerifyRoleInUserAsync(user, role)
                .ConfigureAwait(false);
                if (!isrole)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.IsNotRoleOfUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.IsNotRoleOfUser);
                    return serviceResponse;
                }
                //Elimina el role del usuario.
                var remove_role_owner = await this._userService
                .RemoveRoleOfUserAsync(user, role)
                .ConfigureAwait(false);
                if (!remove_role_owner)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.IsNotRemoveRoleOfUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.IsNotRemoveRoleOfUser);
                    return serviceResponse;
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
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Asigna un nuevo role a un usuario.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AssigningRoleToUserAsync(int UserId, int RoleId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica si el usuario está disponible
                var user = await this._userService.GetUserByIdAsync(UserId).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Verfica si el role está disponible
                var role = await this._userService.VerifyRoleAsync(RoleId).ConfigureAwait(false);
                if (role == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.BadRole;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.BadRole);
                    return serviceResponse;
                }
                //Verifica si el usuario tiene el role
                var verify_role = await this._userService
                .VerifyRoleInUserAsync(user, role)
                .ConfigureAwait(false);
                if(verify_role)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.IsUserHaveRole;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.IsUserHaveRole);
                    return serviceResponse;
                }
                //Asigna el role al usuario.
                var assign_role_owner = await this._userService
                .AddRoleForUserAsync(user, role)
                .ConfigureAwait(false);
                if (!assign_role_owner)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.IsNotAssignRoleOfUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.IsNotAssignRoleOfUser);
                    return serviceResponse;
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
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios disponibles con datos disponibles solo para usuarios admin.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetUserAllDataForOnlyAdminDto>>> GetAllDataOfUserAsync()
        {
            ServiceResponse<List<GetUserAllDataForOnlyAdminDto>> serviceResponse = new ServiceResponse<List<GetUserAllDataForOnlyAdminDto>>();
            try
            {
                var list_users = await this._userService.GetAllUserAsync().ConfigureAwait(false);
                if (list_users == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserAllNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserAllNotFound);
                    return serviceResponse;
                }
                //Mapea de una lista de User a una lista de GetUserDto.
                List<GetUserAllDataForOnlyAdminDto> userAllDataForOnlyAdmins = new List<GetUserAllDataForOnlyAdminDto>();
                foreach(User c in list_users)
                {
                    var useralldataforonlyadmins = new GetUserAllDataForOnlyAdminDto
                    {
                        Id = c.Id,
                        Address = c.Address,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        PhoneNumber = c.PhoneNumber,
                        ImageUserProfile = c.ImageUserProfile,
                        DateCreated = c.DateCreated,
                        DateUpdated = c.DateUpdated,
                        LastDateConnected = c.LastDateConnected,
                        Email = c.Email,
                        IdForClaim = c.IdForClaim,
                        RolesOfUsers = await this._userService.GetAllRoleOfUserAsync(c).ConfigureAwait(false),
                    };
                    userAllDataForOnlyAdmins.Add(useralldataforonlyadmins);
                }
                serviceResponse.Data = userAllDataForOnlyAdmins;
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene un usuario dado su Id con datos disponibles solo para usuarios admin.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetUserAllDataForOnlyAdminDto>> GetAllDataOfOnlyUserAsync(int UserId)
        {
            ServiceResponse<GetUserAllDataForOnlyAdminDto> serviceResponse = new ServiceResponse<GetUserAllDataForOnlyAdminDto>();
            try
            {
                var user = await this._userService.GetUserByIdAsync(UserId).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                //Mapea de User a GetUserDto.
                serviceResponse.Data = new GetUserAllDataForOnlyAdminDto
                {
                    Id = user.Id,
                    Address = user.Address,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    ImageUserProfile = user.ImageUserProfile,
                    DateCreated = user.DateCreated,
                    DateUpdated = user.DateUpdated,
                    LastDateConnected = user.LastDateConnected,
                    Email = user.Email,
                    IdForClaim = user.IdForClaim,
                    RolesOfUsers = await this._userService.GetAllRoleOfUserAsync(user).ConfigureAwait(false),
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene los roles disponibles de un usuario.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<string>>> GetAllRoleOfUser(int UserId)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                //Verifica que el usuario este registrado
                var user = await this._userService.GetUserByIdAsync(UserId).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                //Obtiene los roles del usuario.
                serviceResponse.Data = await this._userService.GetAllRoleOfUserAsync(user).ConfigureAwait(false);
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

      
        /// <summary>
        /// Establece si un usuario se banea o no.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="banner"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> BannerUserAsync(int UserId, bool banner)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtiene los usuarios de un determinado role.
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetUserDto>>> GetAllUserWithRoleAsync(int RoleId)
        {
            ServiceResponse<List<GetUserDto>> serviceResponse = new ServiceResponse<List<GetUserDto>>();
            try
            {
                var list_users = await this._userService.GetAllUserWithRoleAsync(RoleId).ConfigureAwait(false);
                if (list_users == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserAllNotFoundWithRole;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserAllNotFoundWithRole);
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
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene los usuarios admins del sistema.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetUserAllDataForOnlyAdminDto>>> GetAllUserAdminsAsync()
        {
            ServiceResponse<List<GetUserAllDataForOnlyAdminDto>> serviceResponse = new ServiceResponse<List<GetUserAllDataForOnlyAdminDto>>();
            try
            {
                var list_users = await this._userService.GetAllUserWithRoleAsync(1).ConfigureAwait(false);
                if (list_users == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserAllNotFoundWithRole;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserAllNotFoundWithRole);
                    return serviceResponse;
                }
                //Mapea de una lista de User a una lista de GetUserDto.
                serviceResponse.Data = list_users.Select(c => new GetUserAllDataForOnlyAdminDto
                {
                    Address = c.Address,
                    FirstName = c.FirstName,
                    Id = c.Id,
                    LastName = c.LastName,
                    PhoneNumber = c.PhoneNumber,
                    ImageUserProfile = c.ImageUserProfile,
                    Email = c.Email,
                }).ToList();
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Elimina un usuario admin a la lista de los que deben recibir notificaciones.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> RemoveUserAdminForNotifications(int UserId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var user_notifications = await this._serviceGenericUserAdminsNotificationsHelper
                .WhereFirstEntityAsync(c => c.User.Id == UserId, c => c.User)
                .ConfigureAwait(false);
                if (user_notifications == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserAdminNotExistForNotifications;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserAdminNotExistForNotifications);
                    return serviceResponse;
                }
                //Elimina la relación.
                this._serviceGenericUserAdminsNotificationsHelper.RemoveEntity(user_notifications);
                await this._serviceGenericUserAdminsNotificationsHelper.SaveChangesBDAsync().ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = true;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Agrega un usuario admin a la lista de los que deben recibir notificaciones.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddUserAdminForNotifications(int UserId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var user = await this._userService.GetUserByIdAsync(UserId).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Verifica que el usuario no se encuentre en la lista ya.
                var user_existing = await this._serviceGenericUserAdminsNotificationsHelper
                .WhereFirstEntityAsync(c => c.User == user, c => c.User).ConfigureAwait(false);
                if(user_existing != null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserAdminExistForNotifications;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserAdminExistForNotifications);
                    return serviceResponse;
                }
                //Verifica que el role admin este disponible
                var exist_role = await this._userService.VerifyRoleAsync(Constants.RolesOfSystem[0]).ConfigureAwait(false);
                if(!exist_role)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.BadRole;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.BadRole);
                    return serviceResponse;
                }
                //Verfica si el usuario tiene el role admin.
                var have_role_admin = await this._userService.VerifyRoleInUserAsync(user, Constants.RolesOfSystem[0]).ConfigureAwait(false);
                if(!have_role_admin)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.IsNotRoleOfUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.IsNotRoleOfUser);
                    return serviceResponse;
                }
                //Verifica si el usuario tiene el role
                var isrole = await this._userService
                .VerifyRoleInUserAsync(user, Constants.RolesOfSystem[0])
                .ConfigureAwait(false);
                if (!isrole)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.IsNotRoleOfUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.IsNotRoleOfUser);
                    return serviceResponse;
                }
                var user_admins_notifications = new UserAdminsNotifications
                {
                    User = user
                };
                await this._serviceGenericUserAdminsNotificationsHelper.AddEntityAsync(user_admins_notifications).ConfigureAwait(false);
                await this._serviceGenericUserAdminsNotificationsHelper.SaveChangesBDAsync();
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
        /// Agrega o actualiza el chat id de telegram de un usuario admin para recibir las notificaciones
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="TelegramChatId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdateChatIdTelegramUserAdminForNotifications(int UserId, int TelegramChatId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que el usuario existe
                var user = await this._userService.GetUserByIdAsync(UserId).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                //Verifica que el usuario sea admin y puede recibir las notificaciones de ordenes.
                var user_existing = await this._serviceGenericUserAdminsNotificationsHelper
                .WhereFirstEntityAsync(c => c.User == user, c => c.User).ConfigureAwait(false);
                if (user_existing == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserAdminNotExistForNotifications;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserAdminNotExistForNotifications);
                    return serviceResponse;
                }
                user_existing.UserTelegramChatId = TelegramChatId;
                this._serviceGenericUserAdminsNotificationsHelper.UpdateEntity(user_existing);
                await this._serviceGenericUserAdminsNotificationsHelper.SaveChangesBDAsync();
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

        
    }
}
