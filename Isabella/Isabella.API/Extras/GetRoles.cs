namespace Isabella.API.Extras
{
    using System.Collections.Generic;
    using Common.Extras;
   
    /// <summary>
    /// Obtiene el tipo de role, según su nombre.
    /// </summary>
    public static class ManagerRolesForUsers
    {
        /// <summary>
        /// Devuelve una lista Enum que identifica los roles del usuario.
        /// </summary>
        /// <param name="names_roles"></param>
        /// <returns></returns>
        public static List<EnumRoles> GetRoles(IList<string> names_roles)
        {
            if(names_roles == null)
            return null;
            List<EnumRoles> GetRoles = new List<EnumRoles>();
            foreach(string name in names_roles)
            {
                switch (name)
                {
                    case "admin":
                    GetRoles.Add(EnumRoles.admin);
                    continue;
				
                    case "client":
                    GetRoles.Add(EnumRoles.client);
                    continue;

                    default:
                    return null;
                }
            }
            return GetRoles;
        }
    }
}
