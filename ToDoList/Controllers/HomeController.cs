using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using ToDoList.Models;
using ToDoList.Models.ViewModels;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var todoListViewModel = GetAllTodos();
            return View(todoListViewModel);
        }

        internal TodoViewModel GetAllTodos()
        {
            List<TodoItem> todoList = new List<TodoItem>();

            using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var tableCommand = connection.CreateCommand())
                {
                    connection.Open();
                    tableCommand.CommandText = "SELECT * FROM todo";

                    using (var reader = tableCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                todoList.Add(
                                    new TodoItem
                                    {
                                        Id = reader.GetInt32(0),
                                        Name = reader.GetString(1)
                                    });
                            }
                        }
                        else
                        {
                            return new TodoViewModel
                            {
                                TodoList = todoList
                            };
                        }
                    }
                }
            }

            return new TodoViewModel
            {
                TodoList = todoList
            };
        }

        [HttpGet]
        public JsonResult PopulateForm(int id)
        {
            var todo = GetById(id);
            return Json(todo);
        }

        internal TodoItem GetById(int id)
        {
            TodoItem todo = new TodoItem();

            using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var tableCommand = connection.CreateCommand())
                {
                    connection.Open();
                    tableCommand.CommandText = $"SELECT * FROM todo Where Id = '{id}'";

                    using (var reader = tableCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            todo.Id = reader.GetInt32(0);
                            todo.Name = reader.GetString(1);
                        }
                        else
                        {
                            return todo;
                        }
                    }
                }
            }
            
            return todo;
        }

        public RedirectResult Insert(TodoItem todo)
        {
            using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var tableCommand = connection.CreateCommand())
                {
                    connection.Open();
                    tableCommand.CommandText = $"INSERT INTO todo (name) VALUES ('{todo.Name}')";
                    try
                    {
                        tableCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            return Redirect("https://localhost:7184/");
        }

        public RedirectResult Update(TodoItem todo)
        {
            using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var tableCommand = connection.CreateCommand())
                {
                    connection.Open();
                    tableCommand.CommandText = $"UPDATE todo SET name = '{todo.Name}' WHERE Id = '{todo.Id}'";

                    try 
                    {
                        tableCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            return Redirect("https://localhost:7184/");
        }

        public JsonResult Delete(int Id)
        {
            using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var tableCommand = connection.CreateCommand())
                {
                    connection.Open();
                    tableCommand.CommandText = $"DELETE from todo WHERE Id = '{Id}'";
                    tableCommand.ExecuteNonQuery();
                }
            }
            
            return Json(new {});
        }
    }
}