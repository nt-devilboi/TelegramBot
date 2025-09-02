using EasyTgBot.Entity;
using Newtonsoft.Json;
using TgBot.Domain.Entity;
using TgBot.StatePayload;
using JsonException = System.Text.Json.JsonException;

namespace TgBot.Commands.AddRecipe.Flow;

public class RecipeContext
{
    public const int AddingName = 10000;
    public const int AddingIngredient = 10001;
    public bool WasChanged;

    private RecipeContext(ChatContext chatContext, AddRecipePayload? payload)
    {
        ChatContext = chatContext;
        Payload = payload;
    }

    public int State => ChatContext.State;

    public AddRecipePayload? Payload { get; private set; }
    public ChatContext ChatContext { get; }

    public void NextState()
    {
        ChatContext.State++;
        WasChanged = true;
    }

    public void SetPayload(AddRecipePayload payload)
    {
        Payload = payload;

        ChatContext.Payload = JsonConvert.SerializeObject(payload);
    }

    public static RecipeContext Create(ChatContext context)
    {
        if (string.IsNullOrEmpty(context.Payload)) return new RecipeContext(context, null);

        var payload = JsonConvert.DeserializeObject<AddRecipePayload>(context.Payload);
        if (payload == null) throw new JsonException($"payload: {context.Payload} not {typeof(RecipeContext)}");

        return new RecipeContext(context, payload);
    }

    public void SaveChanges()
    {
        ChatContext.Payload = JsonConvert.SerializeObject(Payload);
        WasChanged = true;
    }
}