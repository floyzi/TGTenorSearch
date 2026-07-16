using System.Text.Json;
using TGTenorSearch;
using TGTenorSearch.Models;

public static class Program
{
    public static Config? Config;
    public static DateTime StartedAt;
    static Bot? Bot;

    public static async Task Main(string[] args)
    {
        var confPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        if (!File.Exists(confPath)) throw new FileNotFoundException(confPath);

        Config = JsonSerializer.Deserialize<Config>(File.ReadAllBytes(confPath));

        if (Config == null || string.IsNullOrEmpty(Config.BotToken)) throw new InvalidOperationException("No bot token provided");

        StartedAt = DateTime.UtcNow;

        Bot = new();

        await Bot.Launch(Config.BotToken);

        await Task.Delay(-1);
    }
}