using Microsoft.Azure.Cosmos;

namespace Dam.Model.CosmosDb
{
    public static class CosmosDbExtensions
    {
        public static void DefaultDamSetup(this IndexingPolicy indexingPolicy, bool automatic)
        {
            indexingPolicy.Automatic = automatic;
            indexingPolicy.IndexingMode = IndexingMode.Consistent;
        }
        public static void ClearIndexingPaths(this IndexingPolicy indexingPolicy)
        {
            indexingPolicy.IncludedPaths.Clear();
            indexingPolicy.ExcludedPaths.Clear();
            indexingPolicy.ExcludedPaths.Add(new ExcludedPath() { Path = "/" });
        }

        public static void IncludeEntityPaths(this IndexingPolicy indexingPolicy)
        {
            indexingPolicy.IncludedPaths.Add(new IncludedPath() { Path = $"{Entity.PropertyPath.Active}/?" });
            indexingPolicy.IncludedPaths.Add(new IncludedPath() { Path = $"{Entity.PropertyPath.CreatedOn}/?" });
            indexingPolicy.IncludedPaths.Add(new IncludedPath() { Path = $"{Entity.PropertyPath.Pk}/?" });
            indexingPolicy.IncludedPaths.Add(new IncludedPath() { Path = $"{Entity.PropertyPath.Tags}/?" });
            indexingPolicy.IncludedPaths.Add(new IncludedPath() { Path = $"{Entity.PropertyPath.Type}/?" });
        }        
    }
}
