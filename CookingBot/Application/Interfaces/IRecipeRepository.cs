using CookingBot.Domain.Entity;

namespace CookingBot.Application.Interfaces;

public interface IRecipeRepository
{
    Task Upsert(Recipe recipe);
    Task<List<Recipe>> Get(long chatId);

    Task<Recipe?> Get(string name);
}