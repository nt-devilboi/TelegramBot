using CookingBot.Application.Interfaces;
using CookingBot.Infrastructure.DataBase;
using EasyTgBot.Abstract;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Vostok.Logging.Abstractions;
using Chat = EasyTgBot.Entity.Chat;

namespace CookingBot.Application.Flows.Friends.InContext;

public class CheckFriendRecipe(
    IFriendRepository friendRepository,
    ITelegramBotClient botClient,
    IRecipeRepository recipeRepository,
    IChatRepository chatRepository,
    ILog log)
    : ContextHandler<BasePayload, FriendsContext>
{
    protected override async Task Handle(Update update, DetailContext<BasePayload, FriendsContext> context)
    {
        if (long.TryParse(update.CallbackQuery?.Data, out var id))
        {
            var recipe = await recipeRepository.GetByChatId(id);

            await botClient.SendTextMessageAsync(context.ChatId, $"Последний рецепт готовил: {recipe[0].nameRecipe}");
        }
    }

    protected override async Task Enter(DetailContext<BasePayload, FriendsContext> context)
    {
        var friendslist = await friendRepository.GetFriends(context.ChatId);
        var friendData = new Chat[friendslist.Count];
        log.Debug($"Friends {context.ChatId}:\n");
        for (var index = 0; index < friendslist.Count; index++)
        {
            friendData[index] = await chatRepository.Get(friendslist[index].FriendId);
            log.Debug($"{friendslist[index].FriendId}");
        }

        await botClient.SendTextMessageAsync(context.ChatId, "Чьи рецепты хочешь посмотреть",
            replyMarkup: ButtonFriends(friendData));
    }

    private InlineKeyboardMarkup ButtonFriends(Chat[] friends)
    {
        // формируем кнопки из списка друзей

        var rows = friends
            .Select(f => new[]
                { InlineKeyboardButton.WithCallbackData(f.Name, $"{f.Id}") }) // каждая кнопка в отдельной строке
            .ToArray();

        return new InlineKeyboardMarkup(rows);
    }
}

public interface IFriendRepository
{
    Task<List<Friend>> GetFriends(long chatId);
    Task Add(Friend result);
}

public class FriendRepository(ChatTelegramDb chatTelegramDb) : IFriendRepository
{
    public async Task<List<Friend>> GetFriends(long chatId)
    {
        return await chatTelegramDb.Friends.Where(x => x.UserId == chatId).ToListAsync();
    }

    public async Task Add(Friend friend)
    {
        await chatTelegramDb.Friends.AddAsync(friend);
        await chatTelegramDb.SaveChangesAsync();
    }
}

public class Friend
{
    public long UserId { get; init; }
    public long FriendId { get; init; }
}