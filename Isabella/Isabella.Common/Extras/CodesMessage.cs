namespace Isabella.Common.Extras
{
    using System.Collections.Generic;

    public static class CodeMessage
    {
        public enum Code
        {
            //Success//
            CodeSuccess_Ok = 1,

            //Errors//
            CodeError_Exception = 2,
            CodeError_NullObjectSend = 3,
            CodeError_DataBase = 4,   
            
            //Users//
            CodeUser_NotFound = 5,
            CodeUser_AllNotFound = 6,
            CodeUser_BadUserName = 7,
            CodeUser_NotConfirmRegister = 8,
            CodeUser_ErrorGenerateToken = 104,
            CodeUser_LoginTokenUser = 105,
            CodeUser_CodeVerificationNotCorrect = 106,
            CodeUser_ValueNotValide = 107,
            CodeUser_PasswordsNotEquals = 108,
            CodeUser_NotRecoverPassword = 109,
            CodeUser_BadEmail = 110,
            CodeUser_NotNew = 111,
            CodeUser_YesConfirmRegister = 112,
            CodeUser_NotNewUser = 113,
            CodeUser_NotEmail = 114,
            CodeUser_PasswordNotCorrect = 115,
            CodeUser_VerifyPasswordAndUserAccount = 116,

            //Roles
            CodeRole_BadRole = 200,
            CodeRole_RoleNotAuthorization = 201,

            //Emails
            EmailRegisterConfirmation = 300,
            CodeRecoverPassword = 301,
            EmailNotSend = 302,

            //Product
            CodeProduct_NotFound = 400,
            CodeProduct_AllNotFound = 401,
            CodeProduct_NotNew = 402,
            CodeProduct_AddCalificationNotPossible = 403,

            //Category
            CodeCategory_NotFound = 500,
            CodeCategory_Exist = 501,
            CodeCategory_NotAllFound = 502,
            

            //Images        
            CodeImage_ImageUserNotValide = 600,
            CodeImage_ImageProductNotValide = 601,
            CodeImage_ImageErrorValue = 602,
            CodeImage_ImageErrorCreated = 603,
            CodeImage_ImageNotExist = 604,
            CodeImage_ProductNotImage = 605,
            CodeImage_ProductNotNewImage = 606,

            //CodeIdentification
            CodeIdentification_NotCode = 607,
            //CarShop
            CodeCarShop_NotProducts = 608,
        }

        public static string MessageOfCode(Code code)
        {
            switch(code)
            {
                //Success y Errors
                case Code.CodeSuccess_Ok:
                return "La operación se ha ejecutado correctamente.";
                case Code.CodeError_Exception:
                return "Se ha generado una excepción en la aplicación.";
                case Code.CodeError_NullObjectSend:
                return "Se ha enviado un objeto nulo.";
                case Code.CodeError_DataBase:
                return "Error de base de datos.No se pudo realizar la operación.";
               
                //Users
                case Code.CodeUser_NotFound:
                return "El usuario no se encuentra en la base de datos.";
                case Code.CodeUser_AllNotFound :
                return "No hay usuarios en la base de datos.";
                case Code.CodeUser_BadUserName:
                return "La cuenta de usuario seleccionada está en uso.Seleccione otra cuenta de usuario.";
                case Code.CodeUser_BadEmail:
                return "El correo electrónico seleccionado está en uso.Seleccione otro correo electrónico.";
                case Code.CodeUser_NotConfirmRegister:
                return "El usuario no ha confirmado el registro en la aplicación Duma.";
                case Code.CodeUser_YesConfirmRegister:
                return "El usuario ya ha confirmado el registro en la aplicación.";
                case Code.CodeUser_ErrorGenerateToken:
                return "Error al generar el Token del usuario.";
                case Code.CodeUser_LoginTokenUser:
                return "El usuario ha iniciado sesión en el sistema.Se le ha enviado el Token al usuario y el tiempo de expioración del mismo.";
                case Code.CodeUser_CodeVerificationNotCorrect:
                return "El código de verificación es incorrecto.";
                case Code.CodeUser_PasswordsNotEquals:
                return "La nueva contraseña no es igual a la contraseña de confirmación.";
                case Code.CodeUser_NotRecoverPassword:
                return "El usuario no ha solicitado la recuperación de la contraseña.";
                case Code.CodeUser_ValueNotValide:
                return "El valor enviado por el usuario no es válido.";
                case Code.CodeUser_NotNew:
                return "No se han agregado nuevos usuarios al sistema.";
                case Code.CodeUser_NotEmail:
                return "Para loguearse debe especificar el email.";
                case Code.CodeUser_NotNewUser:
                return "No se han agregado nuevos usuarios al sistema.";
                case Code.CodeUser_PasswordNotCorrect:
                return "La contraseña es incorrecta.";
                case Code.CodeUser_VerifyPasswordAndUserAccount:
                return "Verifique la contraseña y el usuario.";

                //Emails
                case Code.EmailRegisterConfirmation:
                return "Se la enviado un correo electrónico con el código de confirmación para finalizar el registro en la aplicación Duma.";
                case Code.CodeRecoverPassword:
                return "Se le ha enviado un correo con el código para la recuperación de la contraseña.";
                case Code.EmailNotSend:
                return "No se envió el email, imposible de conectar con el servidor SMTP";
                
                //Roles
                case Code.CodeRole_BadRole:
                return "El role seleccionado no es válido.";
                case Code.CodeRole_RoleNotAuthorization:
                return "El usuario no tiene autorización para realizar esta operación.";

                //Products
                case Code.CodeProduct_NotFound:
                return "El producto seleccionado no se encuentra en la base de datos.";
                case Code.CodeProduct_NotNew:
                return "No hay nuevos productos de este tipo en estos momentos.";
                case Code.CodeProduct_AllNotFound:
                return "No hay productos de este tipo en estos momentos.";
                case Code.CodeProduct_AddCalificationNotPossible:
                return "El usuario solo puede emitir una calificación de un producto una vez al día.";

                //CategoryProductStandard
                case Code.CodeCategory_NotFound:
                return "La categoria no se encuentra en la base de datos.";
                case Code.CodeCategory_NotAllFound:
                return "No hay categorias de este tipo en la base de datos.";
                case Code.CodeCategory_Exist:
                return "La categoria ya existe en la base de datos.";

                //Imagenes
                case Code.CodeImage_ImageUserNotValide:      
                return "La imagen de perfil de usuario no puede ser mayor de 100Kb.";
                case Code.CodeImage_ImageProductNotValide:
                return "La imagen de un producto no puede ser mayor de 100Kb.";
                case Code.CodeImage_ImageErrorValue:
                return "Error en la longitud de la imagen. Verifique que la imagen seleccionada sea valida.";
                case Code.CodeImage_ImageErrorCreated:
                return "Error al crear la imagen.";
                case Code.CodeImage_ImageNotExist:
                return "La imagen seleccionada no está disponible en la base de datos o no pertenece a al producto seleccionado.";
                case Code.CodeImage_ProductNotImage:
                return "El producto no tiene imagenes.";
                case Code.CodeImage_ProductNotNewImage:
                return "El producto no tiene nuevas imagenes.";

                //Codigo de identificación
                case Code.CodeIdentification_NotCode:
                return "El código de identificación no está disponible.Solicite un código de identificación para poder realizar un pedido.";

                //CarShop
                case Code.CodeCarShop_NotProducts:
                return "No hay productos en el carro de compras.";

                default:
                return "Código desconocido";
            }
        }
    }

}
