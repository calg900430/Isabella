namespace Isabella.App.Services.ServicesMock
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Isabella.App.Services.ServicesMock.RepositorysMock;
    using Isabella.Common.Dtos.Users;


    public class UserMockService : IUserMockRepository
    {
        public GetDataUserForLoginDto LoginMock(string email, string password)
        {
            var login = new GetDataUserForLoginDto
            {
                DateExpirationToken = DateTime.UtcNow,
                Address = "Cuartel / 2 y 3 sur",
                Email = email,
                FirstName = "Carlos Antonio",
                LastName = "Lamoth Guilarte",
                PhoneNumber = "+5354069957",
                Id = 1,
                ImageUserProfile = null,
                RolesOfUsers = new List<string> { "admin", "owner" },
                Token = "eqwrk9023940234kl2ewfkamdi1i9049209534i21jo3nfsg.df,gheld34i01ADGKe00320",
            };
            return login;
        }
    }
}
