namespace Isabella.Web.Models.Entities
{
    using System.ComponentModel.DataAnnotations;
    using Models;
    
    /// <summary>
    /// GPS
    /// </summary>
    public class Gps : IEntity
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Latitud(Coordenada GPS)
        /// </summary>
        public double Latitude_Gps { get; set; }

        /// <summary>
        /// Longitude(Coordenada GPS)
        /// </summary>
        public double Longitude_Gps { get; set; }

        /// <summary>
        /// GPS Favorito
        /// </summary>
        public int Favorite_Gps { get; set; }

        /// <summary>
        /// Nombre del GPS
        /// </summary>
        public string Name_Gps { get; set; }
    }
}
