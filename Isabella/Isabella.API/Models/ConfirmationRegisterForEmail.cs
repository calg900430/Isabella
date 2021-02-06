namespace Isabella.API.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Extras;

    /// <summary>
    /// Representa la entidad para la confirmación de registro.
    /// </summary>
    [Table("ConfirmationsEmails")]
    public class ConfirmationRegisterForEmail : IModel
    { 
       /// <summary>
       /// Key
       /// </summary>
       [Key]
       public int Id { get; set; }

       /// <summary>
       /// User
       /// </summary>
       public User User { get; set; }
       
       /// <summary>
       /// Token
       /// </summary>
       public string Token { get; set; }
    }
}
