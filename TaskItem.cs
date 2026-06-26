using System;

namespace CyberBloom
{
    public class TaskItem
    {
        public int TaskID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? ReminderDateTime { get; set; }
        public int? ReminderMinutes { get; set; }
        public bool IsCompleted { get; set; }
    }
}
