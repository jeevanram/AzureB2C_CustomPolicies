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
       ![image](https://user-images.githubusercontent.com/5312171/226083295-22780193-e602-4c73-b878-1b5b5470780c.png)
  
   - Create User Attribute “requiresMigration” using Graph API 
     - POST - https://graph.microsoft.com/v1.0/applications/\<\<b2c-extensions-app Application ObjectId\>\>/extensionProperties 
     - b2c-extensions-app is a default application in App registrations “b2c-extensions-app. Do not modify. Used by AADB2C for storing user data” 
       ![image](https://user-images.githubusercontent.com/5312171/226083454-332e56bd-e36d-44f9-8342-7fc460536c56.png)
     - On the Request Header, copy paste the access_token generated above. 
       ![image](https://user-images.githubusercontent.com/5312171/226083535-c1b78fbd-1f1c-47ce-9fa7-bbad069b09fb.png)
     - Payload to create the user attribute: <br/>
       ```diff
       {  
       "name": "requiresMigration",  
       "dataType": "Boolean",  
       "targetObjects": ["User"]
       }
      ![image](https://user-images.githubusercontent.com/5312171/226084216-72934577-7bae-4540-909e-669eaf035da3.png)

    - Grant administrative permission to your application 
      - Click on API permissions. 
      - Click on Add a Permission -> Microsoft Graph. 
      - Select Application Permissions, select the Directory.ReadWrite.All and Application.ReadWrite.All permissions and click Save. 
      - Finally, back in the Application Permissions menu, click on the Grant admin Consent button. 
        
      ![image](https://user-images.githubusercontent.com/5312171/226084316-0f7d207c-9597-4767-bdac-12ef166c1d34.png)


 
