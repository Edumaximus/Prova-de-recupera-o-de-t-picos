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

//endpoint 3: Buscar livro por id (get)
app.MapGet("/api/livros/{id}", ([FromServices] BibliotecaDbContext ctx, [FromRoute] int id) =>
{
    Livro? livro = ctx.Livros.Find(id);
    if (livro == null){
        return Results.NotFound("Livro com ID {id} não encontrado.");
    }

    return Results.Ok(livro);
});

//endpoint 4: Atualizar livro (put)
app.MapPut("/api/livros/{id}", ([FromServices] BibliotecaDbContext ctx, [FromRoute] int id, [FromBody] Livro livroAlterado) =>
{
    Livro? livro = ctx.Livros.Find(id);
    if (livro == null){
        return Results.NotFound("Livro com ID {id} não encontrado para atualização.");
    }

    Categoria? categoria = ctx.Categorias.Find(livroAlterado.CategoriaId);
    if (categoria is null){
        return Results.NotFound("Categoria inválida. O ID da categoria fornecido não existe.");
    }

    String? titulo = livroAlterado.Titulo;
    if (titulo.Length < 3){
        return Results.BadRequest("Título deve ter no mínimo 3 caracteres.");
    }

    String? autor = livroAlterado.Autor;
    if (autor.Length < 3){
        return Results.BadRequest("Autor deve ter no mínimo 3 caracteres.");
    }

    livro.Categoria = categoria;
    livro.Titulo = titulo;
    livro.Autor = autor;

    ctx.Livros.Update(livro);
    ctx.SaveChanges();
    return Results.Ok(livro);
});

app.Run();
