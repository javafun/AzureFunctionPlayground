# How to use this lab demo

1. Check out the repo to your preferred location.
2. Add `local.settings.json` to your project root with the following settings.
    ```json
    {
        "IsEncrypted": false,
        "Values": {
            "AzureWebJobsStorage": "UseDevelopmentStorage=true",
            "AzureWebJobsSecretStorageType": "files",
            "FUNCTIONS_WORKER_RUNTIME": "dotnet"
        },
        "Host": {
            "CORS": "*"
        }
    }
```
3. Build and run the solution.
4. Open `Index.html` from `TodoWebSite` folder.

