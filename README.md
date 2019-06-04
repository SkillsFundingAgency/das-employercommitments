# Employer Commitments

This solution represents the Employer Commitments product.

## Getting Started ##

* Clone das-employercommitments repo
* Open das-employercommitments solution - set Startup projects as SFA.DAS.EmployerCommitments.CloudService
* Obtain cloud config
* Workaround for "rosylyn csc exe error" - 'Update-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform -r'
* Workaround for certificate (expired) error - use Edge
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
