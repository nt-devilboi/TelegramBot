using CookingBot.Application.Interfaces;
using CookingBot.Commands.AddRecipe.Flow;
using EasyTgBot.Abstract;
using EasyTgBot.Entity;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CookingBot.Tests.AddRecipeFlow;

public class HandleShould
{
    private readonly Mock<IChatContextRepository> _chatContextRepositoryMock;
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly Mock<ITelegramBotClient> _telegramBotClientMock;
    private readonly Application.Commands.AddRecipe.Flow.AddRecipeFlow _flow;
    private readonly ChatContext _chatContext;
    private readonly Update _update;

    public HandleShould()
    {
        // Инициализация моков
        _chatContextRepositoryMock = new Mock<IChatContextRepository>();
        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        _telegramBotClientMock = new Mock<ITelegramBotClient>();

        // Создаем экземпляр класса под тест
        _flow = new Application.Commands.AddRecipe.Flow.AddRecipeFlow(
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