using Database.Application.Interface;
using Quartz;

namespace HostService.HostService
{
    [DisallowConcurrentExecution]
    public class PurgeService : IJob
    {
        private readonly IDiscordBotCleanupImplementation _cleanUp;
        public PurgeService(IDiscordBotCleanupImplementation cleanup)
        {
            _cleanUp = cleanup;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync("Executing background Cleanup job");
            await _cleanUp.CleanUp();
        }
    }
}
