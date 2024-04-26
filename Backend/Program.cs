using Managers;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

DBManager dbManager = new DBManager("Database");
dbManager.NewTable("Users", new List<(string, string)> { ("Id", "INTEGER PRIMARY KEY"), ("Username", "STRING"), ("Password", "STRING") });
dbManager.NewTable("Tasks", new List<(string, string)> { ("Id", "INTEGER PRIMARY KEY"), ("User_Id", "INTEGER NOT NULL"), ("Header", "STRING"), ("Description", "STRING"), ("Done", "INT") });

app.MapPost("/API/V1/{user_id}/Tasks", (int user_id, TodoTask task) => {
    task = new TodoTask(-1, user_id, task.header, task.description, false);
    dbManager.NewTask(task.user_id, task.header, task.description);
});

app.MapPost("/API/V1/Users", (User user) => {
    dbManager.NewUser(user.name, user.password);
});

app.MapGet("/API/V1/{user_id}/Tasks", (int user_id) => {
    List<TodoTask> tasks = dbManager.GetTasksFromUserId(user_id);
    return tasks;
});

app.MapPost("/API/V1/Login", (User user) => {
    int user_id = dbManager.SignIn(user.name, user.password);
    return user_id;
});

app.MapGet("/API/V1/{user_id}/Tasks/{id}", (int id) => {
    TodoTask? task = dbManager.GetTaskDataFromId(id);
    return task;
});

app.MapPut("/API/V1/{user_id}/Tasks/{id}", (int id, TodoTask task) => {
    dbManager.UpdateTask(task.header, task.description, task.done, id);
});

app.MapDelete("/API/V1/{user_id}/Tasks/{id}", (int id) => {
    dbManager.DeleteTaskOfId(id);
});

app.MapGet("/API/V1/Users", () =>
{
    List<string> usernames = dbManager.GetUsernames();
    return usernames;
});

app.Run();