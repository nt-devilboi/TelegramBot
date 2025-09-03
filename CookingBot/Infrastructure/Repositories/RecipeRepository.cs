using CookingBot.Application.Interfaces;
using CookingBot.Domain.Entity;
using CookingBot.Infrastucture.DataBase;
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

    public IReadOnlyList<Recipe> Get(long chatId)
    {
        return chatDb.Recipes.Where(x => x.ChatId == chatId).ToReadOnlyCollection();
    }
}