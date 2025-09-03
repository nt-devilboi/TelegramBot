using CookingBot.Domain.Entity;

namespace CookingBot.Application.Interfaces;

public interface IRecipeRepository
{
    Task Upsert(Recipe recipe);
    IReadOnlyList<Recipe> Get(long chatId);
}