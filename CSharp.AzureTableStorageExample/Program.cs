/*
 * For more information on how Azure table storage models work, read:
 * https://docs.microsoft.com/en-us/rest/api/storageservices/understanding-the-table-service-data-model
 *
 * For information on using the WindowsAzure.Storage library:
 * https://docs.microsoft.com/en-us/azure/cosmos-db/table-storage-how-to-use-dotnet
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharp.AzureTableStorageExample.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;


namespace CSharp.AzureTableStorageExample
{
    class Program
    {
        private static IConfiguration _configuration;
        private static CloudStorageAccount _storageAccount;
        private static CloudTableClient _tableClient;
        private static CloudTable _peopleTable;

        static void Init()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = _configuration["azureStorage:storageConnectionString"];
            var tableName = _configuration["azureStorage:storageTableName"];

            // Create storage account reference
            _storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the table client.
            _tableClient = _storageAccount.CreateCloudTableClient();

            // Get a reference to the table
            _peopleTable = _tableClient.GetTableReference(tableName);

            // Create the CloudTable if it does not exist
            _peopleTable.CreateIfNotExistsAsync().Wait();
        }

        static void Main()
        {
            Init();
            var demoPersonEntity1 = new PersonEntity("John","Smith", "0411223344");
            var demoPersonEntity2 = new PersonEntity("Jane", "Doe", "9999999");
            var demoPersonEntity3 = new PersonEntity("John", "Doe", "1111111");

            // Create
            AddPersonEntityToTable(demoPersonEntity1).Wait();
            AddPersonEntityToTable(demoPersonEntity2).Wait();
            AddPersonEntityToTable(demoPersonEntity3).Wait();

            // Read
            var retrieveResult = FindPersonEntityInTable("Smith", "John").Result;
            if (retrieveResult != null)
            {
                Console.WriteLine($"Result found: {retrieveResult.FirstName} {retrieveResult.LastName} {retrieveResult.PhoneNumber}");
            }
            Console.WriteLine("Sleeping for 5 secs");
            System.Threading.Thread.Sleep(5000);

            // Read all values in table
            var allResults = GetAllPersonEntitiesInTable().Result;
            allResults.ToList().ForEach(x => Console.WriteLine($"{x.FirstName} {x.LastName} {x.PhoneNumber}"));
            Console.WriteLine("Sleeping for 5 secs");
            System.Threading.Thread.Sleep(5000);

            // Update
            UpdatePersonEntityInTable("Smith", "John", "12345678").Wait();
            Console.WriteLine("Sleeping for 5 secs");
            System.Threading.Thread.Sleep(5000);
        
            // Delete
            DeletePersonEntityInTable("Doe", "John").Wait();

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        // Inserting a record into a table
        static async Task AddPersonEntityToTable(PersonEntity personEntity)
        {
            Console.WriteLine($"Adding new entry: {personEntity.FirstName} {personEntity.LastName} {personEntity.PhoneNumber}");
            TableOperation insertOperation = TableOperation.InsertOrReplace(personEntity);
            try
            {
                await _peopleTable.ExecuteAsync(insertOperation);
                Console.WriteLine("Successfully wrote to table store");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Insert to table failed with error {e.Message}");
                throw;
            }
        }

        // Modifying a record in a table
        static async Task UpdatePersonEntityInTable(string lastName, string firstName, string newNumber)
        {
            Console.WriteLine("Updating record");

            TableOperation retrieveOperation = TableOperation.Retrieve<PersonEntity>(lastName, firstName);
            var result = await _peopleTable.ExecuteAsync(retrieveOperation);
            PersonEntity updateEntity = (PersonEntity) result.Result;

            if (updateEntity != null)
            {
                updateEntity.PhoneNumber = newNumber;
                TableOperation updateOperation = TableOperation.Replace(updateEntity);
                await _peopleTable.ExecuteAsync(updateOperation);
            }
            else
            {
                Console.WriteLine("No matching record in table");
            }
        }

        // Querying a table for specific records
        static async Task<PersonEntity> FindPersonEntityInTable(string lastName, string firstName)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<PersonEntity>(lastName, firstName);

            var result = await _peopleTable.ExecuteAsync(retrieveOperation);
            return (PersonEntity)result.Result;
        }

        // Deleting a record in a table
        static async Task DeletePersonEntityInTable(string lastName, string firstName)
        {
            Console.WriteLine($"Deleting entry: {firstName} {lastName}");
            TableOperation retrieveOperation = TableOperation.Retrieve<PersonEntity>(lastName, firstName);
            var result = await _peopleTable.ExecuteAsync(retrieveOperation);
            var deleteEntity = (PersonEntity) result.Result;

            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                await _peopleTable.ExecuteAsync(deleteOperation);
                Console.WriteLine("Delete successful");
            }
            else
            {
                Console.WriteLine("No matching record in table");
            }
        }

        // Get all entities in table
        static async Task<IEnumerable<PersonEntity>> GetAllPersonEntitiesInTable()
        {
            Console.WriteLine("Getting all values in table");
            TableContinuationToken token = null;
            var results = new List<PersonEntity>();

            do
            {
                var queryResult = await _peopleTable.ExecuteQuerySegmentedAsync(new TableQuery<PersonEntity>(), token);
                results.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return results;
        }
    }
}
