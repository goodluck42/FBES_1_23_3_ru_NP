using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/{page}/{id:int}", (string page, int id) =>
{
	Console.WriteLine($"Id = {id} | Page = {page}");
});

app.MapPost("/user/", (User user) =>
{
	Console.WriteLine(user.Id);
	Console.WriteLine(user.FirstName);
	Console.WriteLine(user.LastName);
});

app.MapGet("/getuser", () => new User(42, "John", "Doe"));

app.Run();




record User(int Id, string FirstName, string LastName);