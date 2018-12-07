using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
namespace lab_01
{
    class Program
    {
        static readonly Uri endPointUri = new Uri(Secrets.UriToCosmosDBAccount);
        const string primaryKey = Secrets.Key;
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Started");
            using (DocumentClient client = new DocumentClient(endPointUri, primaryKey))
            {
                DatabaseAccount account = await client.GetDatabaseAccountAsync();
                await client.OpenAsync();
                await CreateDatabase(client);
                await CreateDefaultCollection(client);
                bool isSQL = IsSQLAPI(account);
                await CreateCustomCollectionsAndIndexingPolicies(client);
                await Populator.PopulateUnlimitedCollectionWithData(client);
            }
        }

        private static bool IsSQLAPI(DatabaseAccount account)
        {
            //TODO - Check for a simple write and read. The exception will occur if CosmosDB Account is not SQL API. Differentiate the exception and determine the API model.
            return true;  
        }

        private async static Task CreateDefaultCollection(DocumentClient client)
        {
            Uri databaseLink = UriFactory.CreateDatabaseUri("EntertainmentDatabase");
            DocumentCollection defaultCollection = new DocumentCollection
            {
                Id = "DefaultCollection"
            };
            defaultCollection = await client.CreateDocumentCollectionIfNotExistsAsync(databaseLink, defaultCollection);
            await Console.Out.WriteLineAsync($"Default Collection created. Self-Link:\t{defaultCollection.SelfLink}");
        }

        static async Task CreateCustomCollectionsAndIndexingPolicies(DocumentClient client)
        {
            Uri dbLink = UriFactory.CreateDatabaseUri("EntertainmentDatabase");
            IndexingPolicy idxPolicy = new IndexingPolicy
            {
                IndexingMode = IndexingMode.Consistent,
                Automatic = true,
                IncludedPaths = new Collection<IncludedPath>
                    {
                        new IncludedPath
                        {
                            Path="/*",
                            Indexes = new Collection<Index>
                            {
                            new RangeIndex(DataType.Number,-1),
                            new RangeIndex(DataType.String,-1),
                        }
                        }
                    }
            };
            PartitionKeyDefinition partDef = new PartitionKeyDefinition
            {
                Paths = new Collection<string> { "/type" }
            };
            DocumentCollection customColl = new DocumentCollection
            {
                Id = "CustomCollection",
                PartitionKey = partDef,
                IndexingPolicy = idxPolicy
            };
            RequestOptions requestOptions = new RequestOptions
            {
                OfferThroughput = 1000
            };
            customColl = await client.CreateDocumentCollectionIfNotExistsAsync(dbLink, customColl, requestOptions);
            await Console.Out.WriteLineAsync($"Custom collection {customColl.Id} created. Self link {customColl.SelfLink}");
        }

        private static async Task CreateDatabase(DocumentClient client)
        {
            Database targetDB = new Database { Id = "EntertainmentDatabase" };
            targetDB = await client.CreateDatabaseIfNotExistsAsync(targetDB);
            Console.WriteLine($"Database {targetDB.Id} created. Self link - {targetDB.SelfLink}");
        }
    }
}
