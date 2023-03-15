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
