using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace CSharp.AzureTableStorageExample.Entities
{
    public class PersonEntity : TableEntity
    {
        public string LastName => PartitionKey;
        public string FirstName => RowKey;
        public string PhoneNumber { get; set; }

        public PersonEntity(string firstName, string lastName, string phoneNumber)
        {
            PartitionKey = lastName;
            RowKey = firstName;
            PhoneNumber = phoneNumber;
            Timestamp = DateTimeOffset.UtcNow;
        }

        public PersonEntity(){}
    }
}
