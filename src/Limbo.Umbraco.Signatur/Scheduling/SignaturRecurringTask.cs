using System;
using System.Text;
using System.Threading.Tasks;
using Limbo.Umbraco.Signatur.Models.Import;
using Limbo.Umbraco.Signatur.Models.Settings;
using Limbo.Umbraco.Signatur.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Time;
using Skybrud.Essentials.Umbraco.Scheduling;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.HostedServices;

namespace Limbo.Umbraco.Signatur.Scheduling;

public class SignaturRecurringTask : RecurringHostedServiceBase {

    private readonly SignaturSettings _settings;
    private readonly SignaturJobsService _signaturJobsService;
    private readonly TaskHelper _taskHelper;

    public SignaturRecurringTask(ILogger<SignaturRecurringTask> logger, IOptions<SignaturSettings> settings, SignaturJobsService signaturJobsService, TaskHelper taskHelper) : base(logger, settings.Value.Scheduling.Interval, settings.Value.Scheduling.Delay) {
        _settings = settings.Value;
        _signaturJobsService = signaturJobsService;
        _taskHelper = taskHelper;
    }

    public override Task PerformExecuteAsync(object? state) {

        // Don't do anything if the site is not running.
        if (_taskHelper.RuntimeLevel != RuntimeLevel.Run) return Task.CompletedTask;

        // TODO: verify that the task should actually run on this server/application

        StringBuilder sb = new();
        sb.AppendLine(EssentialsTime.Now.Iso8601);

        //if (!_taskHelper.ShouldRun(this, _importSettings.ImportInterval)) {
        //    sb.AppendLine("> Exiting as not supposed to run yet.");
        //    _taskHelper.AppendToLog(this, sb);
        //    return Task.CompletedTask;
        //}

        foreach (SignaturFeedSettings feed in _settings.Feeds) {

            // Write a bit to the log
            sb.AppendLine($"> Starting import from feed with URL '{feed.Url}'...");

            // Run a new import
            ImportJob result = _signaturJobsService.Import(feed);

            // Save the result to the disk
            if (_settings.LogResults) _signaturJobsService.WriteToLog(result);

            // Write a bit to the log
            sb.AppendLine($"> Import finished with status {result.Status}.");
            _taskHelper.AppendToLog(this, sb);

        }

        // Make sure we save that the job has run
        _taskHelper.SetLastRunTime(this);

        return Task.CompletedTask;

    }

}