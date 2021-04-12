namespace Isabella.App.Services.ServicesMock.RepositorysMock
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Isabella.Common.Dtos.Users;

    /// <summary>
    /// ILocginMock
    /// </summary>
    public interface IUserMockRepository
    {
        GetDataUserForLoginDto LoginMock(string email, string password);
    }
}
