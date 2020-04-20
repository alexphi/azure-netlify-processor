# azure-netlify-processor

This repo contains an Azure Function App (C#) to perform two specific tasks for Netlify sites:

**Fetch and process Netlify form submissions**
(See https://www.netlify.com/docs/form-handling/). These functions periodically fetch the received submissions and process them. Instead of setting up notifications in the Netlify site, you can use this functions to apply custom logic and support complex s
cenarios.

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
    "StorageConnectionString": "<STORAGE_ACCOUNT_CONNECTION_STRING>",
    "NetlifySettings_BaseUrl": "https://api.netlify.com/api/v1/",
    "NetlifySettings_AccessToken": "<NETLIFY_PERSONAL_ACCESS_TOKEN>",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  }
}
```

## Storage resources

Mappings to Netlify sites and some specific settings are read from an Azure Storage table called `NetlifyMappings`. There are two types of entities stored in this table:

**Netlify Submissions mapping**

Contains different data about the sites to query submmissions form and how those submissions are routed.

* PartitionKey: `"submission-routing"`
* RowKey: `{site identifier}-{formName}`
  * Either the custom domain or site-id/uuid can be used as the identifier.
  * To apply the same configuration for all forms within a site, use `{side id}-*`.
* QueueNames: comma-separated names of the queues to route the submission to.

**Netlify Build hooks**

Contains data about sites and their build hooks to trigger deployments.

* PartitionKey: `"deploy-signal"`
* RowKey: unique string value identifying the site.
* HookId: Netlify hook id (i.e. XXXXXXXXXXXXXXX)

Also, some Storage queues are used to pass around infomation between functions

* `netlify-submission-info`: stores submissions after downloading them, to decouple the download process from the actual routing process.
* `netlify-deploy-signal`: stores site deploy signals 
