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
            return Results.NotFound("Categoria inválida. O ID da categoria fornecido não existe.");
        }
        String? titulo = livro.Titulo;
        if (titulo.Length < 3){
            return Results.BadRequest("Título deve ter no mínimo 3 caracteres.");
        }
        String? autor = livro.Autor;
        if (autor.Length < 3){
            return Results.BadRequest("Autor deve ter no mínimo 3 caracteres.");
        }
        livro.Categoria = categoria;
        ctx.Livros.Add(livro);
        ctx.SaveChanges();
        return Results.Created("/api/livros" + livro.Id, livro);
    });

//endpoint 2: Listar todos os livros (get)
app.MapGet("/api/livros", ([FromServices] BibliotecaDbContext ctx) =>
{
    if(ctx.Livros.Any()){
        return Results.Ok(ctx.Livros.Include(x=>x.Categoria).ToList());
    }
    return Results.NotFound();
});

app.Run();
