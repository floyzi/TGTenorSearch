using TGTenorSearch;

public static class Program
{
    static Bot? Bot;
    public static async Task Main(string[] args)
    {
        Bot = new();

        await Bot.Launch("");

        await Task.Delay(-1);
    }
}