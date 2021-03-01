namespace Isabella.API.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Restaurante
    /// </summary>
    public class Restaurant : IEntity
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }
    }
}
