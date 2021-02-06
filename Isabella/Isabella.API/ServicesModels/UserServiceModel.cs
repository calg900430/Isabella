namespace Isabella.API.ServicesModels
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.IdentityModel.Tokens.Jwt;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Configuration;

    using Data;
    using Models;
    using RepositorysModels;
    using API.Extras;
    using Common;
    using Microsoft.AspNetCore.Http;
    using System.IO;

    /// <summary>
    /// Servicio para la entidad que representa los usuarios.
    /// </summary>
    public class UserServiceModel : IUserRepositoryModel
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly MailServiceModel _mailService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="mailService"></param>
        /// <param name="configuration"></param>
        public UserServiceModel(DataContext dataContext, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager,
         MailServiceModel mailService, IConfiguration configuration)
        {
            this._dataContext = dataContext;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._mailService = mailService;
            this._configuration = configuration;
        }

        /// <summary>
        /// Agrega un usuario con contraseña definida.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User> AddUserAsync(User user, string password)
        {
            //Guarda al usuario en la base de datos.
            var user_identity = await this._userManager.CreateAsync(user, password).ConfigureAwait(false);
            //No se agregó el usuario
            if (user_identity.Succeeded == false)
            return null;
            return user;    
        }

        /// <summary>
        /// Agrega un usuario sin contraseña definida(Esto es para login con proveedor externo).
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<User> AddUserAsync(User user)
        {
            //Guarda al usuario en la base de datos.
            var user_identity = await this._userManager.CreateAsync(user).ConfigureAwait(false);
            //No se agregó el usuario
            if (user_identity.Succeeded == false)
            return null;
            return user;
        }

        /// <summary>
        /// Agrega un role determinado a un usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<bool> AddRoleForUserAsync(User user, string role)
        {
            //Agrega al nuevo usuario el rol.
            var identity_result = await this._userManager.AddToRoleAsync(user, role).ConfigureAwait(false);
            if (!identity_result.Succeeded)
            return false;
            return true;
        }
        /// <summary>
        /// Verifica si un email está en uso.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> VerifyEmailAsync(string email)
        {
            //Verificar que el email no este en uso.
            var user_email = await this._userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user_email != null)
            return true;
            return false; 
        }

        /// <summary>
        /// Verifica si una cuenta de usuario está en uso.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<bool> VerifyUserNameAsync(string username)
        {
            //Verificar que la cuenta de usuario no este en uso.
            var user_useraccount = await this._userManager.FindByNameAsync(username).ConfigureAwait(false);
            if (user_useraccount != null)
            return true;
            return false;
        }

        /// <summary>
        /// Envia un correo al usuario con el código para confirmación del registro en la aplicación.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> SendEmailWithCodeConfirmRegisterAsync(User user)
        {
            //Verifica si hay codigos de confirmación de registro del usuario almacenados anteriores. 
            var old_codes = await this._dataContext.ConfirmationRegisterForEmail
            .Include(c => c.User)
            .Where(c => c.User == user).ToListAsync();
            //Si hay códigos anteriores los elimina, para generar uno nuevo.
            if (old_codes != null)
            {
                this._dataContext.ConfirmationRegisterForEmail.RemoveRange(old_codes);
                await this._dataContext.SaveChangesAsync();
            }
            //TODO: Mejorar la semilla del Random del código de confirmación del correo.
            Random random = new Random(DateTime.UtcNow.Millisecond);
            var Token = random.Next(100000, 999999);
            var body_message = $"Hello, you have registered in the Duma application, " +
            $"to finish the registration below you are shown the registration confirmation code. \n Confirmation Code {Token} ";
            //Envia el correo al usuario con el código de confirmación del registro.
            var send_email = this._mailService.SendMail(user.Email, "Code Confirmation for registration in the Duma app", body_message);
            if(!send_email)
            return false;
            //Guarda la relación del usuario y su código de registro.
            ConfirmationRegisterForEmail confirmationEmail = new ConfirmationRegisterForEmail
            {
                Token = Token.ToString(),
                User = user,
            };
            await this._dataContext.ConfirmationRegisterForEmail.AddAsync(confirmationEmail).ConfigureAwait(false);
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Verifica si el role está definido en la app.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<bool> VerifyRoleAsync(string role)
        {
            var role_existing = await this._roleManager.RoleExistsAsync(role).ConfigureAwait(false);
            if(!role_existing)
            return false;
            return true;
        }

        /// <summary>
        /// Obtiene el Id del último usuario registrado en el sistema.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetIdOfLastUserAsync()
        {
           var list_users = await this._dataContext.Users.ToListAsync().ConfigureAwait(false);
           if (list_users == null)
           return -1;
           return list_users.LastOrDefault().Id;
        }

        /// <summary>
        /// Obtiene un usuario dada su cuenta de usuario.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            var user = await this._userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if(user == null)
            return null;
            return user;
        }

        /// <summary>
        /// Obtiene un Usuario dado su Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await this._userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            if (user == null)
            return null;
            return user;
        }

        /// <summary>
        /// Obtiene un usuario dado su correo electrónico.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await this._userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
            return null;
            return user;
        }

        /// <summary>
        /// Verifica si el usuario confirmó su registro.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> VerifyConfirmRegisterUserAsync(User user)
        {
            var confirm_register = await this._userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false);
            if(!confirm_register)
            return false;
            return true;
        }

        /// <summary>
        /// Verifica si el usuario confirmó su registro.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public async Task<bool> ConfirmRegisterUserAsync(User user, string Token)
        {
            //Obtiene la relación del usuario con su código de confirmación. 
            var code_user = await this._dataContext.ConfirmationRegisterForEmail
            .Where(c => c.User.Id == user.Id && c.Token == Token)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
            if (code_user == null)
            return false;
            //Confirma el correo electrónico.
            var TokenEmail = await this._userManager.GenerateEmailConfirmationTokenAsync(user)
            .ConfigureAwait(false);
            await this._userManager.ConfirmEmailAsync(user, TokenEmail).ConfigureAwait(false);
            //Elimina la relación ya que el usuario confirmó el registro.
            this._dataContext.ConfirmationRegisterForEmail.Remove(code_user);
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Verifica que la contraseña del usuario es correcta.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> VerifyPasswordUserAsync(User user, string password)
        {
            var password_correct = await _userManager.CheckPasswordAsync(user, password).ConfigureAwait(false);
            if(!password_correct)
            return false;
            return true;
        }

        /// <summary>
        /// Crea un Token Web Json para el usuario.
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
            //Agrega como reclamo Identity.Name el UserName del usuario
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            //Agrega como reclamo para el identificador de nombres el Id Guid del usuario.
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.IdForClaim.ToString()));
            //Agrega un reclamo que usa el Id del usuario
            claims.Add(new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()));
            //Agrega un reclamo que usa el Email del usuario
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
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
        /// Obtiene todos los usuarios del sistema.
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAllUserAsync()
        {
            var list_users = await this._dataContext.Users.ToListAsync().ConfigureAwait(false);
            if (list_users == null)
            return null;
            return list_users;
        }

        /// <summary>
        /// Cambia la contraseña de un usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePasswordAsync(User user, string oldPassword, string newPassword)
        {
            var identity_result = await this._userManager.ChangePasswordAsync(user, oldPassword, newPassword).ConfigureAwait(false);
            if (!identity_result.Succeeded)
            return false;
            return true;
        }

        /// <summary>
        /// Borra la imagen de perfil de un usuario determinado.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> DeleteImageProfileUserAsync(User user)
        {
            user.ImageUserProfile = null;
            var delete_image_user = await this._userManager.UpdateAsync(user).ConfigureAwait(false);
            if(delete_image_user.Succeeded)
            return true;
            return false;
        }
       
        /// <summary>
        /// Actualiza un usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<User> UpdateUserAsync(User user)
        {
            var identity_result = await this._userManager.UpdateAsync(user).ConfigureAwait(false);
            if(identity_result.Succeeded)
            return user;
            return null;
        }

        /// <summary>
        /// Envia un correo al usuario con un código para la recuperaión de su contraseña.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> SendEmailForResetPasswordUserAsync(User user)
        {
            //Verifica si hay codigos de recuperación de contraseñas del usuario almacenados anteriores. 
            var old_codes = await this._dataContext.RecoverPasswords
            .Include(c => c.User)
            .Where(c => c.User == user).ToListAsync();
            //Si hay códigos anteriores los elimina, para generar uno nuevo.
            if (old_codes != null)
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
            var send_email = this._mailService.SendMail(user.Email, "Password recovery code in the Duma app", body_message);
            if(!send_email)
            return false;
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
            return true;
        }

        /// <summary>
        /// Verifica si el usuario solicito la recuperación de su contraseña.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool VerifyUserRecoverPasswordAsync(User user)
        {
            if (user.RecoverPassword)
            return true;
            return false; 
        }

        /// <summary>
        /// Recupera la contraseña del usuario. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="Token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<bool> RecoverPasswwordUserAsync(User user, string Token, string newPassword)
        {
            //Verifica que el código de recuperación.
            var recover_password = await this._dataContext.RecoverPasswords
            .Where(c => c.User == user && c.Token == Token)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
            if (recover_password == null)
            return false;
            //Genera el Token de recuperación de contraseña.
            var token = await this._userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            //Confirma el código de recuperación
            await this._userManager.ResetPasswordAsync(user, token, newPassword).ConfigureAwait(false);
            //Elimina la relación ya que el usuario confirmo el registro.
            this._dataContext.RecoverPasswords.Remove(recover_password);
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
            //Establece la bandera de recuperación de contraseña en false;
            user.RecoverPassword = false;
            //Actualiza el usuario
            this._dataContext.Users.Update(user);
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Agrega o actualiza la imagen de perfil del usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public async Task<bool> AddOrUpdateImageProfileUserAsync(User user, IFormFile formFile)
        {
            //Nombre de la imagen
            var file = $"{Guid.NewGuid()}.jpg";
            //Ruta temporal donde la guardaremos antes de enviarla a la base de datos.
            var path = Path.Combine(Directory.GetCurrentDirectory(), file);
            //Crea el archivo de la imagen que se encuentra en memoria RAM y lo guarda en la ruta seleccionada.
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await formFile.CopyToAsync(stream).ConfigureAwait(false);
            };
            //Verifica si se guardo la imagen.
            var exiting = System.IO.File.Exists(path);
            if (!exiting)
            return false;
            //Obtiene el archivo de imagen
            var arraybyte_image = System.IO.File.ReadAllBytes(path);
            if (arraybyte_image.Length <= 0)
            return false;
            //Actualiza la imagen de perfil del usuario.
            user.ImageUserProfile = arraybyte_image;
            this._dataContext.Users.Update(user);
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
            //Elimina la imagen
            System.IO.File.Delete(path);
            return true;
        }
    }
}
