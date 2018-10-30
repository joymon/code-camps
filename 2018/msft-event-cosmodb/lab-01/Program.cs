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
        static readonly Uri endPointUri = new Uri("<URL to cosmosdb account>");
         const string primaryKey="";
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Started");
            using (DocumentClient client=new DocumentClient(endPointUri,primaryKey))
            {
                Database targetDB = new Database{Id="EntertainmentDatabase"};
                targetDB = await client.CreateDatabaseIfNotExistsAsync(targetDB);
                Console.WriteLine($"Self link - {targetDB.SelfLink}");
                Uri dbLink = UriFactory.CreateDatabaseUri("EntertainmentDatabase");
                DocumentCollection defColl = new DocumentCollection{Id="DefaultCollection"};
                defColl = await client.CreateDocumentCollectionIfNotExistsAsync(dbLink,defColl);
                Console.WriteLine($"DefaultCollection Self link - {defColl.SelfLink}");
                IndexingPolicy idxPolicy = new IndexingPolicy
                {
                    IndexingMode = IndexingMode.Consistent,
                    Automatic = true,
                    IncludedPaths = new Collection<IncludedPath>
                    {
                        new IncludedPath{
                        Path="/*",
                        Indexes = new Collection<Index>
                        {
                            new RangeIndex(DataType.Number,-1),
                            new RangeIndex(DataType.String,-1),
                        }    
                        }
                    }
                };
                PartitionKeyDefinition partDef = new PartitionKeyDefinition{
                    Paths = new Collection<string>{"/type"}
                };
                DocumentCollection customColl = new DocumentCollection{
                    Id="CustomCollection",
                    PartitionKey = partDef,
                    IndexingPolicy = idxPolicy
                };
                RequestOptions requestOptions = new RequestOptions{
                    OfferThroughput = 1000
                };
                customColl  = await client.CreateDocumentCollectionIfNotExistsAsync(dbLink,customColl,requestOptions);
            }
        }
    }
}
