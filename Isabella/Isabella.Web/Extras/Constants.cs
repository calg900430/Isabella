namespace Isabella.Web.Extras
{
    using Microsoft.Extensions.Configuration;
  
    /// <summary>
    /// Esta clase tiene todas las constantes del proyecto y recibe los parametros para la conexión
    /// con nuestra cuenta de Cosmos Azure DB.
    /// </summary>
    public static class Constants
    {           
        /// <summary>
        /// Método que obtiene el accountkey, el accountEndpoint 
        /// y el nombre de la base de datos del archivo de configuración appsetting.json para
        /// darle los datos de la cuenta de Azure Cosmos
        /// </summary>
        /// <param name="configurationSection"></param>
        /// <returns></returns>
        public static string[] GetStringConnectionCosmos(IConfiguration configurationSection)
        {
            string[] cosmos = new string[3];
            cosmos = new string[3]
            {
               configurationSection.GetSection("Account").Value,
               configurationSection.GetSection("Key").Value,
               configurationSection.GetSection("DatabaseName").Value
            };
            return cosmos;            
        }

        /// <summary>
        /// Obtiene la cadena de conexión de la base de datos SQL Server
        /// desde el archivo de configuración appsetting.json
        /// </summary>
        /// <param name="configurationSection"></param>
        /// <returns></returns>
        public static string GetStringConnectionSQLServer(IConfiguration configurationSection)
        => configurationSection.GetSection("DataSource").Value;
        
    }
}
