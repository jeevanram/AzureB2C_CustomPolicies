# AzureB2C_UserMigration
Seamless user migration from Database to AzureB2C

References
1. https://learn.microsoft.com/en-us/azure/active-directory-b2c/user-migration
2. https://learn.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-overview
3. https://learn.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-user-flows?pivots=b2c-custom-policy
4. https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack
5. https://github.com/azure-ad-b2c/user-migration/tree/master/seamless-account-migration
6. https://learn.microsoft.com/en-us/azure/active-directory-b2c/restful-technical-profile

 
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

 2) Pre-Migration Step: Migrating Existing User 
    - Use the attribute name “extension_975********************_requiresMigration” 	while migrating users to AzureB2C using Graph API. 
    - POST - https://graph.microsoft.com/v1.0/users 
    - On the Request Header, copy paste the access_token generated above. 
      ![image](https://user-images.githubusercontent.com/5312171/226113225-3b8e1c65-ef1d-4638-8a20-cf884937eb8e.png)
    - Payload to create the user: 
      <<Use the user attribute generated - extension_975********************_requiresMigration>>
      ```diff
      { 
       "displayName": "TestFN TestLN", 
       "identities": [ 
         { 
           "signInType": "emailAddress", 
           "issuer": "yourtenant.onmicrosoft.com", 
           "issuerAssignedId": "test@email.com" 
         } 
       ], 
       "passwordProfile" : { 
         "password": <<TempPassword>>, 
         "forceChangePasswordNextSignIn": false 
       }, 
       "passwordPolicies": "DisablePasswordExpiration", 
       "extension_975********************_requiresMigration": true 
      }
     ![image](https://user-images.githubusercontent.com/5312171/226114312-deaa24c0-58af-4d52-941d-c8d0bf0737c7.png)

 3) Register Custom Policy Applications 
  - Create Signing Key – TokenSigningKeyContainer – Key Type: RSA – Key Usage: Signature 
  - Create Encryption Key – TokenEncryptionKeyContainer – Key Type: RSA – Key Usage: Encryption 
  - Register two applications,  
    - IdentityExperienceFramework, a web API. 
    - ProxyIdentityExperienceFramework, a native app with delegated permission to the IdentityExperienceFramework app. 
    - You need to register these two applications in your Azure AD B2C tenant only once. 
      ![image](https://user-images.githubusercontent.com/5312171/226114949-4194d9b5-c483-4ab6-b7ef-16ec2b37e313.png)
      
      ![image](https://user-images.githubusercontent.com/5312171/226115102-2e855e10-c758-4de0-b17c-a172be1ca03e.png)

      ![image](https://user-images.githubusercontent.com/5312171/226115003-c8b02d93-6138-4e25-bd42-41bb559035e0.png)

 4) How Custom Policy works?
    ![image](https://user-images.githubusercontent.com/5312171/226186086-fd96d5b1-dabc-484f-b8bc-8659f9f59c0e.png)
    
    - A Base file that contains most of the definitions. To help with troubleshooting and long-term maintenance of your policies, try to minimize the number of changes       you make to this file. 
    - A Localization file that holds the localization strings. This policy file is derived from the Base file. Use this file to accommodate different languages to suit       your customer needs. 
    - An Extensions file that holds the unique configuration changes for your tenant. This policy file is derived from the Localization file. Use this file to add new       functionality or override existing functionality. For example, use this file to federate with new identity providers. 
    - A Relying Party (RP) file that is the single task-focused file that is invoked directly by the relying party application, such as your web, mobile, or desktop         applications. Each unique task, such as sign-up, sign-in, or profile edit, requires its own relying party policy file. This policy file is derived from the             extensions file. 
    - The inheritance model is as follows: 
      - The child policy at any level can inherit from the parent policy and extend it by adding new elements. 
      - For more complex scenarios, you can add more inheritance levels (up to 10 in total). 
      - You can add more relying party policies. For example, delete my account, change a phone number, SAML relying party policy and more.
      
 5) Custom Policy Starter Pack 
    https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack 
    - Azure AD B2C custom policy starter pack comes with several pre-built policies to get you started quickly. Each of these starter packs contains the smallest       
      number of technical profiles and user journeys needed to achieve the scenarios described: 
      - LocalAccounts - Enables the use of local accounts only. 
      - SocialAccounts - Enables the use of social (or federated) accounts only. 
      - SocialAndLocalAccounts - Enables the use of both local and social accounts. Most of our samples refer to this policy. 
      - SocialAndLocalAccountsWithMFA - Enables social, local, and multi-factor authentication options. 

    - Refer to https://github.com/jeevanram/AzureB2C_UserMigration/tree/main/CustomPolicies for Custom policies(XMLs) used in Seamless user migration with legacy
      identity provider rest API 
