using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace ServerlessFunc
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer,
            [Table("todos", Connection = "AzureWebJobsStorage")]CloudTable todoTable,
            ILogger log)
        {
            var query = new TableQuery<TodoTableEntity>();
            var segament = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            var deleted = 0;
            foreach (var todo in segament)
            {
                if (todo.IsCompleted)
                {
                    await todoTable.ExecuteAsync(TableOperation.Delete(todo));
                    deleted++;
                }
            }
            log.LogInformation($"Deleted {deleted} items at {DateTime.Now}");
        }
    }
}
