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
        static readonly Uri endPointUri = new Uri("");
        const string primaryKey = "";
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Started");
            using (DocumentClient client = new DocumentClient(endPointUri, primaryKey))
            {
                CreateCollectionsAndIndexingPolicies(client);
                await PopulateUnlimitedCollectionWithData(client);
            }
        }
        private static async Task PopulateUnlimitedCollectionWithData(DocumentClient client)
        {
            await client.OpenAsync();
            Uri collectionLink = UriFactory.CreateDocumentCollectionUri("EntertainmentDatabase", "CustomCollection");
            var foodInteractions = new Bogus.Faker<PurchaseFoodOrBeverage>()
            .RuleFor(i => i.type, (fake) => nameof(PurchaseFoodOrBeverage))
            .RuleFor(i => i.unitPrice, (fake) => Math.Round(fake.Random.Decimal(1.99m, 15.99m)))
            .RuleFor(i => i.quantity, (fake) => fake.Random.Number(1,5))
            .RuleFor(i => i.totalPrice, (fake,user) => Math.Round(user.unitPrice * user.quantity,5))
            .Generate(100);
            foreach (var interaction in foodInteractions)
            {
                ResourceResponse<Document> result = await client.CreateDocumentAsync(collectionLink, interaction);
                await Console.Out.WriteLineAsync($"Document #{foodInteractions.IndexOf(interaction):000} created.  Id {result.Resource.Id}");
            }
            
            DocumentCollection coll = await client.ReadDocumentCollectionAsync(collectionLink);
            await Console.Out.WriteLineAsync(coll;
        }

        static async void CreateCollectionsAndIndexingPolicies(DocumentClient client)
        {
            Database targetDB = new Database { Id = "EntertainmentDatabase" };
            targetDB = await client.CreateDatabaseIfNotExistsAsync(targetDB);
            Console.WriteLine($"Self link - {targetDB.SelfLink}");
            Uri dbLink = UriFactory.CreateDatabaseUri("EntertainmentDatabase");
            DocumentCollection defColl = new DocumentCollection { Id = "DefaultCollection" };
            defColl = await client.CreateDocumentCollectionIfNotExistsAsync(dbLink, defColl);
            Console.WriteLine($"DefaultCollection Self link - {defColl.SelfLink}");
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
            Console.Out.WriteLineAsync($"Custom collection created. Self link {customColl.SelfLink}");
        }

    }
}
