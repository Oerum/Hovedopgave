namespace gpt.application.Interface;

public interface IGptImplementation
{
    Task<string> UpdateFtModel(int amountToCollect);
    Task<string> Gpt(string question);
}