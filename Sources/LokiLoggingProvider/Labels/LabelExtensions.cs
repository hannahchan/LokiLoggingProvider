namespace LokiLoggingProvider.Labels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging;

    internal static class LabelExtensions
    {
        public static void ValidateAndThrow(this StaticLabelOptions options)
        {
            string[] reservedLabels = Labels.All;

            foreach (string reservedLabel in reservedLabels)
            {
                if (options.AdditionalStaticLabels.TryGetValue(reservedLabel, out _))
                {
                    throw new InvalidOperationException($"Reserved labels cannot be added to '{nameof(options.AdditionalStaticLabels)}'.");
                }
            }
        }

        public static IReadOnlyDictionary<string, string> ToReadOnlyDictionary(this StaticLabelOptions options)
        {
            options.ValidateAndThrow();

            Dictionary<string, string> labels = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(options.JobName))
            {
                labels.Add(Labels.Job, options.JobName);
            }

            if (options.IncludeInstanceLabel)
            {
                labels.Add(Labels.Instance, Environment.MachineName);
            }

            foreach (KeyValuePair<string, object?> label in options.AdditionalStaticLabels)
            {
                labels.Add(label.Key, label.Value?.ToString() ?? string.Empty);
            }

            return new ReadOnlyDictionary<string, string>(labels);
        }

        public static IReadOnlyDictionary<string, string> AddDynamicLables(
            this IReadOnlyDictionary<string, string> staticLabels,
            DynamicLabelOptions options,
            string categoryName,
            LogLevel logLevel,
            EventId eventId,
            Exception? exception)
        {
            Dictionary<string, string> labels = new Dictionary<string, string>(staticLabels);

            if (options.IncludeCategoryName)
            {
                labels.Add(Labels.CategoryName, categoryName);
            }

            if (options.IncludeLogLevel)
            {
                labels.Add(Labels.LogLevel, logLevel.ToString());
            }

            if (options.IncludeEventId)
            {
                labels.Add(Labels.EventId, eventId.ToString());
            }

            if (options.IncludeException && exception != null)
            {
                labels.Add(Labels.Exception, exception.GetType().ToString());
            }

            return new ReadOnlyDictionary<string, string>(labels);
        }

        public static string ToLabelString(this IEnumerable<KeyValuePair<string, string>> labels)
        {
            IEnumerable<string> keyValuePairs = labels.Select(keyValuePair => $"{keyValuePair.Key}=\"{keyValuePair.Value}\"");
            return $"{{{string.Join(",", keyValuePairs)}}}";
        }
    }
}
