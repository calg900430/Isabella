namespace Isabella.Web.Resources
{
    using System;
    using System.IO;


    /// <summary>
    /// Utilidad para leeer scripts SQL.
    /// </summary>
    public class UtilityForReadFileEmbedded
    {
        /// <summary>
        /// Lee un archivo que este incrustado en el emsamblado.
        /// </summary>
        /// <param name="getResource">Tipo de recurso</param>
        /// <param name="filename">Nombre del archivo</param>
        /// <returns></returns>
        public static string ReadFileEmmbeded(Type getResource, string filename)
        {
            //Define que es inscrustado(ensamblado)
            var assembly = getResource.Assembly;
            //Obtiene la referencia al recurso
            string resourceName = $"{getResource.Namespace}.{filename}";
            using(Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                   throw new FileNotFoundException(GetValueResourceFile
                   .GetValueResourceString(GetValueResourceFile.KeyResource.NotFoundFileEmbedded));
                }
                using (var reader = new StreamReader(stream))
                {
                    //Lee el archivo SQL
                    string content = reader.ReadToEnd();
                    return content;
                }
            }
        }
    }
}
