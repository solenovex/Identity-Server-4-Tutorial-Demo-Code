using System;

namespace Api1Resource.Models
{
    public class ToDo
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }
    }
}
