using Fina.Api.Data;
using Fina.Core.Handlers;
using Fina.Core.Models;
using Fina.Core.Requests.Categories;
using Fina.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fina.Api.Handlers
{
    public class CategoryHandler(AppDbContext db) : ICategoryHandler
    {
        public async Task<Response<Category?>> CreateAsync(CreateCategoryRequest request)
        {
            var category = new Category
            {
                UserId = request.UserId,
                Title = request.Title,
                Description = request.Description,
            };

            try
            {

                await db.Categories.AddAsync(category);
                await db.SaveChangesAsync();

                return new Response<Category?>(category, 201, "Categoria criada com sucesso");
            }
            catch
            {
                return new Response<Category?>(null, 500, "Não foi possivel criar a categoria");
            }

        }

        public async Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request)
        {
            try
            {
                var category = await db
                    .Categories
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
                if (category is null)
                    return new Response<Category?>(null, 404, "Categoria não encontrada");


                db.Categories.Remove(category);
                await db.SaveChangesAsync();

                return new Response<Category?>(category, message: "Categoria removida com sucesso!");
            }
            catch
            {
                return new Response<Category?>(null, 500, "Não foi possivel remover a categoria");

            }
        }

        

        public async Task<Response<Category?>> GetByIdAsync(GetCategoryByIdRequest request)
        {
            try
            {
                var category = await db
                    .Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == x.UserId);
                return category is null
                       ? new Response<Category?>(null, 404, "Categoria não encontrada!")
                       : new Response<Category?>(category);

            }
            catch
            {
                return new Response<Category?>(null, 500, "Não foi possivel remover a categoria");
            }
        }

        public async Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request)
        {
            try
            {
                var category = await db
                    .Categories
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

                if (category is null)
                    return new Response<Category?>(null, 404, "Categoria não encontrada");

                category.Title = request.Title;
                category.Description = request.Description;

                db.Categories.Update(category);
                await db.SaveChangesAsync();

                return new Response<Category?>(category, message: "Categoria atualizada com sucesso");
            }
            catch
            {
                return new Response<Category?>(null, 500, "Não foi possível alterar a categoria");
            }
        }
        public async Task<PagedResponse<List<Category>?>> GetAllAsync(GetAllCategoriesRequest request)
        {
            try
            {
                var query = db
                 .Categories
                 .AsNoTracking()
                 .Where(x => x.UserId == request.UserId)
                 .OrderBy(x => x.Title);

                var categories = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var count = await query.CountAsync();

                return new PagedResponse<List<Category>?>(
                    categories,
                    count,
                    request.PageNumber,
                    request.PageSize
                    );
            }
            catch
            {
                return new PagedResponse<List<Category>?>(null, 500, "Houve um problema ao listar as categorias");
            }
        }
    }
}
