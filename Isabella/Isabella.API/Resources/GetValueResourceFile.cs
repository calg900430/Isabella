namespace Isabella.API.Resources
{
    using System;
    using System.IO;
    using System.Resources;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Collections.Generic;
    using System.Collections;
    using System.Threading.Tasks;

    /// <summary>
    /// Obtiene los valores de un archivo de recursos.
    /// </summary>
    public static class GetValueResourceFile
    {
        /// <summary>
        /// Representa los Key de los recursos.
        /// </summary>
        public enum KeyResource
        {
            //Recursos String
            SuccessOk = 1,
            Exception = 2,
            EntityIsNull = 3,
            UserNotFound = 4,
            UserAllNotFound = 5,
            UserBadUserName = 6,
            CantIsNegative = 7,
            ProductNotNewImage = 8,
            CategoryNotFound = 9,
            ProductNotFound = 10,
            ProductAllNotFound = 11,
            ImageNotExist = 12,
            ImageProductNotValide = 13,
            ImageUserNotValide = 14,
            CategoryExist = 15,
            CategoryNotAllFound = 16,
            ProductNotNew = 17,
            SubCategoryExist = 18,
            SubCategoryNotFound = 19,
            SubCategoryNotAllFound = 20,
            ImageAggregateNotValide = 21,
            AggregateNotFound = 22,
            AggregateAllNotFound = 23,
            AggregateNotNew = 24,
            BadRole = 25,
            BadEmail = 26,
            ErrorDataBaseUserIdentity = 27,
            UserConfirmRegister = 28,
            TokeConfirmRegisterBad = 29,
            RequiredEmailOfUser = 30,
            NotConfirmRegister = 31,
            VerifyPasswordAndEmail = 32,
            ErrorGenerateToken = 33,
            LoginUserSuccess = 34,
            UserNotNew = 35,
            EmailNotSend = 36,
            EmailRegisterConfirmation = 37,
            ImageErrorValue = 38,
            PasswordNotCorrect = 39,
            CodeRecoverPassword = 40,
            ErrorGetCredentialsUser = 41,
            ProductExist = 42,
            AggregateExist = 43,
            ImagesNoExist = 44,
            NotCodeIdentification = 45,
            CarShopNotProducts = 46,
            ProductNotIsAvailable = 47,

            /// <summary>
            /// No se encuentra el archivo incrustado.
            /// </summary>
            NotFoundFileEmbedded = 48,

            /// <summary>
            /// 
            /// </summary>
            NotUsersClient = 49,

            /// <summary>
            /// 
            /// </summary>
            NotUsersOwner = 50,

            /// <summary>
            /// 
            /// </summary>
            IsNotRoleOfUser = 51,

            /// <summary>
            /// 
            /// </summary>
            IsNotRemoveRoleOfUser = 52,

            /// <summary>
            /// 
            /// </summary>
            IsUserHaveRole = 53,

            /// <summary>
            /// 
            /// </summary>
            IsNotAssignRoleOfUser = 54,

            /// <summary>
            /// 
            /// </summary>
            ProductAllNotIsAvailable = 55,

            /// <summary>
            /// 
            /// </summary>
            ProductsOfCategoryNotAvailable = 56,

            /// <summary>
            /// 
            /// </summary>
            ProductsIsAvailableOfCategoryNotAvailable = 57,

            /// <summary>
            /// 
            /// </summary>
            ExceptionDeleteEntity = 58,

            /// <summary>
            /// 
            /// </summary>
            AggregateNotIsAvailable = 59,

            /// <summary>
            /// 
            /// </summary>
            AggregateAllNotIsAvailable = 60,

            /// <summary>
            /// El producto no soporta agregados.
            /// </summary>
            ProductNotSupportAggregate = 61,

            /// <summary>
            /// 
            /// </summary>
            FormatAggregateNotSupport = 62,

            /// <summary>
            /// La subcategoria no pertence al producto.
            /// </summary>
            SubCategoryNotIsProduct = 63,
        }

        /// <summary>
        /// Recursos de Imagenes de la aplicación
        /// </summary>
        public enum KeyResourceImage
        {
            //Recursos de Imagenes
            ImageAceitunas = 1,
            ImageAtun = 2,
            ImageSetas = 3,
            ImageCamarones = 4,
            ImageChorizo = 5,
            ImageJamon = 6,
            ImageQuesoGouda = 7,
            ImageBistecCerdo1 = 8,
            ImageBistecCerdo2 = 9,
            ImageCamaronGrille1 = 11,
            ImageCamaronGrille2 = 11,
            ImageCamaronGrille3 = 12,
            ImageCocoGlaset = 13,
            ImageEnsaldadaFria1 = 14,
            ImageEnsaldadaFria2 = 15,
            ImageEspaguettiJamon1 = 16,
            ImageEspaguettiJamon2 = 17,
            ImageMaltaHolland = 18,
            ImagePizzaCamarones1 = 19,
            ImagePizzaCamarones2 = 20,
            ImageVinoBlanco = 21,
            ImageQuesoBlanco = 22,
        }

        /// <summary>
        /// Obtiene el valor de un recurso desde el archivo de recurso dado un key dado.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueResourceString(KeyResource key)
        {
            Type getResource = typeof(GetValueResourceFile);
            var assembly = getResource.Assembly;
            var name_file_resources = "ResourceFile.resources";
            //Obtiene la referencia al archivo de recurso, esto lo hago así, porque el archivo de recursos
            //está en el directorio donde está la clase GetValueResourceFile.
            string resourceName = $"{getResource.Namespace}.{name_file_resources}";
            string valueResource = string.Empty;
            using (ResourceReader resourceReader = new ResourceReader(assembly.GetManifestResourceStream(resourceName)))
            {
                try
                {
                    resourceReader.GetResourceData(key.ToString(), out string dataType, out byte[] data);
                    switch (dataType)
                    {
                        //Recurso String(Message)
                        case "ResourceTypeCode.String":
                        using (var reader = new BinaryReader(new MemoryStream(data)))
                        {
                           valueResource = reader.ReadString();
                           reader.Close();
                           reader.Dispose();
                        }
                        break;

                        default:
                        valueResource = "El mensaje no está disponible en el archivo de recursos.";
                        break;
                    }
                    resourceReader.Dispose();
                    resourceReader.Close();
                }
                catch
                {
                    return "Key desconocida en el archivo de recursos.";
                }

            }
            return valueResource;
        }

        /// <summary>
        /// Obtiene una imagen desde el archivo de recursos.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] GetValueResourceImage(KeyResourceImage key)
        => GetImage(key.ToString());

        /// <summary>
        /// Obtiene una imagen desde el archivo de recursos dado el key como string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] GetValueResourceImage(string key)
        => GetImage(key);

        /// <summary>
        /// Obtiene una imagen que se encuenta en el archivo de recursos.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static byte[] GetImage(string key)
        {
            Type getResource = typeof(GetValueResourceFile);
            var assembly = getResource.Assembly;
            var name_file_resources = "ResourceFile.resources";
            string resourceName = $"{getResource.Namespace}.{name_file_resources}";
            byte[] valueResource = null;
            using (ResourceReader resourceReader = new ResourceReader(assembly.GetManifestResourceStream(resourceName)))
            {
                try
                {
                    resourceReader.GetResourceData(key, out string dataType, out byte[] data);
                    switch (dataType)
                    {
                        case "ResourceTypeCode.Stream":
                        const int OFFSET = 4;
                        int size = BitConverter.ToInt32(data, 0);
                        using (Bitmap image = new Bitmap(new MemoryStream(data, OFFSET, size)))
                        {
                           //Convertir em mapa de bit a un arreglo de bytes
                           image.Save($"{Directory.GetCurrentDirectory()}\\temp.jpg");
                           image.Dispose();
                        }
                        valueResource = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}\\temp.jpg");
                        //Elimina la imagen anterior
                        File.Delete($"{Directory.GetCurrentDirectory()}\\temp.jpg");
                        break;

                        default:
                        valueResource = null;
                        break;
                    }
                    resourceReader.Dispose();
                    resourceReader.Close();
                }
                catch
                {
                    return null;
                }
            }
            return valueResource;
        }
    }     
}
