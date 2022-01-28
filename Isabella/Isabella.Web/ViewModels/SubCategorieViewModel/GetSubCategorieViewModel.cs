namespace Isabella.Web.ViewModels.SubCategorieViewModel
{
    using Isabella.Common.Dtos.SubCategorie;
    using Isabella.Web.ViewModels.ProductViewModel;

    /// <summary>
    /// GetCategorieViewModel
    /// </summary>
    public class GetSubCategorieViewModel : GetSubCategorieDto
    {
       /// <summary>
       /// Producto
       /// </summary>
       public GetProductViewModel GetProductViewModel { get; set; }
    }
}
