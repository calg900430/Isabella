namespace Duma.API.Services
{
    using Common;
    using Common.Dtos.Users;
    using Common.Extras;
    using Data;
    using Extras;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Models;
    using Repositorys;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Servicio para el manejo de usuarios.
    /// </summary>
    public class UserServiceAPI : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IMailRepository _mailService;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        /// <summary>
        /// Constructor de UserService.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="mailService"></param>
        /// <param name="roleManager"></param>
        /// <param name="dataContext"></param>
        /// <param name="configuration"></param>
        public UserServiceAPI(DataContext dataContext, IConfiguration configuration, RoleManager<IdentityRole<int>> roleManager,
        UserManager<User> userManager, IMailRepository mailService)
        {
            this._dataContext = dataContext;
            this._configuration = configuration;
            this._userManager = userManager;
            this._mailService = mailService;
            this._roleManager = roleManager;
        }


        /// <summary>
        /// Agrega un usuario en la Base de Datos.
        /// </summary>
        /// <param name="newuser"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddUserAsync(RegisterUserDto newuser, string role)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que los campos para el registro del usuario no esten vacios.
                if (newuser == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Verifica que la nueva contraseña es igual que la de confirmación.
                if (newuser.Password != newuser.PasswordConfirm)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_PasswordsNotEquals;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_PasswordsNotEquals);
                    return serviceResponse;
                }
                //Verificar que el email no este en uso.
                var user_email = await this._userManager.FindByEmailAsync(newuser.Email).ConfigureAwait(false);
                if (user_email != null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_BadEmail;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_BadEmail);
                    return serviceResponse;
                }
                //Verificar que la cuenta de usuario no este en uso.
                var user_useraccount = await this._userManager.FindByNameAsync(newuser.UserName).ConfigureAwait(false);
                if (user_useraccount != null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_BadUserName;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_BadUserName);
                    return serviceResponse;
                }
                //Verifica si el role existe en la base de datos
                var role_existing = await this._roleManager.RoleExistsAsync(role).ConfigureAwait(false);
                if(!role_existing)
                {
                   serviceResponse.Code = CodeMessage.Code.CodeRole_BadRole;
                   serviceResponse.Data = false;
                   serviceResponse.Success = false;
                   serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeRole_BadRole);
                   return serviceResponse;
                }
                //Mapea de RegisterUserDto a User
                var user = new User
                { 
                   Email = newuser.Email,
                   UserName = newuser.UserName,
                   DateCreated = DateTime.UtcNow,
                   DateUpdated = DateTime.UtcNow,
                   LastDateConnected = DateTime.UtcNow,
                   CodeUser = Guid.NewGuid(),
                   RecoverPassword = false,
                   EmailConfirmed = false,
                };
                //Guarda al usuario en la base de datos.
                var user_identity = await this._userManager.CreateAsync(user, newuser.Password).ConfigureAwait(false);
                //No se agregó el usuario
                if (user_identity.Succeeded == false)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_DataBase;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_DataBase);
                    return serviceResponse;
                }
                //Agrega al nuevo usuario el rol.
                var identity_result = await this._userManager.AddToRoleAsync(user, role).ConfigureAwait(false);
                if (!identity_result.Succeeded)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_DataBase;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_DataBase);
                    return serviceResponse;
                }
                if(role == Constants.RolesOfSystem[1])
                {
                   var user_baber = new UserBarber
                   {
                      User = user
                   };
                   await this._dataContext.UserBarbers.AddAsync(user_baber).ConfigureAwait(false);
                   await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                }
                if(role == Constants.RolesOfSystem[2])
                {
                    var user_baber = new UserClient
                    {
                        User = user
                    };
                    await this._dataContext.UserClients.AddAsync(user_baber).ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                }
                //Genera un número aleatorio que se envia al correo del usuario.
                //TODO: Mejorar la semilla del Random del código de confirmación del correo.
                Random random = new Random(DateTime.UtcNow.Millisecond);
                var Token = random.Next(100000, 999999);
                var body_message = $"Hello, you have registered in the Duma application, " +
                $"to finish the registration below you are shown the registration confirmation code. \n Confirmation Code {Token} ";
                //Envia el correo al usuario con el código de confirmación del registro.
                this._mailService.SendMail(user.Email, "Code Confirmation for registration in the Duma app", body_message);
                //Guarda la relación del usuario y su código de registro.
                ConfirmationRegisterForEmail confirmationEmail = new ConfirmationRegisterForEmail
                {
                   Token = Token.ToString(),
                   User = user,
                };
                await this._dataContext.ConfirmationRegisterForEmail.AddAsync(confirmationEmail).ConfigureAwait(false);
                await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                serviceResponse.Code = CodeMessage.Code.EmailRegisterConfirmation;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.EmailRegisterConfirmation);
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
        /// Agrega un usuario en la Base de Datos a través de login por proveedor externo.
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<User> AddUserAsyncForExternProvider(string Email, string role)
        {
            try
            {
                //Verifica que los campos para el registro del usuario no esten vacios.
                if (Email == null || role == null)
                return null;
                //Verificar que el email no este en uso si está en uso devuelve al usuario.
                var user_email = await this._userManager.FindByEmailAsync(Email).ConfigureAwait(false);
                if (user_email != null)
                return user_email;
                //Mapea de RegisterUserDto a User
                var user = new User
                {
                    Email = Email,
                    UserName = Email,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    LastDateConnected = DateTime.UtcNow,
                    CodeUser = Guid.NewGuid(),
                    RecoverPassword = false,
                    EmailConfirmed = true,
                };
                //Guarda al usuario en la base de datos.
                var user_identity = await this._userManager.CreateAsync(user).ConfigureAwait(false);
                //No se agregó el usuario
                if (user_identity.Succeeded == false)
                return null;
                //Agrega al nuevo usuario el rol.
                var identity_result = await this._userManager.AddToRoleAsync(user, role).ConfigureAwait(false);
                if (!identity_result.Succeeded)
                return null;
                if (role == Constants.RolesOfSystem[1])
                {
                    var user_baber = new UserBarber
                    {
                        User = user
                    };
                    await this._dataContext.UserBarbers.AddAsync(user_baber).ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                }
                if (role == Constants.RolesOfSystem[2])
                {
                    var user_client = new UserClient
                    {
                        User = user
                    };
                    await this._dataContext.UserClients.AddAsync(user_client).ConfigureAwait(false);
                    await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                }
                return user;
            }
            catch (Exception)
            {
                return null;
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
                 //Obtiene todos los usuarios del sistema
                 var list_users = await this._dataContext.Users.ToListAsync().ConfigureAwait(false);
                 if(list_users == null)
                 {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_AllNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_AllNotFound);
                    return serviceResponse;
                 }
                 //Mapea de User a GetUserDto
                 serviceResponse.Data = list_users.Select(c => new GetUserDto 
                 { 
                     Address = c.Address,
                     FirstName = c.FirstName,
                     Id = c.Id,
                     //ImageFullPath = c.ImageFullPath,
                     //ImageUserPath = c.ImageUserPath,
                     LastName = c.LastName,
                     PhoneNumber = c.PhoneNumber,
                     ImageUserProfile = c.ImageUserProfile,
                 }).ToList();
                 serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                 serviceResponse.Success = true;
                 serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                 return serviceResponse;  
             }
             catch (Exception)
             {
                 serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                 serviceResponse.Data = null;
                 serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                 serviceResponse.Success = false;
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
                if (UserName == null || UserName == "")
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                var user = await this._userManager.FindByNameAsync(UserName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = new GetUserDto
                {
                    Id = user.Id,
                    Address = user.Address,
                    FirstName = user.FirstName,
                    //ImageFullPath = user.ImageFullPath,
                    //ImageUserPath = user.ImageUserPath,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    ImageUserProfile = user.ImageUserProfile,
                };
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Data = null;
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene un usuario dado su Id de usuario.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetUserDto>> GetUserByIdAsync(int UserId)
        {
            ServiceResponse<GetUserDto> serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                var user = await this._userManager.FindByIdAsync(UserId.ToString()).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = new GetUserDto
                {
                    Id = user.Id,
                    Address = user.Address,
                    FirstName = user.FirstName,
                    //ImageFullPath = user.ImageFullPath,
                    //ImageUserPath = user.ImageUserPath,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    ImageUserProfile = user.ImageUserProfile
                };
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Data = null;
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene un usuario dado su Email de usuario.
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetUserDto>> GetUserByEmailAsync(string Email)
        {
            ServiceResponse<GetUserDto> serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                if (Email == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                var user = await this._userManager.FindByEmailAsync(Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = new GetUserDto
                {
                    Id = user.Id,
                    Address = user.Address,
                    FirstName = user.FirstName,
                    //ImageFullPath = user.ImageFullPath,
                    //ImageUserPath = user.ImageUserPath,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    ImageUserProfile = user.ImageUserProfile,
                };
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Data = null;
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Devuelve una lista de usuarios a partir de un Id de referencia.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="CantUsers"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetUserDto>>> GetCantUserAsync(int UserId, int CantUsers)
        {
            ServiceResponse<List<GetUserDto>> serviceResponse = new ServiceResponse<List<GetUserDto>>();
            try
            {
                if(CantUsers < 1 )
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_ValueNotValide;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_ValueNotValide);
                    return serviceResponse;
                }
                //Obtiene todos los usuarios del sistema
                var all_users = await this._dataContext.Users.ToListAsync().ConfigureAwait(false);
                if (all_users == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_AllNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_AllNotFound);
                    return serviceResponse;
                }
                //Obtiene el usuario de referencia.
                var user_reference = all_users.Where(c => c.Id == UserId).FirstOrDefault();
                if (user_reference == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Verifica si no hay nuevos usuarios.
                var begin_found_user = all_users.FirstOrDefault(c => c.Id == user_reference.Id + 1);
                if (begin_found_user == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotNewUser;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotNewUser);
                    return serviceResponse;
                }
                //Obtiene el index del usuario por el cual se debe comenzar a buscar los demas.
                var index_user = all_users.LastIndexOf(begin_found_user);
                //Obtiene el Index del ultimo usuario.
                var last_index = all_users.LastIndexOf(all_users.LastOrDefault());
                //Obtiene la cantidad de usuarios disponibles a partir del usuario de referencia.
                var users_available = last_index - index_user + 1;
                //Envia la cantidad de usuarios disponibles.
                if (CantUsers >= users_available)
                {
                    //Toma una cantidad de elementos contiguos a partir de un index de referencia.
                    var list_user_to_send = all_users.GetRange(index_user, users_available);
                    var list_users_rol = list_user_to_send.Select(c => new GetUserDto
                    {
                        ImageUserProfile = c.ImageUserProfile,
                        Address = c.Address,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        PhoneNumber = c.PhoneNumber,
                        Id = c.Id,
                    }).ToList();
                    serviceResponse.Data = list_users_rol;
                    serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                    serviceResponse.Success = true;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                    return serviceResponse;
                }
                //Envia las publicaciones solicitadas por el usuario
                else
                {
                    //Toma una cantidad de elementos contiguos a partir de un index de referencia.
                    var list_user_to_send = all_users.GetRange(index_user, CantUsers);
                    var list_users_rol = list_user_to_send.Select(c => new GetUserDto
                    {
                        ImageUserProfile = c.ImageUserProfile,
                        Address = c.Address,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        PhoneNumber = c.PhoneNumber,
                        Id = c.Id
                    }).ToList();
                    serviceResponse.Data = list_users_rol;
                    serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                    serviceResponse.Success = true;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                    return serviceResponse;
                }
            }
            catch (Exception)
            {
                serviceResponse.Data = null;
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene un usuario dado su CodeUser.
        /// </summary>
        /// <param name="CodeUser"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<User>> GetUserByCodeUserAsync(string CodeUser)
        {
            ServiceResponse<User> serviceResponse = new ServiceResponse<User>();
            try
            {
                var user = await this._dataContext.Users
                .FirstOrDefaultAsync(c => c.CodeUser == Guid.Parse(CodeUser))
                .ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = user;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Data = null;
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Cambia la contraseña del usuario.
        /// </summary>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<IdentityResult>> UpdatePasswordAsync(ChangePasswordUserDto changePassword)
        {
            ServiceResponse<IdentityResult> serviceResponse = new ServiceResponse<IdentityResult>();
            try
            {
                if (changePassword == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Obtiene el usuario al que se le desea cambiar la contraseña
                var user = await this._dataContext.Users
                .FirstOrDefaultAsync(c => c.CodeUser == Guid.Parse(changePassword.CodeUser))
                .ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Intenta actualizar la contraseña del usuario a la base de datos
                var identity_user = await this._userManager
                .ChangePasswordAsync(user, changePassword.PasswordOld, changePassword.PasswordNew)
                .ConfigureAwait(false);
                //No se actualizó la contraseña del usuario
                if (!identity_user.Succeeded)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    foreach (IdentityError identityError in identity_user.Errors)
                    serviceResponse.Message += identityError.Description;
                    return serviceResponse;
                }
                //Se actualizó la contraseña del usuario
                serviceResponse.Data = identity_user;
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Data = null;
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Actualiza todos los campos que desee el usuario menos el correo.
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetUserDto>> UpdateUserAsync(UpdateUserDto updateUser)
        {
            ServiceResponse<GetUserDto> serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                if (updateUser == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                var user = await this._dataContext.Users
                .FirstOrDefaultAsync(c => c.CodeUser == Guid.Parse(updateUser.CodeUser))
                .ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Verifica que el usuario se encuentra en la base de datos
                if (updateUser.FirstName != null)
                user.FirstName = updateUser.FirstName;
                if (updateUser.LastName != null)
                user.LastName = updateUser.LastName;
                if (updateUser.PhoneNumber != null)
                user.PhoneNumber = updateUser.PhoneNumber.ToString();
                if (updateUser.Address != null)
                user.Address = updateUser.Address;
                //Cambia la fecha de actualización del usuario
                user.DateUpdated = DateTime.UtcNow;
                //Actualiza el usuario en la base de datos
                var identity_result = await this._userManager.UpdateAsync(user).ConfigureAwait(false);
                if (!identity_result.Succeeded)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    foreach (IdentityError identityError in identity_result.Errors)
                    serviceResponse.Message += identityError.Description;
                    return serviceResponse;
                }
                //Mapea de User a GetUserDto
                serviceResponse.Data = new GetUserDto
                {
                    Id = user.Id,
                    Address = user.Address,
                    FirstName = user.FirstName,
                    //ImageFullPath = user.ImageFullPath,
                    //ImageUserPath = user.ImageUserPath,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                };
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Data = null;
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Crea un Token Web Json.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<Token_DateTimeExpired_Roles> CreateTokenAsync(User user)
        {
            List<Claim> claims = new List<Claim>();
            //Genera un número aleatorio con esto hago que el Token sea diferente cada vez
            Random random = new Random(DateTime.Now.Hour);
            //Obtiene los roles del usuario
            var list_roles = await this._userManager.GetRolesAsync(user).ConfigureAwait(false);
            foreach (string role in list_roles)
             claims.Add(new Claim(ClaimTypes.Role, role));
            //Agrega como reclamo Identity.Name
            claims.Add(new Claim(ClaimTypes.Name, user.CodeUser.ToString()));
            //Agrega como reclamo para el identificador de nombres el Id del usuario.
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            //Agrega un reclamo que usa el Id del usuario
            claims.Add(new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()));
            //Agrega un reclamo que usa el Email del usuario
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Email));
            //Usamos como reclamo la generación de Guid, cada llamado es único 
            //con esto no hacemos llamadas estandar y nos protegemos de los hackers.
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            //Utiliza como número serial un número aleatorio que utiliza como semilla la hora de login
            claims.Add(new Claim(ClaimTypes.SerialNumber, random.Next(1, int.MaxValue).ToString()));
            //Leemos el Key de nuestro archivo de configuración y lo almacenamos como una llave de seguridad simetrica.
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.
            GetSection("AppSettings:Token").Value));
            //Creamos nuevas credenciales con un algoritmo de encriptado(HmacSha256(Es uno de los más seguros))
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //Configuramos el futuro Token
            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), //Lista de Claims
                Expires = DateTime.UtcNow.AddDays(30), //Tiempo de expiración del Token(30 días)
                SigningCredentials = credentials,
                Issuer = _configuration["AppSettings:Issuer"],
                Audience = _configuration["AppSettings:Audience"],
            };
            //Designado para crear y validar Json Web Tokens
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            //Crea un Token Web Json
            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            //Accede a la fecha de expiración del Token
            var data_expires = securityTokenDescriptor.Expires;
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);
            //Envia el Token creado
            var enum_roles = ManagerRolesForUsers.GetRoles(list_roles);
            var token_datetime_expired = new Token_DateTimeExpired_Roles
            {
                UserRoles = enum_roles,
                DateTime = data_expires,
                Token = token
            };
            return token_datetime_expired;
        }

        /// <summary>
        /// Crea un Token JWT para el usuario.
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<ResponseLoginTokenDto>> LoginUserForApiAsync(LoginUserWithUserNameDto loginUser)
        {
            ServiceResponse<ResponseLoginTokenDto> serviceResponse = new ServiceResponse<ResponseLoginTokenDto>();
            try
            {
                if (loginUser == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Verifica que el usuario se encuentre en la base de datos
                var user = await this._userManager.FindByNameAsync(loginUser.UserName)
                .ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Verifica que el usuario haya verificado el registro por correo
                if (!await this._userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false))
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotConfirmRegister;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotConfirmRegister);
                    return serviceResponse;
                }
                //Verifica que la contraseña del usuario sea correcta
                bool result = await _userManager.CheckPasswordAsync(user, loginUser.Password).ConfigureAwait(false);
                if (result)
                {
                    var token_dateexpires = await CreateTokenAsync(user);
                    if (token_dateexpires == null)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Code = CodeMessage.Code.CodeUser_NotConfirmRegister;
                        serviceResponse.Success = false;
                        serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotConfirmRegister);
                        return serviceResponse;
                    }
                    var getuser = new GetUserDto
                    {
                        Id = user.Id,
                        Address = user.Address,
                        FirstName = user.FirstName,
                        //ImageFullPath = user.ImageFullPath,
                        //ImageUserPath = user.ImageUserPath,
                        ImageUserProfile = user.ImageUserProfile,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber
                    };
                    ResponseLoginTokenDto responseLoginToken = new ResponseLoginTokenDto
                    {
                        Roles = token_dateexpires.UserRoles,
                        CodeUser = user.CodeUser.ToString(),
                        DateExpiration = token_dateexpires.DateTime,
                        Token = token_dateexpires.Token,
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
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = "Verifique la contraseña y el usuario.";
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Data = null;
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Confirma el registro del nuevo usuario.
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
                //Verifica el correo del usuario
                var user = await this._userManager.FindByEmailAsync(confirmEmail.Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Verifica que el usuario aún no haya confirmado el registro anteriormente
                if (user.EmailConfirmed == true)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_YesConfirmRegister;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_YesConfirmRegister);
                    return serviceResponse;
                }
                //Obtiene la relación del usuario con su código de confirmación. 
                var code_user = await this._dataContext.ConfirmationRegisterForEmail
                .Where(c => c.User.Id == user.Id && c.Token == confirmEmail.Token)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
                if (code_user == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_CodeVerificationNotCorrect;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_CodeVerificationNotCorrect);
                    return serviceResponse;
                }
                //Confirma el correo electrónico.
                var Token = await this._userManager.GenerateEmailConfirmationTokenAsync(user)
                .ConfigureAwait(false);
                await this._userManager.ConfirmEmailAsync(user, Token).ConfigureAwait(false);
                //Elimina la relación ya que el usuario confirmó el registro.
                this._dataContext.ConfirmationRegisterForEmail.Remove(code_user);
                await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                serviceResponse.Data = true;
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Data = false;
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Envia un nuevo correo para la confirmación del registro.
        /// </summary>
        /// <param name="sendToNewCodeConfirmationRegister"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> SendToNewCodeConfirmationEmail(SendToNewCodeConfirmationRegisterDto sendToNewCodeConfirmationRegister)
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
                //Verifica que el usuario se encuentre en la base de datos.
                var user = await this._userManager.FindByEmailAsync(sendToNewCodeConfirmationRegister.Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Verifica que el usuario aún no haya confirmado el registro anteriormente
                if (user.EmailConfirmed == true)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_YesConfirmRegister;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_YesConfirmRegister);
                    return serviceResponse;
                }
                //Verifica si hay codigos de confirmación de registro del usuario almacenados anteriores. 
                var old_codes = await this._dataContext.ConfirmationRegisterForEmail
                .Include(c => c.User)
                .Where(c => c.User == user).ToListAsync();
                if (user.EmailConfirmed == true)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_YesConfirmRegister;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_YesConfirmRegister);
                    return serviceResponse;
                }
                //Si hay códigos anteriores los elimina, para generar uno nuevo.
                if (old_codes != null)
                { 
                    this._dataContext.ConfirmationRegisterForEmail.RemoveRange(old_codes);
                    await this._dataContext.SaveChangesAsync();
                }
                //Genera un número aleatorio que se envia al correo del usuario.
                //TODO: Mejorar la semilla del Random del código de confirmación del correo.
                Random random = new Random(DateTime.UtcNow.Millisecond);
                var Token = random.Next(100000, 999999);
                var body_message = $"Hello, you have registered in the Duma application, " +
                $"to finish the registration below you are shown the registration confirmation code. \n Confirmation Code {Token} ";
                //Envia el correo al usuario con el código de confirmación del registro.
                this._mailService.SendMail(user.Email, "Code Confirmation for registration in the Duma app", body_message);
                //Guarda la relación del usuario y su código de registro.
                ConfirmationRegisterForEmail confirmationEmail = new ConfirmationRegisterForEmail
                {
                    Token = Token.ToString(),
                    User = user,
                };
                await this._dataContext.ConfirmationRegisterForEmail.AddAsync(confirmationEmail).ConfigureAwait(false);
                await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                serviceResponse.Code = CodeMessage.Code.EmailRegisterConfirmation;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.EmailRegisterConfirmation);
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return serviceResponse;
            }
        }

        /// <summary>
        /// Envia un correo al usuario con los detalles para la recuperaión de su contraseña.
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
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
                //Verifica que el usuario se encuentre en la base de datos.
                var user = await this._userManager.FindByEmailAsync(resetPassword.Email).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Verifica si hay codigos de recuperación de contraseñas del usuario almacenados anteriores. 
                var old_codes = await this._dataContext.RecoverPasswords
                .Include(c => c.User)
                .Where(c => c.User == user).ToListAsync();
                //Si hay códigos anteriores los elimina, para generar uno nuevo.
                if(old_codes != null)
                {
                    this._dataContext.RecoverPasswords.RemoveRange(old_codes);
                    await this._dataContext.SaveChangesAsync();
                }
                //Envia un correo para la recuperación de la contraseña del usuario.
                //Genera un número aleatorio que se envia al correo del usuario.
                //TODO: Mejorar la semilla del Random del código de confirmación del correo.
                Random random = new Random(DateTime.UtcNow.Millisecond);
                var Token = random.Next(100000, 999999);
                var body_message = $"Hello, you have requested a code for the recovery of your password" +
                $" in the Duma app below we show you recovery code itself.\n Recuperation Password Code {Token}";
                this._mailService.SendMail(user.Email, "Password recovery code in the Duma app", body_message);
                //Guarda la relación del usuario y el código.
                RecoverPassword recoverPassword = new RecoverPassword
                {
                    Token = Token.ToString(),
                    User = user,
                };
                //Guarda el código de recuperación.
                await this._dataContext.RecoverPasswords.AddAsync(recoverPassword).ConfigureAwait(false);
                await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                //Establece la bandera de recuperación de contraseña en true;
                user.RecoverPassword = true;
                //Actualiza el usuario
                this._dataContext.Users.Update(user);
                await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                serviceResponse.Data = true;
                serviceResponse.Code = CodeMessage.Code.CodeRecoverPassword;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeRecoverPassword);
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
        /// Recupera la contraseña del usuario contraseña del usuario. 
        /// </summary>
        /// <param name="recoverPassword"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> RecoverPasswwordUserAsync(RecoverPasswordDto recoverPassword)
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
                //Verifica que la nueva contraseña es igual que la de confirmación.
                if (recoverPassword.NewPasswordConfirm != recoverPassword.NewPassword)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_PasswordsNotEquals;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_PasswordsNotEquals);
                    return serviceResponse;
                }
                //Verifica que el usuario existe.
                var user = await this._dataContext.Users.Where(c => c.Email == recoverPassword.Email)
                .FirstOrDefaultAsync().ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                //Verifica si el usuario solicito la recuperación de contraseña
                if (user.RecoverPassword == false)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotRecoverPassword;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotRecoverPassword);
                    return serviceResponse;
                }
                //Verifica que el código de recuperación.
                var recover_password = await this._dataContext.RecoverPasswords
                .Where(c => c.User == user && c.Token == recoverPassword.Token)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
                if (recover_password == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_CodeVerificationNotCorrect;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_CodeVerificationNotCorrect);
                    return serviceResponse;
                }
                //Genera el Token de recuperación de contraseña.
                var token = await this._userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
                //Confirma el código de recuperación
                await this._userManager.ResetPasswordAsync(user, token, recoverPassword.NewPassword).ConfigureAwait(false);
                //Elimina la relación ya que el usuario confirmo el registro.
                this._dataContext.RecoverPasswords.Remove(recover_password);
                await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                //Establece la bandera de recuperación de contraseña en false;
                user.RecoverPassword = false;
                //Actualiza el usuario
                this._dataContext.Users.Update(user);
                await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                serviceResponse.Data = true;
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Data = false;
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene el Id del último usuario en la base de datos.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<int>> GetIdOfLastUser()
        {
            ServiceResponse<int> serviceResponse = new ServiceResponse<int>();
            try
            {
                var list_users = await this._dataContext.Users.ToListAsync().ConfigureAwait(false);
                if (list_users == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_AllNotFound;
                    serviceResponse.Data = -1;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_AllNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = list_users.LastOrDefault().Id;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Data = -1;
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Verifica si el usuario tiene un role determinado
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> VerifyUserIfRoleBarber(User user, string role)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (user == null || role == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Verifica si el role existe.
                var existing_roles = await this._roleManager.RoleExistsAsync(role).ConfigureAwait(false);
                if (user == null || role == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeRole_BadRole;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeRole_BadRole);
                    return serviceResponse;
                }
                //Verifica si el usuario tiene el rol determinado
                var user_role = await this._userManager.IsInRoleAsync(user, role).ConfigureAwait(false);
                if (user_role == false)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Code = CodeMessage.Code.CodeRole_RoleNotAuthorization;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeRole_RoleNotAuthorization);
                    return serviceResponse;
                }
                serviceResponse.Data = true;
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Data = false;
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
           
        }

        /// <summary>
        /// Agrega una imagen de perfil de un usuario determinado
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="CodeUser"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddImageProfileUserAsync(IFormFile formFile, string CodeUser)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que el usuario exista.
                var user = await this._dataContext.Users
                .FirstOrDefaultAsync(c => c.CodeUser == Guid.Parse(CodeUser))
                .ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
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
                if (formFile.Length > Constants.MAX_LENTHG_IMAGE_LOGO_BARBERSHOP)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ImageBarberShopNotValide;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ImageBarberShopNotValide);
                    return serviceResponse;
                }
                //Nombre de la imagen
                var file = $"{Guid.NewGuid()}.jpg";
                //Ruta temporal donde la guardaremos antes de enviarla a la base de datos.
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\temp\\users", file);
                //Crea el archivo de la imagen que se encuentra en memoria RAM y lo guarda en la ruta seleccionada.
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream).ConfigureAwait(false);
                };
                //Verifica si se guardo la imagen.
                var exiting = System.IO.File.Exists(path);
                if (!exiting)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ImageErrorCreated;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ImageErrorCreated);
                    return serviceResponse;
                }
                //Obtiene el archivo de imagen
                var arraybyte_image = System.IO.File.ReadAllBytes(path);
                if (arraybyte_image.Length <= 0)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ImageErrorCreated;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ImageErrorCreated);
                    return serviceResponse;
                }
                //Actualiza la imagen en la base de datos
                user.ImageUserProfile = arraybyte_image;
                this._dataContext.Users.Update(user);
                await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
                //Elimina la imagen
                System.IO.File.Delete(path);
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
        /// Borra la imagen de perfil de un usuario determinado.
        /// </summary>
        /// <param name="CodeUser"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> DeleteImageProfileUserAsync(string CodeUser)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var user = await this._dataContext.Users
               .FirstOrDefaultAsync(c => c.CodeUser == Guid.Parse(CodeUser))
               .ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeUser_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_NotFound);
                    return serviceResponse;
                }
                user.ImageUserProfile = null;
                //Actualiza y guarda los cambios en la base de datos.
                this._dataContext.Users.Update(user);
                await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
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
    }
}
