using System.ComponentModel.DataAnnotations.Schema;
using CookingBot.Domain.Payloads;

namespace CookingBot.Domain.Entity;

public class Recipe
{
    public Guid Id { get; set; }

    public string nameRecipe { get; set; }

    [Column(TypeName = "jsonb")]
    
    public Dictionary<string, IngredientDetail> Ingredients { get; init; }

    public string Instruction { get; set; }

    public long ChatId { get; set; }
}