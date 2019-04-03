# azure-netlify-processor

Azure functions to fetch and process Netlify form submissions.

The processing is basically mapping the site url/form name combo to an AzureQueue name to route the form data to.

After a submission is successfully routed, it's deleted from Netlify.
