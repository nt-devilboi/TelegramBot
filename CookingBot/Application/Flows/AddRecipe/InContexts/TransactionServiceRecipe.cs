using CookingBot.Application.Commands.AddRecipe.Flow.ContextHandlers;
using EasyTgBot.Abstract;

namespace CookingBot.Application.Flows.AddRecipe.InContexts;

public class TransactionServiceRecipe : TransactionService<AddingRecipeContext>
{
    protected override Dictionary<AddingRecipeContext, AddingRecipeContext> NextStepFrom { get; } = new()
    {
        {
            AddingRecipeContext.AddingName, AddingRecipeContext.AddingIngredient
        },
        {
            AddingRecipeContext.AddingIngredient, AddingRecipeContext.AddingInstruction
        },
        {
            AddingRecipeContext.AddingInstruction, AddingRecipeContext.SaveRecipe
        }
    };

    protected override Dictionary<AddingRecipeContext, AddingRecipeContext> CanMove { get; } = new()
    {
        {
            AddingRecipeContext.AddingName, AddingRecipeContext.AddingIngredient
        }
    };
}