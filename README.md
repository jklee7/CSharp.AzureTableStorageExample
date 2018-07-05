# CSharp.AzureTableStorageExample
Example demonstrating how to do basic CRUD operations against Azure table storage in .Net Core.

---
### Instructions

##### Packages
The following packages are required to get this working:
```powershell
Install-Package WindowsAzure.Storage
Install-Package Microsoft.Extensions.Configuration
Install-Package Microsoft.Extensions.Configuration.Json
```

_Microsoft.Extensions.Configuration, Microsoft.Extensions.Configuration.Json_: These are not really required, but we use it in order to pull Azure storage account settings from appsettings.json.

---
##### Appsettings
Be sure to update the storage settings in *appsettings.json* with your storage key and table name. E.g.:
```json
"storageConnectionString": "DefaultEndpointsProtocol=https;AccountName=yourstore;AccountKey=LpawL*******************************QQ==;EndpointSuffix=core.windows.net",
"storageTableName": "YourTable"
```

---
### Useful References
For understanding Azure table storage, and how to interact with them using the WindowsAzure.Storage package, refer to Microsoft's own documentation:

- https://docs.microsoft.com/en-us/rest/api/storageservices/understanding-the-table-service-data-model
- https://docs.microsoft.com/en-us/azure/cosmos-db/table-storage-how-to-use-dotnet