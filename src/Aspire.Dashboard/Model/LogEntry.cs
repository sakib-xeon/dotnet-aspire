// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Aspire.Dashboard.Model;

internal sealed partial class LogEntry
{
    private static readonly Regex s_rfc3339NanoRegEx = GenerateRfc3339NanoRegEx();

    public string? Content { get; set; }
    public string? Timestamp { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter<LogEntryType>))]
    public LogEntryType Type { get; init; } = LogEntryType.Default;

    public static LogEntry Create(string s, LogEntryType type)
    {
        var indexOfSpace = s.IndexOf(' ');
        var possibleTimestamp = s[..indexOfSpace];

        // For right now, we only support RFC3339Nano timestamps, which is what (most) containers generate
        // We can tweak this once project/executable log timestamps are finalized
        if (s_rfc3339NanoRegEx.IsMatch(possibleTimestamp))
        {
            return new() { Timestamp = possibleTimestamp, Content = s[(indexOfSpace + 1)..], Type = type };
        }
        else
        {
            return new() { Content = s, Type = type };
        }
    }

    // Regular Expression for an RFC3339 timestamp, including RFC3339Nano
    //
    // Example timestamps:
    // 2023-10-02T12:56:35.123456789Z
    // 2023-10-02T13:56:35.123456789+10:00
    // 2023-10-02T13:56:35.123456789-10:00
    // 2023-10-02T13:56:35.123456789Z10:00
    // 2023-10-02T13:56:35.123456Z
    // 2023-10-02T13:56:35Z
    //
    // Explanation:
    // ^                                                   - Starts the string
    // (?:\\d{4})                                          - Four digits for the year
    // -                                                   - Separator for the date
    // (?:0[1-9]|1[0-2])                                   - Two digits for the month, restricted to 01-12
    // -                                                   - Separator for the date
    // (?:0[1-9]|[12][0-9]|3[01])                          - Two digits for the day, restricted to 01-31
    // 'T'                                                 - Literal, separator between date and time
    // (?:[01][0-9]|2[0-3])                                - Two digits for the hour, restricted to 00-23
    // :                                                   - Separator for the time
    // (?:[0-5][0-9])                                      - Two digits for the minutes, restricted to 00-59
    // :                                                   - Separator for the time
    // (?:[0-5][0-9])                                      - Two digits for the seconds, restricted to 00-59
    // (?:\\.\\d{1,9})                                     - A period and up to nine digits for the partial seconds
    // Z                                                   - Literal, same as +00:00
    // (?:[Z+-](?:[01][0-9]|2[0-3]):(?:[0-5][0-9]))        - Time Zone offset, in the form ZHH:MM or +HH:MM or -HH:MM
    // $                                                   - Ends the string
    //
    // Note: (?:) is a non-capturing group, since we don't care about the values, we are just interested in whether or not there is a match
    [GeneratedRegex("^(?:\\d{4})-(?:0[1-9]|1[0-2])-(?:0[1-9]|[12][0-9]|3[01])T(?:[01][0-9]|2[0-3]):(?:[0-5][0-9]):(?:[0-5][0-9])(?:\\.\\d{1,9})?(?:Z|(?:[Z+-](?:[01][0-9]|2[0-3]):(?:[0-5][0-9])))?$")]
    private static partial Regex GenerateRfc3339NanoRegEx();
}

internal enum LogEntryType
{
    Default,
    Error,
    Warning
}