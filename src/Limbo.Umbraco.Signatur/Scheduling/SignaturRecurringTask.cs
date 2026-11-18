using System.Threading.Tasks;
using Limbo.Umbraco.Signatur.Models.Import;
using Limbo.Umbraco.Signatur.Models.Settings;
using Limbo.Umbraco.Signatur.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.HostedServices;

namespace Limbo.Umbraco.Signatur.Scheduling;

public class SignaturRecurringTask : RecurringHostedServiceBase {

    private readonly SignaturSettings _settings;
    private readonly SignaturJobsService _signaturJobsService;
    private readonly IRuntimeState _runtimeState;
    private readonly IServerRoleAccessor _serverRoleAccessor;

    public SignaturRecurringTask(ILogger<SignaturRecurringTask> logger, IOptions<SignaturSettings> settings, SignaturJobsService signaturJobsService, IRuntimeState runtimeState, IServerRoleAccessor serverRoleAccessor) : base(logger, settings.Value.Scheduling.Interval, settings.Value.Scheduling.Delay) {
        _settings = settings.Value;
        _signaturJobsService = signaturJobsService;
        _runtimeState = runtimeState;
        _serverRoleAccessor = serverRoleAccessor;
    }

    public override Task PerformExecuteAsync(object? state) {

        // Don't do anything if the site is not running.
        if (_runtimeState.Level != RuntimeLevel.Run) return Task.CompletedTask;

        // Only run if server role is not Subscriber
        if (_serverRoleAccessor.CurrentServerRole == ServerRole.Subscriber) return Task.CompletedTask;

        foreach (SignaturFeedSettings feed in _settings.Feeds) {

            // Run a new import
            ImportJob result = _signaturJobsService.Import(feed, true);

            // Save the result to the disk
            if (_settings.LogResults) _signaturJobsService.WriteToLog(result);

        }

        return Task.CompletedTask;

    }

}