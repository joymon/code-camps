using System.Threading.Tasks;
using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
namespace lab_01
{
    class Populator
    {
        internal static async Task PopulateUnlimitedCollectionWithData(DocumentClient client)
        {
            await client.OpenAsync();
            Uri collectionLink = UriFactory.CreateDocumentCollectionUri("EntertainmentDatabase", "CustomCollection");
            var foodInteractions = new Bogus.Faker<PurchaseFoodOrBeverage>()
            .RuleFor(i => i.type, (fake) => nameof(PurchaseFoodOrBeverage))
            .RuleFor(i => i.unitPrice, (fake) => Math.Round(fake.Random.Decimal(1.99m, 15.99m)))
            .RuleFor(i => i.quantity, (fake) => fake.Random.Number(1, 5))
            .RuleFor(i => i.totalPrice, (fake, user) => Math.Round(user.unitPrice * user.quantity, 5))
            .Generate(100);
            foreach (var interaction in foodInteractions)
            {
                ResourceResponse<Document> result = await client.CreateDocumentAsync(collectionLink, interaction);
                await Console.Out.WriteLineAsync($"Document #{foodInteractions.IndexOf(interaction):000} created.  Id {result.Resource.Id}");
            }

            DocumentCollection coll = await client.ReadDocumentCollectionAsync(collectionLink);
            await Console.Out.WriteLineAsync(coll.SelfLink);
        }
    }
}
