using System;
using System.Collections.Generic;
using Api1Resource.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Api1Resource.Controllers
{
    [Route("api/todo")]
    public class ToDoController: Controller
    {
        private readonly List<ToDo> _toDos;
        private const string Key = "TODO_KEY";
        private readonly IMemoryCache _memoryCache;

        public ToDoController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _toDos = new List<ToDo>
            {
                new ToDo
                {
                    Id = Guid.NewGuid(),
                    Title = "吃饭",
                    Completed = true
                },
                new ToDo
                {
                    Id = Guid.NewGuid(),
                    Title = "学习C#",
                    Completed = false
                },
                new ToDo
                {
                    Id = Guid.NewGuid(),
                    Title = "学习.NET Core",
                    Completed = false
                },
                new ToDo
                {
                    Id = Guid.NewGuid(),
                    Title = "学习 ASP.NET Core",
                    Completed = false
                },
                new ToDo
                {
                    Id = Guid.NewGuid(),
                    Title = "学习 Entity Framework",
                    Completed = false
                }
            };

            if (!memoryCache.TryGetValue(Key, out List<ToDo> todos))
            {
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromDays(1));
                _memoryCache.Set(Key, todos, options);
            }
        }

        public IActionResult Get()
        {
            if (!_memoryCache.TryGetValue(Key, out List<ToDo> todos))
            {
                todos = _toDos;
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromDays(1));
                _memoryCache.Set(Key, todos, options);
            }

            return Ok(todos);
        }

        [HttpPost]
        public IActionResult Post([FromBody]ToDoEdit toDoEdit)
        {
            var todo = new ToDo
            {
                Id = Guid.NewGuid(),
                Title = toDoEdit.Title,
                Completed = toDoEdit.Completed
            };

            if (!_memoryCache.TryGetValue(Key, out List<ToDo> todos))
            {
                todos = _toDos;
            }
            todos.Add(todo);
            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1));
            _memoryCache.Set(Key, todos, options);

            return Ok(todo);
        }
    }
}
