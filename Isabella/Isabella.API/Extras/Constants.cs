namespace Isabella.API.Extras
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Esta clase tiene todas las constantes del proyecto y recibe los parametros para la conexión
    /// con nuestra cuenta de Cosmos Azure DB.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Obtiene la cadena de conexión de la base de datos SQL Server
        /// desde el archivo de configuración appsetting.json
        /// </summary>
        /// <param name="configurationSection"></param>
        /// <returns></returns>
        public static string GetStringConnectionSQLServer(IConfiguration configurationSection)
        => configurationSection.GetSection("DataSource").Value;

        /// <summary>
        /// Roles del sistema.
        /// </summary>
        public static List<string> RolesOfSystem = new List<string>
        {
            new string("admin"),
        };

        /// <summary>
        /// Capacidad Máxima de la imagen de perfil de usuario.
        /// </summary>
        public static int MAX_LENTHG_IMAGE_PROFILE_USER = 100000; //100Kb

        /// <summary>
        /// Capacidad Máxima de las imagen de un producto.
        /// </summary>
        public static int MAX_LENTHG_IMAGE_PRODUCT = 100000; //100Kb

    }
}
