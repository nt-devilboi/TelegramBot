using CookingBot.Commands.AddRecipe.Flow;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;

namespace CookingBot.Application.Commands.AddRecipe.Flow;

public class TransactionService : ITransactionService
{
    private readonly Dictionary<AddingRecipeStateContext, AddingRecipeStateContext> NextStepFrom = new()
    {
        {
            AddingRecipeStateContext.AddingName, AddingRecipeStateContext.AddingIngredient
        },
        {
            AddingRecipeStateContext.AddingIngredient, AddingRecipeStateContext.AddingInstruction
        },
        {
            AddingRecipeStateContext.AddingInstruction, AddingRecipeStateContext.SaveRecipe
        },
        {
            AddingRecipeStateContext.SaveRecipe, AddingRecipeStateContext.ReturnToMenu
        }
    };

    private readonly Dictionary<AddingRecipeStateContext, AddingRecipeStateContext> CanMove = new()
    {
        {
            AddingRecipeStateContext.AddingName, AddingRecipeStateContext.AddingIngredient
        }
    };

    public void NextState(ChatContext context)
    {
        context.State = (int)NextStepFrom[(AddingRecipeStateContext)context.State];
    }

    public void NextState(ChatContext context, AddingRecipeStateContext addingRecipeStateContext)
    {
        if (CanMove[(AddingRecipeStateContext)context.State] == addingRecipeStateContext)
            context.State = (int)NextStepFrom[(AddingRecipeStateContext)context.State];
    }
}