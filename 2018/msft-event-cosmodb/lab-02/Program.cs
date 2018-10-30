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
        static readonly Uri endPointUri = new Uri("<URL to CosmosDB Account>");
        const string primaryKey = "";
        const string databaseId = "UniversityDatabase";
        const string collectionId = "StudentCollection";
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Started");
            using (DocumentClient client = new DocumentClient(endPointUri, primaryKey))
            {
                Uri collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

                //string sql = "select top 5 VALUE s.studentAlias from coll s where s.enrollmentYear = 2018";
                string sql = "select s.clubs from students s where s.enrollmentYear = 2018";
                //Console.WriteLine($"Self link - {targetDB.SelfLink}");
                //IQueryable<string> query = client.CreateDocumentQuery<string>(collectionLink, new SqlQuerySpec(sql));
                // foreach (string alias in query)
                // {
                //     await Console.Out.WriteLineAsync($"Alias - {alias}");
                // }
                IQueryable<Student> studs = client.CreateDocumentQuery<Student>(collectionLink, new SqlQuerySpec(sql));
                foreach (Student alias in studs)
                {
                    foreach (string club in alias.Clubs)
                    await Console.Out.WriteLineAsync($"Club - {club}");
                }
            }
        }
    }
}
