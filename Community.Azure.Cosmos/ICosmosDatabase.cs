using Microsoft.Azure.Cosmos;

namespace Community.Azure.Cosmos
{
    internal interface ICosmosDatabase
    {
        Database Value { get; }
    }
}