[![Build Status](https://alexphi.visualstudio.com/side-projects/_apis/build/status/alexphi.azure-netlify-processor?branchName=master)](https://alexphi.visualstudio.com/side-projects/_build/latest?definitionId=12&branchName=master)

# azure-netlify-processor

This repo contains an Azure Function App (C#) to perform two specific tasks for Netlify sites:

**Fetch and process Netlify form submissions**
(See https://www.netlify.com/docs/form-handling/). Instead of setting up notifications in the Netlify site, these functions periodically fetch the received submissions and process them.

Each submission is fetched using the Netlify API and then serialized and routed to an Azure queue. After a submission is successfully routed, it's deleted from Netlify, and the queued submission can then be picked up by external functions or applications.

**Trigger a site re-deploy**
(See https://www.netlify.com/docs/webhooks/). This function is triggered by a queue messsage containing a "deploy signal" (basically a string value) which is mapped to an specific hookId. The function calls the Netlify API to trigger the deployment.

The idea is that external applications can create the deploy signal message in the specified queue and it will be picked up by this function.


## Local Setup

To run this functions locally, you need to add an Azure Storage connection string and a few other settings to a file named `local.settings.json` (not included in the repo):

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "<STORAGE_ACCOUNT_CONNECTION_STRING>",
    "NetlifySettings_BaseUrl": "https://api.netlify.com/api/v1/",
    "NetlifySettings_AccessToken": "<NETLIFY_PERSONAL_ACCESS_TOKEN>",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  }
}
```

## Storage resources

The site list and the rouing configuration (the queue names to write the submissions to) are read from an Azure Storage table called `NetlifyMappings`. There are two types of entities stored in this table:

* Netlify forms config: Contains info about the site to query submmissions for (either the custom domain or site-id/uuid can be used).
  * PartitionKey: 'forms-submissions'
  * RowKey: site domain OR uuid
  * QueueName: name of the queue to write to
* Build Hooks config: Contains info about the build hooks used to trigger new deployments.
  * PartitionKey: 'deploy-signal'
  * RowKey: unique string value identifying the site.
  * HookId: Netlify hook id (i.e. XXXXXXXXXXXXXXX)
  
Also, some Storage queues are used to pass around infomation between functions

* `netlify-submission-info`: stores submissions after downloading them, to decouple the download process from the actual routing process.
* `netlify-deploy-signal`: stores site deploy signals 
