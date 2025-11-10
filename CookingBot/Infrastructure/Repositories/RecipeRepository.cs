using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using CookingBot.Domain.Payloads;
using CookingBot.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace CookingBot.Infrastructure.Repositories;

public class RecipeRepository(ChatTelegramDb chatTelegramDb) : IRecipeRepository
{
    public async Task Upsert(Recipe recipe)
    {
        var recipeDb = await chatTelegramDb.Recipes.FindAsync(recipe.Id);

        if (recipeDb == null)
        {
            chatTelegramDb.Recipes.Add(recipe);
        }
        else
        {
            chatTelegramDb.Entry(recipeDb).CurrentValues.SetValues(recipe);
            recipe.Ingredients = new Dictionary<string, IngredientDetail>(recipe.Ingredients);
        }

        await chatTelegramDb.SaveChangesAsync();
    }

    public async Task<List<Recipe>> GetByChatId(long chatId)
    {
        return await chatTelegramDb.Recipes.Where(x => x.ChatId == chatId).ToListAsync();
    }

    public async Task<Recipe?> GetByChatId(string name)
    {
        return await chatTelegramDb.Recipes.FirstOrDefaultAsync(x => EF.Functions.ILike(x.nameRecipe, name));
    }

    public async Task<Recipe?> GetById(Guid id)
    {
        return await chatTelegramDb.Recipes.FindAsync(id);
    }
}