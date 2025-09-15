using CookingBot.Application.Commands.AddRecipe;
using CookingBot.Application.Flows.WantToCook;
using CookingBot.Domain.Entity;
using CookingBot.Domain.Payloads;
using CookingBot.Infrastructure.Repositories;
using EasyTgBot.Entity;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Chat = EasyTgBot.Entity.Chat;


[TestFixture]
public class WantToCookTests
{
    private Mock<RecipeRepository> _mockRecipeRepository;
    private Mock<ITelegramBotClient> _mockBot;
    private WantToCook _command;
    private Update _update;
    private ChatContext _context;

    [SetUp]
    public void Setup()
    {
        // Создаем моки
        _mockRecipeRepository = new Mock<RecipeRepository>();
        _mockBot = new Mock<ITelegramBotClient>();

        // Создаем команду
        _command = new WantToCook(_mockRecipeRepository.Object);

        // Создаем тестовый Update
        _update = new Update
        {
            Message = new Message
            {
                Chat = new Telegram.Bot.Types.Chat { Id = 12345 },
                Text = typeof(WantToCook).GetProperty("Trigger").GetValue(_command).ToString(),
                MessageId = 1
            }
        };

        // Создаем тестовый ChatContext (предполагая, что у вас есть конструктор или способ создания)
        _context = new ChatContext(); // Или Mock<ChatContext> если это интерфейс
    }


    [Test]
    public void GetRecipes_If_TheyExist()
    {
        var recipe = CreateFake();
        
        
        
    }


    Recipe CreateFake()
    {
        return new Recipe
        {
            Id = Guid.NewGuid(),
            nameRecipe = "Тестовый рецепт",
            Ingredients = new Dictionary<string, IngredientDetail>
            {
                ["Мясо"] = new IngredientDetail(500, "г"),
                ["Лук"] = new IngredientDetail(1, "шт")
            },
            Instruction = "1. Нарезать\n2. Готовить\n3. Подавать",
            WasCookedLastTime = DateTime.Now.AddDays(-5),
            ChatId = 12345
        };
    }
}