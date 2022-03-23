namespace Isabella.Web.Helpers
{
    using System;
    using System.Text;
    using System.IO;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.IdentityModel.Tokens.Jwt;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Http;

    using Data;
    using Models;
    using Models.Entities;
    using Isabella.Common;
    using RepositoryHelpers;

    /// <summary>
    /// Servicio para la entidad que representa los usuarios.
    /// </summary>
    public class IUserHelper : IUserRepositoryHelper
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name=""></param>
        /// <param name="signInManager"></param>
        /// <param name="configuration"></param>
        public IUserHelper(DataContext dataContext, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager
        ,SignInManager<User> signInManager, IConfiguration configuration)
        {
            this._dataContext = dataContext;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
            this._configuration = configuration;
        }


        /// <summary>
        /// Login Web
        /// </summary>
        /// <returns></returns>
        public async Task<SignInResult> SignInAsync(User user, string password, bool remember)
        {
            return await _signInManager.
            PasswordSignInAsync(user.UserName, password, remember,false)
            .ConfigureAwait(false);
        }


        /// <summary>
        /// Logout Web
        /// </summary>
        /// <returns></returns>
        public async Task SignOutAsync()
        {
            await this._signInManager.SignOutAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Agrega un usuario con contraseña definida.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User> AddUserAsync(User user, string password)
        {
            if (user == null)
                throw new NullReferenceException();
            if (password == string.Empty || password == null)
                throw new NotPasswordIsDefinedException("La contraseña no puede estar en blanco.");
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
            if (user == null)
            throw new NullReferenceException();
            //Guarda al usuario en la base de datos.
            var user_identity = await this._userManager.CreateAsync(user).ConfigureAwait(false);
            //No se agregó el usuario
            if (user_identity.Succeeded == false)
                return null;
            return user;
        }

        /// <summary>
        /// Elimina un usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUserAsync(User user)
        {
            if (user == null)
            throw new NullReferenceException();
            //Elimina el usuario.
            var user_identity = await this._userManager.DeleteAsync(user)
            .ConfigureAwait(false);
            //No se agregó el usuario
            if (user_identity.Succeeded)
            return true;
            else
            return false; 
        }

        
        /// <summary>
        /// Agrega un role determinado a un usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<bool> AddRoleForUserAsync(User user, string role)
        {
            if (user == null)
                throw new NullReferenceException();
            if (role == string.Empty || role == null)
                throw new RoleWrongException("Debe especificar el role que desea agregarle al usuario.");
            //Agrega al nuevo usuario el rol.
            var identity_result = await this._userManager.AddToRoleAsync(user, role).ConfigureAwait(false);
            if (!identity_result.Succeeded)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Verifica si un usuario tiene un role asignado
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<bool> VerifyRoleInUserAsync(User user, string role)
        {
            var role_user = await this._userManager
            .IsInRoleAsync(user, role)
            .ConfigureAwait(false);
            if (role_user)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Obtiene todos los roles que posee un usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<List<string>> GetAllRoleOfUserAsync(User user)
        {
            var all_role = await this._userManager.GetRolesAsync(user).ConfigureAwait(false);
            if (all_role == null)
                return null;
            else
                return all_role.ToList();
        }

        /// <summary>
        /// Elimina un role especifico de un usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<bool> RemoveRoleOfUserAsync(User user, string role)
        {
            var remove_role = await this._userManager.RemoveFromRoleAsync(user, role).ConfigureAwait(false);
            if (remove_role.Succeeded)
                return true;
            else
                return false;
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
        /// Genera el Token del usuario para la confirmación del registro.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> GenerateTokenForConfirmRegisterAsync(User user)
        {
            if (user == null)
                throw new NullReferenceException();
            var token = await this._userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
            return token;
        }

        /// <summary>
        /// Verifica si el role está definido en la app.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<bool> VerifyRoleAsync(string role)
        {
            var role_existing = await this._roleManager.RoleExistsAsync(role).ConfigureAwait(false);
            if (!role_existing)
                return false;
            return true;
        }

        /// <summary>
        /// Verifica si el role está definido en la app.
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public async Task<string> VerifyRoleAsync(int RoleId)
        {
            var role_existing = await this._roleManager.FindByIdAsync(RoleId.ToString()).ConfigureAwait(false);
            if (role_existing == null)
                return null;
            return role_existing.Name;
        }

        /// <summary>
        /// Obtiene el Id del último usuario registrado en el sistema.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetIdOfLastUserAsync()
        {
            var last_user = await this._dataContext.Users
            .OrderBy(c => c.Id).LastOrDefaultAsync()
            .ConfigureAwait(false);
            if (last_user == null)
                return -1;
            return last_user.Id;
        }

        /// <summary>
        /// Obtiene un usuario dada su cuenta de usuario.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            var user = await this._userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user == null)
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
            if (user == null)
                throw new NullReferenceException();
            var confirm_register = await this._userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false);
            if (confirm_register == false)
                return false;
            else
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
            if (user == null)
                throw new NullReferenceException();
            //Confirma el registro del usuario.
            var identity = await this._userManager.ConfirmEmailAsync(user, Token).ConfigureAwait(false);
            if (identity.Succeeded == true)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Verifica que la contraseña del usuario es correcta.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> VerifyPasswordUserAsync(User user, string password)
        {
            if (user == null)
                throw new NullReferenceException();
            var password_correct = await _userManager.CheckPasswordAsync(user, password).ConfigureAwait(false);
            if (!password_correct)
                return false;
            return true;
        }

        /// <summary>
        /// Crea un Token Web Json para el usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(DateTime?, string)> CreateTokenAsync(User user)
        {
            if (user == null)
            throw new NullReferenceException();
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
            return (data_expires, token);
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
        /// Obtiene todos los usuarios del sistema que tienen un role determinado.
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAllUserWithRoleAsync(int RoleId)
        {
            var role_existing = await this._roleManager.FindByIdAsync(RoleId.ToString()).ConfigureAwait(false);
            if (role_existing == null)
                return null;
            var list_users = await this._userManager.GetUsersInRoleAsync(role_existing.Name).ConfigureAwait(false);
            if (list_users == null)
                return null;
            return list_users.ToList();
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
            if (user == null)
                throw new NullReferenceException();
            var identity_result = await this._userManager.ChangePasswordAsync(user, oldPassword, newPassword).ConfigureAwait(false);
            if (!identity_result.Succeeded)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Borra la imagen de perfil de un usuario determinado.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> DeleteImageProfileUserAsync(User user)
        {
            if (user == null)
                throw new NullReferenceException();
            user.ImageUserProfile = null;
            var delete_image_user = await this._userManager.UpdateAsync(user).ConfigureAwait(false);
            if (delete_image_user.Succeeded)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Actualiza un usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<User> UpdateUserAsync(User user)
        {
            if (user == null)
                throw new NullReferenceException();
            var identity_result = await this._userManager.UpdateAsync(user).ConfigureAwait(false);
            if (identity_result.Succeeded)
                return user;
            return null;
        }

        /// <summary>
        /// Envia un correo al usuario con un código para la recuperaión de su contraseña.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> GenerateTokenForRecoverPasswordAsync(User user)
        {
            if (user == null)
                throw new NullReferenceException();
            var token = await this._userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            return token;
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
            //Confirma el código de recuperación
            var identity = await this._userManager.ResetPasswordAsync(user, Token, newPassword).ConfigureAwait(false);
            if (identity.Succeeded == true)
                return true;
            else
                return false;
        }
    }

    /*Excepciones para UserHelper*/
    class NotPasswordIsDefinedException : Exception
    {
        public NotPasswordIsDefinedException(string message) : base(message)
        {

        }
    }
    class RoleWrongException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message"></param>
        public RoleWrongException(string message) : base(message)
        {

        }
    }
}
