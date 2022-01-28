namespace Isabella.Web.Resources
{
    using System.IO;
    using System.Drawing;
    using System.Resources;
    using System.Threading.Tasks;
    using System.Drawing.Imaging;

    /// <summary>
    /// Genera el archivo de recurso.
    /// </summary>
    public static class CreateResourcesFile
    {
        
        /// <summary>
        /// Genera el archivo de recursos.
        /// </summary>
        public static void GenerateResourceFileAsync(string path)
        {
            using (ResourceWriter rw = new ResourceWriter(path))
            {
                //Agrega los mensajes de la aplicación
                #region Mensajes de la aplicación
                rw.AddResource("RestaurantClose", "El restaurante se ha cerrado.");
                rw.AddResource("RestaurantOpen", "El restaurante se ha abierto.");
                rw.AddResource("RestaurantError", "Error al obtener los datos del restaurante.");
                rw.AddResource("RestaurantIsClose", "El restaurante en estos momentos se encuentra cerrado.");
                rw.AddResource("RestaurantIsOpen", "El restaurante en estos momentos se encuentra abierto.");
                rw.AddResource("UserAdminExistForNotifications", "El usuario ya está definido como un usuario que puede recibir notificaciones.");
                rw.AddResource("UserAdminNotExistForNotifications", "El usuario no está definido como un usuario que puede recibir notificaciones.");
                rw.AddResource("NotUserAdminsNotifications", "No hay definidos usuarios admins para recibir las notificaciones..");
                rw.AddResource("UserNotAnyOrder", "El usuario no tiene ordenes disponibles.");
                rw.AddResource("ExceptionDeleteEntity", "Error al borrar la entidad y sus relaciones.");
                rw.AddResource("DeleteEntityProduct", "El producto ya ha sido comprado o algún usuario lo ha calificado.No se puede eliminar para no afectar el historial de la base de datos.");
                rw.AddResource("SuccessOk", "Se ha ejecutado la operación correctamente.");
                rw.AddResource("Exception", "Se ha generado un error en la aplicación");
                rw.AddResource("UserNotFound", "El usuario no está registrado en la aplicación.");
                rw.AddResource("UserAllNotFound", "No hay usuarios registrados en la aplicación.");
                rw.AddResource("UserAllNotFoundWithRole", "No hay usuarios registrados en la aplicación con ese rol.");
                rw.AddResource("UserBadUserName", "La cuenta de usuario seleccionada está en uso.Seleccione otra.");
                rw.AddResource("EntityIsNull", "La entidad pasada como parametro es nula.");
                rw.AddResource("CantIsNegative", "El valor pasado como cantidad es igual o menor que 0.");
                rw.AddResource("ProductNotNewImage", "No se han agregado nuevas imagenes al producto.");
                rw.AddResource("CategoryNotFound", "La categoria no existe.");
                rw.AddResource("ProductNotFound", "El producto no existe.");
                rw.AddResource("ProductNotIsAvailable", "El producto no existe o no está disponible para la venta en estos momentos.");
                rw.AddResource("ProductsOfCategoryNotAvailable", "No hay productos de la categoria especificada.");
                rw.AddResource("ProductsIsAvailableOfCategoryNotAvailable", "No hay productos disponibles de la categoria especificada.");
                rw.AddResource("ProductAllNotIsAvailable", "En estos momentos no hay productos disponibles para venta.");
                rw.AddResource("ProductAllNotFound", "No hay productos en la base de datos.");
                rw.AddResource("ImageNotExist", "La imagen no existe o no pertenece a la entidad seleccionada.");
                rw.AddResource("ImageProductNotValide",
                $"La imagen de un producto no puede ser mayor de {Constants.MAX_LENTHG_IMAGE_PRODUCT/1000} KB.");
                rw.AddResource("ImageUserNotValide",
                $"La imagen de perfil de un usuario no puede ser mayor de {Constants.MAX_LENTHG_IMAGE_PROFILE_USER/1000} KB.");
                rw.AddResource("CategoryExist", "La categoria ya existe.Seleccione otro nombre.");
                rw.AddResource("CategoryNotAllFound", "No hay categorias disponibles.");
                rw.AddResource("ProductNotNew", "No se han agregado nuevos productos.");
                rw.AddResource("SubCategoryNotIsProduct", "La subcategoria no existe o no pertenece a este producto.");
                rw.AddResource("ProductCombinedNotHaveSubCategory", "El producto combinado no tiene ninguna subcategoria.");
                rw.AddResource("SubCategoryExist", "La subcategoria ya existe.Seleccione otro nombre.");
                rw.AddResource("SubCategoryNotFound", "La subcategoria no existe.");
                rw.AddResource("SubCategoryNotIsAvailable", "La subcategoria no existe o no está disponible.");
                rw.AddResource("SubCategoryNotAllFound", "No hay subcategorias disponibles.");
                rw.AddResource("ImageAggregateNotValide",
                $"La imagen de un agregado no puede ser mayor de {Constants.MAX_LENTHG_IMAGE_AGGREGATE} bytes.");
                rw.AddResource("FormatAggregateNotSupport", "El Id del agregado debe ser un número entero.");
                rw.AddResource("ProductInCartHaveSubCategory", "El producto no se encuentra en el carrito o ya tiene asignada la subcategoria.");
                rw.AddResource("ProductInCartNotHaveSubCategory", "El producto no se encuentra en el carrito o no tiene asignada la subcategoria.");
                rw.AddResource("ProductNotHaveAggregate", "El producto no existe o no tiene el agregado.");
                rw.AddResource("AggregateNotFound", "El agregado no existe.");
                rw.AddResource("AggregateAllNotFound", "No hay agregados disponibles.");
                rw.AddResource("AggregateNotNew", "No se han añadido nuevos agregados.");
                rw.AddResource("AggregateNotIsAvailable", "El agregado no existe o no está disponible en estos momentos.");
                rw.AddResource("AggregateAllNotIsAvailable", "En estos momentos no hay agregados disponibles.");
                rw.AddResource("BadRole", "El role seleccionado no es válido.");
                rw.AddResource("BadEmail", "El correo electrónico seleccionado está en uso.Seleccione otro.");
                rw.AddResource("ErrorDataBaseUserIdentity", "Error de base de datos.No se pudo realizar la operación sobre la entidad usuario.");
                rw.AddResource("UserConfirmRegister", "El usuario ya ha confirmado el registro en la aplicación.");
                rw.AddResource("TokeConfirmRegisterBad", "El token de confirmación de registro es incorrecto.");
                rw.AddResource("RequiredEmailOfUser", "Debe especificar el email del usuario para poder loguearse en la aplicación.");
                rw.AddResource("NotConfirmRegister", "El usuario no ha confirmado el registro en la aplicación.");
                rw.AddResource("VerifyPasswordAndEmail", "Verifique la contraseña y el email.");
                rw.AddResource("ErrorGenerateToken", "Error al generar el token del usuario.");
                rw.AddResource("LoginUserSuccess", "El usuario ha iniciado sesión en el sistema." +
                    "Se le ha enviado el Token al usuario y el tiempo de expiración del mismo.");
                rw.AddResource("EmailNotSend", "No se envió el email, imposible de conectar con el servidor SMTP.");
                rw.AddResource("UserNotNew", "No se han agregado nuevos usuarios al sistema.");
                rw.AddResource("EmailRegisterConfirmation", "Se la enviado un correo electrónico con los detalles " +
                               "para finalizar el registro en la aplicación.");
                rw.AddResource("ImageErrorValue", "Imagen no valida.");
                rw.AddResource("PasswordNotCorrect", "La contraseña es incorrecta.");
                rw.AddResource("CodeRecoverPassword", "Se le ha enviado un correo con los detalles para la recuperación de la contraseña.");
                rw.AddResource("ErrorGetCredentialsUser", "Error al obtener los credenciales del usuario del contexto Http.");
                rw.AddResource("ProductExist", "El producto ya existe.Seleccione otro nombre.");
                rw.AddResource("AggregateExist", "El agregado ya existe.Seleccione otro nombre.");
                rw.AddResource("ImagesNoExist", "La entidad no tiene imágenes disponibles.");
                rw.AddResource("NotCodeIdentification", "El código de identificación de usuario no es válido.");
                rw.AddResource("ProductNotSupportAggregate", "El producto seleccionado no admite agregados.");
                rw.AddResource("ProductNotExistInCarShop", "El producto no existe o no se encuentra en el carrito del usuario.");
                rw.AddResource("CarShopNotProducts", "El usuario no ha agregado productos a su carrito de compras.");
                rw.AddResource("IsNotRoleOfUser", "El usuario no tiene el rol especificado.");
                rw.AddResource("IsUserHaveRole", "El usuario ya tiene asignado el role especificado.");
                rw.AddResource("IsNotAssignRoleOfUser", "No se pudo asignar el rol especificado al usuario.");
                rw.AddResource("IsNotRemoveRoleOfUser", "No se pudo eliminar el rol especificado del usuario.");
                #endregion
                //Agrega las imagenes que usa por defecto la aplicación.
                #region Imagenes
                //Crea mapas de bit a partir de las imagenes obtenidas.
                var path_images = $"{Directory.GetCurrentDirectory()}\\Resources\\images";
                var files = Directory.GetFiles(path_images);
                foreach (string fileName in files)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    //Obtiene el nombre de la imagen que corresponde con el Key
                    var name_split = fileName.Split('.');
                    name_split = name_split[1].Split('\\');
                    //Obtiene el nombre de la imagen como tal, que sería el Key de la misma, para 
                    //poder obtener la imagen cdo se lea el archivo de recurso.
                    var key_image = name_split[3];
                    //Crea un mapa de bit de la imagen
                    Bitmap bmp_image = new Bitmap($"{fileName}");
                    bmp_image.Save(memoryStream, ImageFormat.Jpeg);
                    //Guarda la imagen en el archivo de recursos como un mapa de bit.
                    rw.AddResource($"{key_image}", memoryStream);
                }
                #endregion
                //Genera el archivo de recursos.
                rw.Generate();
                //Libera los recursos.
                rw.Dispose();
                //Cierra
                rw.Close();
            }
        }
    }
}
