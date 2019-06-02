## How to use this lab demo

> **Note:** To run this solution successfully, you'll need to have `Azure Storage Emulator` installed and configured correctly.
1. Check out the repo to your preferred location.
2. Add `local.settings.json` to your project root folder `ServerlessFunc` with the following settings.
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
4. Run `Index.html` from `TodoWebSite` folder.

