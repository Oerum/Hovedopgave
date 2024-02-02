using System.Data;
using gpt.application.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace gpt.application.Implementation;

public class GptImplementation : IGptImplementation
{
    private readonly ILogger<GptImplementation> _logger;
    private readonly IConfiguration _configuration;
    private readonly IGptRepository _gpt;

    public GptImplementation(ILogger<GptImplementation> logger, IConfiguration configuration, IGptRepository gpt)
    {
        _logger = logger;
        _configuration = configuration;
        _gpt = gpt;
    }

    async Task<string> IGptImplementation.UpdateFtModel(int amountToCollect)
    {
        try
        {
            return await _gpt.UpdateFtModel(amountToCollect);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Model Update Error: {ErrorMessage}", ex.Message);
            throw new Exception("Model Update Error");
        }
    }

    async Task<string> IGptImplementation.Gpt(string question)
    {
        try
        {
            return await _gpt.Gpt(question);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GPT Implementation Error: {ErrorMessage}", ex.Message);
            throw new Exception($"AI Error: {ex.Message}");
        }
    }
}