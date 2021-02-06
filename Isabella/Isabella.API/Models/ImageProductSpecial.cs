namespace Isabella.API.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Extras;

    /// <summary>
    /// Entidad que representa las imagenes de los Productos
    /// </summary>
    public class ImageProductSpecial : IModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Producto
        /// </summary>
        public ProductSpecial ProductSpecial { get; set; }

        /// <summary>
        /// Imagen del producto.
        /// </summary>
        public byte[] Image { get; set; }

        /*/// <summary>
        /// Guarda la URL donde se encuentran la imagen del usuario
        /// pero partir del caracter virgulilla.
        /// </summary>
        public string ImageProductPath { get; set; }

        /// <summary>
        /// Devuelve la URL completa donde se encuentran la imagen del usuario(Para apk)
        /// </summary>
        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(this.ImageProductPath))
                {
                    return null;
                }
                //Elimina el caracter virgulilla y devuelve la ruta completa de donde se encuentra la imagen.
                return $"https://inknation.azurewebsites.net{this.ImageProductPath.Substring(1)}";
            }
        }*/
    }
}
