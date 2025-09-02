namespace TgBot;

public record AuthWebSiteSettings(
    string ClientSecretVk)
{
    public const string Scope = "friends";
    private static readonly string ClientIdEnv = "CLIENT_ID";
    private static readonly string ClientSecretEnv = "CLIENT_SECRET_BOT";
    private static readonly string ClientSecretGitHubEnv = "GIT_CLIENT_SECRET";

    public static AuthWebSiteSettings FromEnv()
    {
        var vk = GetEnvVariable(ClientSecretEnv);

        return new AuthWebSiteSettings(vk);
    }

    private static string GetEnvVariable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name)
                    ?? Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);

        if (value == null)
            throw new InvalidOperationException($"env variable '{name}' not found");
        return value;
    }
}