using Newtonsoft.Json;

namespace Services.Identity.Models
{
    public class InputClaims
    {
        // Demo: User's object id in Azure AD B2C
        public string? signInName { get; set; }
        public string? password { get; set; }
        public string? objectId { get; set; }
        public bool useInputPassword { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static InputClaims? Parse(string JSON)
        {

            return JsonConvert.DeserializeObject(JSON, typeof(InputClaims)) as InputClaims;

        }
    }
}
