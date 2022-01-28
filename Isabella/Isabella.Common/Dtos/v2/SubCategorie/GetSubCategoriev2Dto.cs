namespace Isabella.Common.Dtos.v2.SubCategorie
{
    using Isabella.Common.Dtos.Product;
    using Isabella.Common.Dtos.SubCategorie;

    public class GetSubCategoriev2Dto : GetSubCategorieDto
    {
       public GetProductDto GetProductDto { get; set; }
    }
}
