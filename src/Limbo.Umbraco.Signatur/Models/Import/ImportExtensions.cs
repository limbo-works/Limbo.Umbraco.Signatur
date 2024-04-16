using System;
using System.Diagnostics;

#pragma warning disable 1591

// ReSharper disable once InconsistentNaming

namespace Limbo.Umbraco.Signatur.Models.Import;

public static class ImportExtensions {

    public static T Start<T>(this T item) where T : ImportTask {
        item.Stopwatch = Stopwatch.StartNew();
        return item;
    }

    public static T AppendToMessage<T>(this T item, string line) where T : ImportTask {
        item.Message = (item.Message + Environment.NewLine + line).Trim();
        return item;
    }

    public static T SetAction<T>(this T item, ImportAction action) where T : ImportTask {
        item.Action = action;
        return item;
    }

    public static T Stop<T>(this T item) where T : ImportTask {
        item.Stopwatch?.Stop();
        return item;
    }

    public static T StopWithTime<T>(this T item) where T : ImportTask {
        if (item.Stopwatch == null) return item;
        item.Stopwatch.Stop();
        item.Duration = item.Stopwatch.Elapsed;
        return item;
    }

    public static T SetStatus<T>(this T item, ImportStatus status) where T : ImportTask {
        item.Status = status;
        return item;
    }

    public static T SetStatusWithTime<T>(this T item, ImportStatus status) where T : ImportTask {

        if (item.Stopwatch != null) {
            item.Stopwatch.Stop();
            item.Duration = item.Stopwatch.Elapsed;
        }

        item.Status = status;

        return item;

    }

    /// <summary>
    /// Sets the task status as completed. If the task has already been marked as failed, the status will not be modified.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="item">The item.</param>
    /// <returns>The input item - useful for method chaining.</returns>
    public static T Completed<T>(this T item) where T : ImportTask {

        if (item.Stopwatch != null) {
            item.Stopwatch.Stop();
            item.Duration = item.Stopwatch.Elapsed;
        }

        if (item.Status != ImportStatus.Failed) item.Status = ImportStatus.Completed;

        return item;

    }

    /// <summary>
    /// Sets the status of the item to <see cref="ImportStatus.Failed"/>.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="item">The item.</param>
    /// <returns>The input item - useful for method chaining.</returns>
    public static T Failed<T>(this T item) where T : ImportTask {
        return Failed(item, null);
    }

    /// <summary>
    /// Sets the status of the item to <see cref="ImportStatus.Failed"/>.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="item">The item.</param>
    /// <param name="ex">Any exception that caused the task to fail.</param>
    /// <returns>The input item - useful for method chaining.</returns>
    public static T Failed<T>(this T item, Exception? ex) where T : ImportTask {

        if (item.Stopwatch != null) {
            item.Stopwatch.Stop();
            item.Duration = item.Stopwatch.Elapsed;
        }

        item.Status = ImportStatus.Failed;

        item.Exception = ex;

        item.Parent?.Failed();

        return item;

    }

    public static ImportTask Aborted(this ImportTask item) {

        if (item.Stopwatch != null) {
            item.Stopwatch.Stop();
            item.Duration = item.Duration;
        }

        if (item.Status != ImportStatus.Failed) item.Status = ImportStatus.Aborted;

        foreach (ImportTask child in item.Items) Aborted(child);

        return item;

    }

    public static T Aborted<T>(this T item) where T : ImportTask {

        if (item.Stopwatch != null) {
            item.Stopwatch.Stop();
            item.Duration = item.Duration;
        }

        if (item.Status != ImportStatus.Failed) item.Status = ImportStatus.Aborted;

        foreach (ImportTask child in item.Items) Aborted(child);

        return item;

    }

}