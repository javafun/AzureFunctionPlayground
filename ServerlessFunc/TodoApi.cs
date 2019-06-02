using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ServerlessFunc
{
    public static class TodoApis
    {

        [FunctionName("CreateTodo")]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")]HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")]IAsyncCollector<TodoTableEntity> todoTable,
            [Queue("todo", Connection = "AzureWebJobsStorage")]IAsyncCollector<TodoTableEntity> todoQueue,
            TraceWriter log)
        {
            log.Info("Creating a new todo list item");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<CreateTodoModel>(requestBody);

            var todo = new Todo { TaskDescription = input.TaskDescription };

            await todoTable.AddAsync(todo.ToTableEntity());
            await todoQueue.AddAsync(todo.ToTableEntity());
            return new OkObjectResult(todo);
        }

        [FunctionName("GetTodos")]
        public static async Task<IActionResult> GetTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")]HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")]CloudTable todoTable,
            TraceWriter log)
        {

            log.Info("Getting todo list items");
            var query = new TableQuery<TodoTableEntity>();
            var segament = await todoTable.ExecuteQuerySegmentedAsync(query, null);

            return new OkObjectResult(segament.Select(Mappings.ToTodo));
        }

        [FunctionName("GetTodoById")]
        public static IActionResult GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")]HttpRequest req,
            [Table("todo", "TODO", "{id}", Connection = "AzureWebJobsStorage")]TodoTableEntity todo,
            TraceWriter log, string id)
        {
            log.Info("Getting todo item by id");

            if (todo == null)
            {
                log.Info($"Item {id} not found");
                return new NotFoundResult();
            }

            return new OkObjectResult(todo);
        }

        [FunctionName("UpdateTodo")]
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")]CloudTable todoTable,
            TraceWriter log, string id)
        {

            log.Info("Updating an existing todo item");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var updated = JsonConvert.DeserializeObject<UpdateTodoModel>(requestBody);

            var findOperation = TableOperation.Retrieve<TodoTableEntity>("TODO", id);
            var findResult = await todoTable.ExecuteAsync(findOperation);

            if (findResult == null)
                return new NotFoundResult();

            var existingRow = findResult.Result as TodoTableEntity;
            existingRow.IsCompleted = updated.IsCompleted;

            if (!string.IsNullOrWhiteSpace(updated.TaskDescription))
                existingRow.TaskDescription = updated.TaskDescription;

            var replaceOperation = TableOperation.Replace(existingRow);
            await todoTable.ExecuteAsync(replaceOperation);

            return new OkObjectResult(existingRow.ToTodo());
        }

        [FunctionName("DeleteTodo")]
        public static async Task<IActionResult> DeleteTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")]HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")]CloudTable todoTable,
            TraceWriter log, string id)
        {
            log.Info("Deleting an existing todo item");

            var deleteOperation = TableOperation.Delete(
                new TableEntity
                {
                    PartitionKey = "TODO",
                    RowKey = id,
                    ETag = "*"
                });

            try
            {
                await todoTable.ExecuteAsync(deleteOperation);
            }
            catch (StorageException se) when (se.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                return new NotFoundResult();
            }


            return new OkResult();
        }


    }
}
