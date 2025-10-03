using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using CookingBot.Domain.Payloads;
using CookingBot.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using VkNet.Utils;

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

    public async Task<IReadOnlyList<Recipe>> Get(long chatId)
    {
        return await chatTelegramDb.Recipes.Where(x => x.ChatId == chatId).ToListAsync();
    }

    public async Task<Recipe?> Get(string name)
    {
        return await chatTelegramDb.Recipes.FirstOrDefaultAsync(x => EF.Functions.ILike(x.nameRecipe, name));
    }
}