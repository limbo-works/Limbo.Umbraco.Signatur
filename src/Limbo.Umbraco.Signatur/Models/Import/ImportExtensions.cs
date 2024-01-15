using System;
using System.Diagnostics;

#pragma warning disable 1591

// ReSharper disable once InconsistentNaming

namespace Limbo.Umbraco.Signatur.Models.Import {

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

        public static T Completed<T>(this T item) where T : ImportTask {
            return item.SetStatusWithTime(ImportStatus.Completed);
        }

        public static T Completed<T>(this T item, ImportAction action) where T : ImportTask {
            return item.SetStatusWithTime(ImportStatus.Completed).SetAction(action);
        }

        public static T Failed<T>(this T item) where T : ImportTask {
            return Failed(item, null);
        }

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

        public static T Aborted<T>(this T item) where T : ImportTask {

            if (item.Stopwatch != null) {
                item.Stopwatch.Stop();
                item.Duration = item.Duration;
            }

            item.Status = ImportStatus.Aborted;

            return item;

        }

    }

}