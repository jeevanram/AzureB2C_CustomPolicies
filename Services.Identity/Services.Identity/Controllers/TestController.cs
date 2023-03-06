using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;
using Services.Identity.Models;

namespace Services.Identity.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TestController : Controller
    {
        private readonly AppSettings AppSettings;

        // Demo: Inject an instance of an AppSettingsModel class into the constructor of the consuming class, 
        // and let dependency injection handle the rest
        public TestController(IOptions<AppSettings> appSettings)
        {
            this.AppSettings = appSettings.Value;
        }

        [HttpGet(Name = "PopulateMigrationTable")]
        public async Task<ActionResult> PopulateMigrationTable()
        {

            CloudTable table = await UserMigrationController.GetSignUpTable(this.AppSettings.BlobStorageConnectionString ?? String.Empty);

            // Create the batch operation.
            TableBatchOperation batchOperation = new TableBatchOperation();

            // Create a customer entity and add it to the table.
            List<UserTable> identities = new List<UserTable>();
            identities.Add(new UserTable("Jeff@contoso.com", "1234", "Jeff", "Smith"));
            identities.Add(new UserTable("Ben@contoso.com", "1234", "Ben", "Smith"));
            identities.Add(new UserTable("Linda@contoso.com", "1234", "Linda", "Brown"));
            identities.Add(new UserTable("Sarah@contoso.com", "1234", "Sarah", "Miller"));
            identities.Add(new UserTable("William@contoso.com", "1234", "William", "Johnson"));
            identities.Add(new UserTable("John@contoso.com", "1234", "John", "Miller"));
            identities.Add(new UserTable("Emily@contoso.com", "1234", "Emily", "Miller"));
            identities.Add(new UserTable("David@contoso.com", "1234", "David", "Johnson"));
            identities.Add(new UserTable("Amanda@contoso.com", "1234", "Amanda", "Davis"));
            identities.Add(new UserTable("Brian@contoso.com", "1234", "Brian", "Wilson"));
            identities.Add(new UserTable("Anna@contoso.com", "1234", "Anna", "Miller"));

            // Add both customer entities to the batch insert operation.
            foreach (var item in identities)
            {
                batchOperation.InsertOrReplace(item);
            }

            // Execute the batch operation.
            await table.ExecuteBatchAsync(batchOperation);

            return Ok(identities);
        }
    }
}
