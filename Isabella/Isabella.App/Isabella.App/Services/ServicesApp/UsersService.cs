namespace Isabella.App.Services.ServicesApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using Isabella.Common;
    using Isabella.Common.Dtos.Users;
    using Isabella.Common.RepositorysDtos;

    class UsersService : IUserRepositoryDto
    {
        /// <summary>
        /// Login del usuario.
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public Task<ServiceResponse<GetDataUserForLoginDto>> LoginUserAsync(LoginUserDto loginUser)
        {
            throw new NotImplementedException();
        }


        public Task<ServiceResponse<bool>> AddUserAdminForNotifications(int UserId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> AddUserAsync(RegisterUserDto newuser)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> AssigningRoleToUserAsync(int UserId, int RoleId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> BannerUserAsync(int UserId, bool banner)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> ConfirmEmailUserAsync(string Id, string Token)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> DeleteImageProfileUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetUserAllDataForOnlyAdmin>> GetAllDataOfOnlyUserAsync(int UserId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<GetUserAllDataForOnlyAdmin>>> GetAllDataOfUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<string>>> GetAllRoleOfUser(int UserId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<GetUserDto>>> GetAllUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<GetUserDto>>> GetAllUserWithRoleAsync(int RoleId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<GetUserDto>>> GetCantUserAsync(int UserId, int CantUsers)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<int>> GetIdOfLastUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetUserDto>> GetUserByEmailAsync(string Email)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetUserDto>> GetUserByIdAsync(int UserId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetUserDto>> GetUserByUserNameAsync(string UserName)
        {
            throw new NotImplementedException();
        }

        public Task LoginExternProviderForAsync(string scheme)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> RecoverPasswwordUserAsync(string Id, string Token, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetRegisterUserModeFastDto>> RegisterUserModeFastAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> RemoveRoleInUserAsync(int UserId, int RoleId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> RemoveUserAdminForNotifications(int UserId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> ResetPasswordUserAsync(ResetPasswordDto resetPassword)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> SendToNewCodeConfirmationEmailAsync(SendToNewCodeConfirmationRegisterDto sendToNewCodeConfirmationRegister)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> UpdatePasswordAsync(ChangePasswordUserDto changePassword)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetUserDto>> UpdateUserAsync(UpdateUserDto updateUser)
        {
            throw new NotImplementedException();
        }
    }
}
