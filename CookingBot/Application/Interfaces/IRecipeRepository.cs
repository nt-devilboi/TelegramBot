using CookingBot.Domain.Entity;

namespace CookingBot.Application.Interfaces;

public interface IRecipeRepository
{
    Task Upsert(Recipe recipe);
    Task<List<Recipe>> GetByChatId(long chatId);

    Task<Recipe?> GetByChatId(string name);

    Task<Recipe?> GetById(long id);
}