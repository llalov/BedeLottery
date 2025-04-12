namespace BedeLottery.Client.Services
{
    public class ConsoleService : IConsoleService
    {
        public void WriteLine(string message = "") => Console.WriteLine(message);

        public string? ReadLine() => Console.ReadLine();
    }
}
