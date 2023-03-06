using Microsoft.WindowsAzure.Storage.Table;

namespace Services.Identity.Models
{
    public class UserTable : TableEntity
    {
        public UserTable(string signInName, string password, string firstName, string lastName)
        {
            this.PartitionKey = Constants.MigrationTablePartition;
            this.RowKey = signInName.ToLower();
            this.Password = password;
            this.DisplayName = firstName + " " + lastName;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
