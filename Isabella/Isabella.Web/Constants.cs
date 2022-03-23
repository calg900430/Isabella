namespace Isabella.Web
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Isabella.Web.Data;
    using Isabella.Web.Helpers;
    using Isabella.Web.Models.Entities;
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
        /// Capacidad máxima de la imagen de perfil de usuario.
        /// </summary>
        public static int MAX_LENTHG_IMAGE_PROFILE_USER = 100000; //100Kb

        /// <summary>
        /// Capacidad máxima de las imagen de un producto.
        /// </summary>
        public static int MAX_LENTHG_IMAGE_PRODUCT = 100000; //100Kb

        /// <summary>
        /// Capacidad máxima de las imagen de un agregado.
        /// </summary>
        public static int MAX_LENTHG_IMAGE_AGGREGATE = 100000; //100Kb

    }
}
