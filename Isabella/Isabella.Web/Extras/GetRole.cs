namespace Isabella.Web.Extras
{
    using Isabella.Common.Extras;
    using System.Collections.Generic;
    
    /// <summary>
    /// Obtiene el tipo de role, según su nombre.
    /// </summary>
    public static class GetRole
    {
        /// <summary>
        /// Devuelve una lista Enum que identifica los roles del usuario
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

                    case "owner":
                    GetRoles.Add(EnumRoles.owner);
                    continue;

                    case "client":
                    GetRoles.Add(EnumRoles.client);
                    continue;

                    default:
                    GetRoles.Add(EnumRoles.not_defined);
                    continue;
                }
            }
            return GetRoles;
        }
    }
}
