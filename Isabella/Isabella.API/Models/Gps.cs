namespace Isabella.API.Models
{
    using System.ComponentModel.DataAnnotations;
    using Extras;
    
    /// <summary>
    /// GPS
    /// </summary>
    public class Gps : IModel
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
