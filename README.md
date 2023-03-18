# AzureB2C_UserMigration
Seamless user migration from Database to AzureB2C

References
1. https://learn.microsoft.com/en-us/azure/active-directory-b2c/user-migration
2. https://learn.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-overview
3. https://learn.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-user-flows?pivots=b2c-custom-policy
4. https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack
5. https://github.com/azure-ad-b2c/user-migration/tree/master/seamless-account-migration

 
Stages of User Migration
1)	Pre-Migration: Migrate existing users to AzureB2C with a default password and the “Require Migration” flag set to true.
2)	When the user logs in, the username and password entered are validated against the legacy identity provider if the “Require migration” flag is set to true. On successful validation, the password entered by the user is updated in the AzureB2C account, and the “Require migration” flag is set to false.
3)	Any login attempt by the user after the initial login validates the user credentials with AzureB2C.

![Just in time migration flow Sign In](Media/signin.png)

Setup Process 

1) Create a user attribute “requiresMigration” using Graph API 
   - Register an Application for User Migration in AzureB2C
     ![image](https://user-images.githubusercontent.com/5312171/226082334-79caff0f-569d-40c4-b348-a1e5e0de6280.png)
     - Create a new application 
     - For Name, use B2CUserMigration. 
     - For Supported account types, use Accounts in this organizational directory only. 
     - For Redirect URI, use type Web: https://localhost (it's not relevant for this application). 
     - Click Register 
   - Create a Client Secret
     
     ![image](https://user-images.githubusercontent.com/5312171/226082459-817a36cf-d416-473a-97e6-83d7b70dc8eb.png)
     - Click on Certificates and Secrets menu and add a new key (also known as client secret). Copy the key for later. 
     - Key name: client-secret 
       ![image](https://user-images.githubusercontent.com/5312171/226082600-ddff604a-8f37-4b32-b342-32dc3cef1286.png)

   - GET AccessToken to invoke MicrosoftGraph API 
     - POST - https://login.microsoftonline.com/yourtenant.onmicrosoft.com/oauth2/token 
     - BODY: x-www-form-urlencoded 
     - grant_type = client_credentials 
     - client_id = \<\<Application Id of B2CUserMigration\>\> 
     - client_secret=\<\<Client Secret Key created for B2CUserMigration Application\>\> 
     - Resource= https://graph.microsoft.com 
