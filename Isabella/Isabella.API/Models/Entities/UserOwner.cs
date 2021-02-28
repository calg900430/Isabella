namespace Isabella.API.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Usuario Cliente
    /// </summary>
    public class UserOwner : IEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Usuario
        /// </summary>
        public User User { get; set; }
    }
}
