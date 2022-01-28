namespace Isabella.Web.Resources
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

            /// <summary>
            /// 
            /// </summary>
            ImageUserNotValide = 14,

            /// <summary>
            /// 
            /// </summary>
            CategoryExist = 15,

            /// <summary>
            /// 
            /// </summary>
            CategoryNotAllFound = 16,

            /// <summary>
            /// 
            /// </summary>
            ProductNotNew = 17,

            /// <summary>
            /// 
            /// </summary>
            SubCategoryExist = 18,

            /// <summary>
            /// 
            /// </summary>
            SubCategoryNotFound = 19,

            /// <summary>
            /// 
            /// </summary>
            SubCategoryNotAllFound = 20,

            /// <summary>
            /// 
            /// </summary>
            ImageAggregateNotValide = 21,

            /// <summary>
            /// 
            /// </summary>
            AggregateNotFound = 22,

            /// <summary>
            /// 
            /// </summary>
            AggregateAllNotFound = 23,

            /// <summary>
            /// 
            /// </summary>
            AggregateNotNew = 24,

            /// <summary>
            /// 
            /// </summary>
            BadRole = 25,

            /// <summary>
            /// 
            /// </summary>
            BadEmail = 26,

            /// <summary>
            /// 
            /// </summary>
            ErrorDataBaseUserIdentity = 27,

            /// <summary>
            /// 
            /// </summary>
            UserConfirmRegister = 28,

            /// <summary>
            /// 
            /// </summary>
            TokeConfirmRegisterBad = 29,

            /// <summary>
            /// 
            /// </summary>
            RequiredEmailOfUser = 30,

            /// <summary>
            /// 
            /// </summary>
            NotConfirmRegister = 31,

            /// <summary>
            /// 
            /// </summary>
            VerifyPasswordAndEmail = 32,

            /// <summary>
            /// 
            /// </summary>
            ErrorGenerateToken = 33,

            /// <summary>
            /// 
            /// </summary>
            LoginUserSuccess = 34,

            /// <summary>
            /// 
            /// </summary>
            UserNotNew = 35,

            /// <summary>
            /// 
            /// </summary>
            EmailNotSend = 36,

            /// <summary>
            /// 
            /// </summary>
            EmailRegisterConfirmation = 37,

            /// <summary>
            /// 
            /// </summary>
            ImageErrorValue = 38,

            /// <summary>
            /// 
            /// </summary>
            PasswordNotCorrect = 39,

            /// <summary>
            /// 
            /// </summary>
            CodeRecoverPassword = 40,

            /// <summary>
            /// 
            /// </summary>
            ErrorGetCredentialsUser = 41,

            /// <summary>
            /// 
            /// </summary>
            ProductExist = 42,

            /// <summary>
            /// 
            /// </summary>
            AggregateExist = 43,

            /// <summary>
            /// 
            /// </summary>
            ImagesNoExist = 44,

            /// <summary>
            /// 
            /// </summary>
            NotCodeIdentification = 45,

            /// <summary>
            /// 
            /// </summary>
            CarShopNotProducts = 46,

            /// <summary>
            /// 
            /// </summary>
            ProductNotIsAvailable = 47,

            /// <summary>
            /// No se encuentra el archivo incrustado.
            /// </summary>
            NotFoundFileEmbedded = 48,

            /// <summary>
            /// 
            /// </summary>
            UserAllNotFoundWithRole = 49,

            /// <summary>
            /// 
            /// </summary>
            ProductNotHaveAggregate = 50,

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

            /// <summary>
            /// El producto no existe en el carrito de compras del usuario.
            /// </summary>
            ProductNotExistInCarShop = 64,

            /// <summary>
            /// El producto no se encuentra en el carrito o ya tiene asignada esa subcategoria.
            /// </summary>
            ProductInCartHaveSubCategory = 65,

            /// <summary>
            /// 
            /// </summary>
            ProductInCartNotHaveSubCategory = 66,

            /// <summary>
            /// 
            /// </summary>
            ProductCombinedNotHaveSubCategory = 67,

            /// <summary>
            /// 
            /// </summary>
            SubCategoryNotIsAvailable = 68,

            /// <summary>
            /// 
            /// </summary>
            UserNotAnyOrder = 70,

            /// <summary>
            /// No hay definidos usuarios admins para que reciban las notificaciones
            /// </summary>
            NotUserAdminsNotifications = 71,

            /// <summary>
            /// El usuario no está definido como un usuario que puede recibir notificaciones.
            /// </summary>
            UserAdminNotExistForNotifications = 72,

            /// <summary>
            /// El usuario está definido como un usuario que puede recibir notificaciones.
            /// </summary>
            UserAdminExistForNotifications = 73,

            /// <summary>
            /// El restaurante está cerrado.
            /// </summary>
            RestaurantIsClose = 74,

            /// <summary>
            /// El restaurante está cerrado.
            /// </summary>
            RestaurantIsOpen = 75,


            /// <summary>
            /// Error al obtener los datos del resturante.
            /// </summary>
            RestaurantError = 76,

            /// <summary>
            /// Cierra el restaurante.
            /// </summary>
            RestaurantClose = 77,

            /// <summary>
            /// Abre el restaurante
            /// </summary>
            RestaurantOpen = 78,

            /// <summary>
            /// Borrar la entidad producto
            /// </summary>
            DeleteEntityProduct = 79
        }

        /// <summary>
        /// Recursos de Imagenes de la aplicación
        /// </summary>
        public enum KeyResourceImage
        {
            //Recursos de Imagenes

            /// <summary>
            /// 
            /// </summary>
            ImageAceitunas = 1,

            /// <summary>
            /// 
            /// </summary>
            ImageAtun = 2,

            /// <summary>
            /// 
            /// </summary>
            ImageSetas = 3,

            /// <summary>
            /// 
            /// </summary>
            ImageCamarones = 4,

            /// <summary>
            /// 
            /// </summary>
            ImageChorizo = 5,

            /// <summary>
            /// 
            /// </summary>
            ImageJamon = 6,
            ImageQuesoGouda = 7,
            ImageBistecCerdo = 8,
            ImageCamaronGrille = 9,
            ImageCocoGlaset = 10,
            ImageEnsaldadaFria = 11,
            ImageEspaguettiJamon = 12,
            ImageMaltaHolland = 13,
            ImagePizzaCamarones = 14,
            ImageVinoBlanco = 15,
            ImageQuesoBlanco = 16,
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
            var name_file_resources = "ResourcesFile.resources";
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
            var name_file_resources = "ResourcesFile.resources";
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
