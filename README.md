# Employer Commitments

This solution represents the Employer Commitments product.

## Getting Started ##

* Clone das-employercommitments repo
* Open das-employercommitments solution - set Startup projects as SFA.DAS.EmployerCommitments.CloudService
* Obtain cloud config
	- SFA.DAS.EmployerCommitments
	- SFA.DAS.AuditApiClient
	- SFA.DAS.EmployerAccountAPI
* Workaround for "rosylyn csc exe error" - 'Update-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform -r'
* In SFA.DAS.CloudService/ServiceConfiguration.Local.cscfg change the certificate thumbprint with a cerificate whose Subject is Localhost and Intended purpose is Server Authentication.
* Install the DasIDPCert (In the install wizard I selected Local Computer and let the wizard select the best store, it ended up in my Local Computer - Personal Certificate)
* For dev env Update the SFA.DAS.EmployerAccountAPI with
	{
	   "ClientId": "0a25a495-140e-4f9f-807b-11e2fa115375",
	   "ServiceBusConnectionString": "",
	   "ApiBaseUrl": "https://sfa-stub-employeraccountapi.herokuapp.com",
	   "IdentifierUri": "https://citizenazuresfabisgov.onmicrosoft.com/eas-api",
	   "ClientSecret": "lP86XAzrbGY1Jd/uB0RWXTXyPtswU4gNM6CyGso//B8=",
	    "DatabaseConnectionString": "Server=.;Database=SFA.DAS.EAS.Employer_Account.Database;Integrated Security=True;Trusted_Connection=True;Connection Timeout=30;",
	   "Tenant": "citizenazuresfabisgov.onmicrosoft.com"
	}
* Start


### Feature Toggle

You can limit areas of the site by adding them to a list, in the controller action format, or having a * to denote all actions within that controller. Below is an example of the config required:

```
{   "Data": [     {       "Controller": "EmployerTeam",       "Action": "Invite"     }   ] }
```
This is added to the configuration table of your local azure storage, with the PartiionKey being **LOCAL** and the RowKey being **SFA.DAS.EmployerApprenticeshipsService.Features_1.0**

## Account API
The Employer Apprenticeships Service provides a REST Api and client for accessing Employer accounts. Nuget link above.

* The API can be found in [src/SFA.DAS.EAS.Api](src/SFA.DAS.EAS.Api)
* The client can be found in [src/SFA.DAS.Account.Api.Client](src/SFA.DAS.Account.Api.Client)
