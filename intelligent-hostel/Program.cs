using intelligent_hostel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Npgsql;
using System.Security.Claims;
using System;
using System.Text.Json;
using Newtonsoft.Json;
await Database.init();

var builder = WebApplication.CreateBuilder(/*
    new WebApplicationOptions { WebRootPath = "build"}*/);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/getinfo", [Authorize] async (HttpContext context) =>
{
    await context.Response.WriteAsync("Hello, " + context.User.Identity?.Name);    
});
app.MapGet("/getroomslist", [Authorize] async (HttpContext context) =>
{

    string? room_id = context.Request.Query["level"];
    if (room_id == null)
    {
        context.Response.StatusCode = 404;
        return;
    }
    context.Response.Headers.Append("Content-Type", "application/json");
    var rooms = await Database.getRoomsByLevel(Int32.Parse(room_id));
    string json = JsonConvert.SerializeObject(rooms); 
    await context.Response.WriteAsync(json);

});
app.MapGet("/getstudents_byroom", [Authorize] async (context) =>
{
    string? room_id_raw = context.Request.Query["room_id"];
    if ( room_id_raw == null )
    {
        context.Response.StatusCode = 404; return;
    }
    int room_id = 0; 
    if (!Int32.TryParse(room_id_raw, out room_id))
    {
        context.Response.StatusCode = 400; return;
    }
    Student[] students = await Database.getStudentsByRoom(room_id);
    context.Response.Headers.Append("Content-Type", "application/json");
    string jsonAnswer = JsonConvert.SerializeObject(students);
    context.Response.StatusCode = 200;
    await context.Response.WriteAsync(jsonAnswer);
});
app.MapGet("/getimplements_byroom", [Authorize] async (context) =>
{
    string? room_id_raw = context.Request.Query["room_id"];
    if (room_id_raw == null)
    {
        context.Response.StatusCode = 404; return;
    }
    int room_id = 0;
    if (!Int32.TryParse(room_id_raw, out room_id))
    {
        context.Response.StatusCode = 400; return;
    }
    Implement[] inventory = await Database.getImplementsByRoom(room_id);
    context.Response.Headers.Append("Content-Type", "application/json");
    string jsonAnswer = JsonConvert.SerializeObject(inventory);
    context.Response.StatusCode = 200;
    await context.Response.WriteAsync(jsonAnswer);
});
app.MapPost("/login", async (string? returnUrl, HttpContext context) =>
{
    var form = context.Request.Form;
    if (!form.ContainsKey("login") || !form.ContainsKey("password"))
        return Results.BadRequest("Email и/или пароль не установлены");

    string? login = form["login"];
    string? password = form["password"];
    if (String.IsNullOrWhiteSpace(login) || String.IsNullOrWhiteSpace(password))
        return Results.BadRequest("Email и/или пароль некорректны");
    
    var foundUser = await Database.getUserByLogin(login);
    if (foundUser == null) 
        return Results.BadRequest();

    if (foundUser.password.Replace(" ", "") != Database.CreateSHA256(password).Replace(" ", ""))
        return Results.Unauthorized();

    var claims = new List<Claim> { new Claim(ClaimTypes.Name, login) };
    // создаем объект ClaimsIdentity
    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    // установка аутентификационных куки
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
    return Results.Ok();
});

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok();
});
app.Run();