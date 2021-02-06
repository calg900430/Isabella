namespace Isabella.Common.Dtos.Product
{
    public class GetProductImageDto
    {
        /// <summary>
        /// Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Guarda la URL donde se encuentran la imagen de la publicación
        /// pero partir del caracter virgulilla.
        /// </summary>
        public string ImageProductPath { get; set; }

        /// <summary>
        /// Devuelve la URL completa donde se encuentran la imagen del usuario(Para apk)
        /// </summary>
        public string ImageFullPath { get; set; }  
    }
}
