[![Build Status](https://alexphi.visualstudio.com/side-projects/_apis/build/status/alexphi.azure-netlify-processor?branchName=master)](https://alexphi.visualstudio.com/side-projects/_build/latest?definitionId=12&branchName=master)

# azure-netlify-processor

Azure functions (v2) to fetch and process Netlify form submissions (https://www.netlify.com/docs/form-handling/). Instead of setting up notifications in the Netlify site, these functions periodically fetch the received submissions and process them.

Each submission is routed to an Azure queue, which is determined using the site url and the form name. The mapping configuration is read from a table in AzureTableStorage.

After a submission is successfully routed, it's deleted from Netlify.

The queued submission can then be picked up by a separate function or another process.

## Configuration values

A few key values must be available as Application Settings for the functions:

* NetlifySettings_BaseUrl: Netlify API url, currently https://api.netlify.com/api/v1/
* NetlifySettings_AccessToken
* NetlifySettings_Sites: a comma-separated list of the sites to fetch submissions from
