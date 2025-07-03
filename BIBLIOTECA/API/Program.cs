using API.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BibliotecaDbContext>();

var app = builder.Build();

//endpoint 1: adicionar livro (post)
app.MapPost("/api/livros", ([FromBody] Livro livro,[FromServices] BibliotecaDbContext ctx)=>
    {
        Categoria? categoria = ctx.Categorias.Find(livro.CategoriaId);
        if (categoria is null){
            return Results.NotFound();
        }
        livro.Categoria = categoria;
        ctx.Livros.Add(livro);
        ctx.SaveChanges();
        return Results.Created("/api/livros" + livro.Id, livro);
    });

app.Run();
