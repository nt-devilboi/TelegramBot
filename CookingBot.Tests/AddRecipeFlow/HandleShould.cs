using CookingBot.Application.Flows.AddRecipe.InContexts.ContextHandlers;
using CookingBot.Application.Interfaces;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Tests.AddRecipeFlow;

public class HandleShould
{
    private readonly Mock<IContextRepository> _chatContextRepositoryMock;
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly Mock<ITelegramBotClient> _telegramBotClientMock;
    private readonly RecipeSetName _setName;
    private readonly ChatContext _chatContext;
    private readonly Update _update;

    public HandleShould()
    {
        // Инициализация моков
        _chatContextRepositoryMock = new Mock<IContextRepository>();
        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        _telegramBotClientMock = new Mock<ITelegramBotClient>();

        // Создаем экземпляр класса под тест
        _setName = new RecipeSetName(
            _chatContextRepositoryMock.Object,
            _recipeRepositoryMock.Object
        );

        // Фейковый ChatContext

        _chatContext = new ChatContext
        {
            ChatId = 12345,
            Payload = null,
            State = 1
        };

        // Фейковый Update
        _update = new Update
        {
            Message = new Message
            {
                Chat = new Telegram.Bot.Types.Chat { Id = 12345 },
                Text = "Тест"
            }
        };
    }
    
    
    
}