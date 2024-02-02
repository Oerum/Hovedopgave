namespace gpt.application.Interface;

public interface IGptRepository
{
    Task<string> UpdateFtModel(int amountToCollect);
    Task<string> Gpt(string question);
}