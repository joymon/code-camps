using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
namespace lab_02
{
    class Program
    {
        protected internal static readonly Uri endPointUri = new Uri(Secrets.UriToCosmosDBAccount);
        const string primaryKey = Secrets.Secret;
        const string databaseId = "UniversityDatabase";
        const string collectionId = "StudentCollection";
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Started");
            using (DocumentClient client = new DocumentClient(endPointUri, primaryKey))
            {
                Database targetDB = new Database { Id = databaseId };
                targetDB = await client.CreateDatabaseIfNotExistsAsync(targetDB);
                Console.WriteLine($"Database creted. Self link - {targetDB.SelfLink}");
                await CreateDocumentCollectionIfNotExists(client, targetDB);

                //string sql = "select top 5 VALUE s.studentAlias from coll s where s.enrollmentYear = 2018";
                //string sql = "select s.clubs from students s where s.enrollmentYear = 2018";
                //Console.WriteLine($"Self link - {targetDB.SelfLink}");
                //IQueryable<string> query = client.CreateDocumentQuery<string>(collectionLink, new SqlQuerySpec(sql));
                // foreach (string alias in query)
                // {
                //     await Console.Out.WriteLineAsync($"Alias - {alias}");
                // }
                //IQueryable<Student> studs = client.CreateDocumentQuery<Student>(collectionLink, new SqlQuerySpec(sql));
                //foreach (Student alias in studs)
                //{
                //    foreach (string club in alias.Clubs)
                //    await Console.Out.WriteLineAsync($"Club - {club}");
                //}
                Console.ReadLine();
            }
        }

        private static async Task CreateDocumentCollectionIfNotExists(DocumentClient client, Database targetDB)
        {
            Uri collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
            DocumentCollection coll = new DocumentCollection() { Id = collectionId };
            coll.UniqueKeyPolicy = new UniqueKeyPolicy()
            {
                UniqueKeys = new Collection<UniqueKey>
                {
                    new UniqueKey { Paths = new Collection<string> { "/studentAlias" }  }
                }
            };
            coll.PartitionKey.Paths.Add("/enrollmentYear");
            coll = await client.CreateDocumentCollectionIfNotExistsAsync(targetDB.SelfLink, coll, new RequestOptions()
            {
                OfferThroughput = 400,
                PartitionKey = new PartitionKey("/enrollmentYear")

            });
            Console.WriteLine($"Document collection created. Self link - {coll.SelfLink}, {coll}");
        }
    }
}
