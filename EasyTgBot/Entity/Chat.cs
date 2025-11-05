namespace EasyTgBot.Entity;

public class Chat // это не проблема, но если мне вдруг захочется создать сайт и уйти от бота - это будет немного мешать. кароче у этой сущности нету автономности. опять таки это просто факт через место для изменения
{
    public required long Id { get; init; }
    public required string Token { get; init; }
    
    public required string Name { get; init; }
}