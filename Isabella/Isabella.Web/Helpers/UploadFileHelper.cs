namespace Isabella.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Servicio para el manejo de archivos de subida.
    /// </summary>
    public class UploadFileHelper
    {
        /// <summary>
        /// Guarda un archivo que envio el usuario en el servidor.
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool UploadFile(MemoryStream memoryStream, string folder, string fileName)
        {
            try
            {
                memoryStream.Position = 0;
                var path = Path.Combine(Directory.GetCurrentDirectory(), folder, fileName);
                File.WriteAllBytes(path, memoryStream.ToArray());
                return true; 
            }
            catch
            {
               return false;
            }
        }
    }
}
