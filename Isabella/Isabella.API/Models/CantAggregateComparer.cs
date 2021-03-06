namespace Isabella.API.Models
{
    using Isabella.API.Models.Entities;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Comparador para la cantidad de agregados
    /// </summary>
    public class CantAggregateComparer : IEqualityComparer<CantAggregate>
    {
        public bool Equals([AllowNull] CantAggregate x, [AllowNull] CantAggregate y)
        {
            throw new System.NotImplementedException();
        }

        public int GetHashCode([DisallowNull] CantAggregate obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
