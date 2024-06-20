using System.Text.RegularExpressions;

namespace DiscordBot.Domain;

public class HwidDomain
{
    public async Task<bool> HwidChecker(string hwid)
    {
        string pattern = @"^[0-9A-Za-z]{4}-[0-9A-Za-z]{4}-[0-9A-Za-z]{4}-[0-9A-Za-z]{4}-[0-9A-Za-z]{4}-[0-9A-Za-z]{4}-[0-9A-Za-z]{4}-[0-9A-Za-z]{4}$";

        if (Regex.IsMatch(hwid, pattern))
        {
            return await Task.FromResult(true);
        }
        else
        {
            throw new Exception($"HWID Input: `{hwid}` doesn't match required format!"
                                + "\nYou'll find the correct HWID at the top of `A console` when you initially load it.");
        }
    }
}