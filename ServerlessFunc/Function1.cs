using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServerlessFunc
{
    public static class TodoApis
    {
        static Collection<Todo> items = new Collection<Todo>();

        [FunctionName("CreateTodo")]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")]HttpRequest req,
            TraceWriter log)
        {
            log.Info("Creating a new todo list item");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<CreateTodoModel>(requestBody);

            var todo = new Todo { Description = input.TaskDescription };
            items.Add(todo);

            return new OkObjectResult(todo);
        }

        [FunctionName("GetTodos")]
        public static IActionResult GetTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous,"get",Route ="todo")]HttpRequest req,
            TraceWriter log)
        {

            log.Info("Getting todo list items");

            return new OkObjectResult(items);
        }

        [FunctionName("GetTodoById")]
        public static IActionResult GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous,"get",Route ="todo/{id}")]HttpRequest req,
            TraceWriter log, string id)
        {
            log.Info("Getting a todo item");


            var todo = items.FirstOrDefault(x => x.Id.Equals(id));

            if (todo == null)
                return new NotFoundResult();

            return new OkObjectResult(todo);
        }

        [FunctionName("UpdateTodo")]
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous,"put",Route = "todo/{id}")] HttpRequest req,
            TraceWriter log, string id)
        {

            log.Info("Updating an existing todo item");

            var todo = items.FirstOrDefault(x => x.Id.Equals(id));

            if (todo == null)
                return new NotFoundResult();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var updated = JsonConvert.DeserializeObject<UpdateTodoModel>(requestBody);

            todo.IsCompleted = updated.IsCompleted;

            if (!string.IsNullOrWhiteSpace(updated.TaskDescription))
                todo.Description = updated.TaskDescription;

            return new OkObjectResult(todo);
        }

        [FunctionName("DeleteTodo")]
        public static IActionResult DeleteTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous,"delete",Route ="todo/{id}")]HttpRequest req, 
            TraceWriter log, string id)
        {
            log.Info("Deleting an existing todo item");

            var todo = items.FirstOrDefault(x => x.Id.Equals(id));

            if (todo == null)
                return new NotFoundResult();

            items.Remove(todo);
            return new OkResult();
        }


    }
}
