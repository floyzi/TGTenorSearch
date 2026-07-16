using System.Text.Json;
using TGTenorSearch;
using TGTenorSearch.Models;

public static class Program
{
    internal static Config? Config;
    internal static DateTime StartedAt;
    internal static bool IsV1;
    static Bot? Bot;

    public static async Task Main(string[] args)
    {
        var confPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        if (!File.Exists(confPath)) throw new FileNotFoundException(confPath);

        Config = JsonSerializer.Deserialize<Config>(File.ReadAllBytes(confPath)) ?? throw new InvalidOperationException();

        if (string.IsNullOrEmpty(Config.BotToken)) throw new InvalidOperationException("No bot token provided");
        if (string.IsNullOrEmpty(Config.APIKey)) throw new InvalidOperationException("No Tenor API key provided");

        StartedAt = DateTime.UtcNow;
        IsV1 = Config.APIKey.Length <= 12;

        Console.WriteLine($"Using {(IsV1 ? "V1" : "V2")} Tenor API");

        Bot = new();

        await Bot.Launch(Config.BotToken);

        await Task.Delay(-1);
    }
}