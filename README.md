# Employer Apprenticeships Service (BETA)

This solution represents the Employer Apprenticeships Service (currently pre private BETA) code base.

### Build
![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/403/badge)

[![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.Account.Api.Client)](https://www.nuget.org/packages/SFA.DAS.Account.Api.Client)

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
