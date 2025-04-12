namespace BedeLottery.Client.Services
{
    public interface IConsoleService
    {
        void WriteLine(string message = "");
        string? ReadLine();
    }
}
