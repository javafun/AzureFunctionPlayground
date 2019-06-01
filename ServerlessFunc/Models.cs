using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ServerlessFunc
{
    public class Todo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string TaskDescription { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; }
    }

    public class CreateTodoModel
    {
        public string TaskDescription { get; set; }
    }

    public class UpdateTodoModel
    {
        public string TaskDescription { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class TodoTableEntity : TableEntity
    {
        public DateTime CreatedTime { get; set; }
        public string TaskDescription { get; set; }
        public bool IsCompleted { get; set; }
    }
}
