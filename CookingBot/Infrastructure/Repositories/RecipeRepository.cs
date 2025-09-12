using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using CookingBot.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using VkNet.Utils;

namespace CookingBot.Infrastructure.Repositories;

public class RecipeRepository(ChatDb chatDb) : IRecipeRepository
{
    public async Task Upsert(Recipe recipe)
    {
        var recipeDb = await chatDb.Recipes.FindAsync(recipe.Id);

        if (recipeDb == null) chatDb.Recipes.Add(recipe);
        else chatDb.Entry(recipeDb).CurrentValues.SetValues(recipe);

        await chatDb.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Recipe>> Get(long chatId)
    {
        return await chatDb.Recipes.Where(x => x.ChatId == chatId).ToListAsync();
    }

    public async Task<Recipe?> Get(string name)
    {
        return await chatDb.Recipes.FirstOrDefaultAsync(x => x.nameRecipe == name.ToLower());
    }
}