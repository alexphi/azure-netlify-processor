[![Build Status](https://alexphi.visualstudio.com/side-projects/_apis/build/status/alexphi.azure-netlify-processor?branchName=master)](https://alexphi.visualstudio.com/side-projects/_build/latest?definitionId=12&branchName=master)

# azure-netlify-processor

Azure functions to fetch and process Netlify form submissions.

The processing is basically mapping the site url/form name combo to an AzureQueue name to route the form data to.

After a submission is successfully routed, it's deleted from Netlify.
