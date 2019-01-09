﻿// <copyright file="Iso8601Duration.cs" company="Nate Barbettini">
// Copyright (c) 2015. Licensed under MIT.
// </copyright>

namespace SimpleDuration
{
    using System;
    using System.Text;

    /// <summary>
    /// Contains methods useful for interacting with ISO 8601 durations.
    /// </summary>
    public static class Iso8601Duration
    {
        /// <summary>
        /// Converts the ISO 8601 string representation of a duration to its equivalent
        /// .NET <see cref="TimeSpan"/> representation.
        /// A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <remarks>
        /// The year (Y) and month (M) designators are currently unsupported. If these
        /// designators are encountered, the method will return <see langword="false"/>.
        /// <para>
        /// These are problematic because of things like leap years, Daylight Savings Time, etc.
        /// More information is needed in order to convert to a <see cref="TimeSpan"/>;
        /// for that, a more powerful library such as NodaTime would be appropriate.
        /// </para>
        /// </remarks>
        /// <param name="duration">The ISO 8601 duration, excluding year (Y) and month (M) designators.
        /// </param>
        /// <param name="timeSpan">
        /// When the method returns, this parameter is set to the resulting <see cref="TimeSpan"/>
        /// if the conversion was successful, or <c>default(TimeSpan)</c> if the conversion was unsuccessful.</param>
        /// <returns><see langword="true"/> if the conversion succeeded; <see langword="false"/> otherwise.</returns>
        public static bool TryParse(string duration, out TimeSpan timeSpan)
        {
            timeSpan = default(TimeSpan);

            var result = DurationVisitor.Parse(duration);
            if (result.Valid)
            {
                timeSpan = ConvertResult(result);
            }

            return result.Valid;
        }

        /// <summary>
        /// Converts the ISO 8601 string representation of a duration to its equivalent
        /// .NET <see cref="TimeSpan"/> representation.
        /// </summary>
        /// <remarks>
        /// The year (Y) and month (M) designators are currently unsupported. If these
        /// designators are encountered, the method will return <see langword="false"/>.
        /// <para>
        /// These are problematic because of things like leap years, Daylight Savings Time, etc.
        /// More information is needed in order to convert to a <see cref="TimeSpan"/>;
        /// for that, a more powerful library such as NodaTime would be appropriate.
        /// </para>
        /// </remarks>
        /// <param name="duration">The ISO 8601 duration, excluding year (Y) and month (M) designators.</param>
        /// <returns><see langword="true"/> if the conversion succeeded; <see langword="false"/> otherwise.</returns>
        /// <exception cref="FormatException">The ISO 8601 string was not in the proper format.</exception>
        public static TimeSpan Parse(string duration)
        {
            TimeSpan result;

            if (!TryParse(duration, out result))
            {
                throw new FormatException("ISO 8601 string was not in the proper format.");
            }

            return result;
        }

        /// <summary>
        /// Formats a <see cref="TimeSpan"/> as an ISO 8601 duration string.
        /// </summary>
        /// <remarks>
        /// The year (Y) and month (M) designators are currently unsupported. The largest
        /// designator this method will return is weeks (W).
        /// <para>
        /// These are problematic because of things like leap years, Daylight Savings Time, etc.
        /// More information is needed in order to convert to a <see cref="TimeSpan"/>;
        /// for that, a more powerful library such as NodaTime would be appropriate.
        /// </para>
        /// </remarks>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>An ISO 8601 string representing the duration in <paramref name="timeSpan"/>.</returns>
        public static string Format(TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero)
            {
                return "PT0S";
            }

            var builder = new StringBuilder("P");

            if (timeSpan.TotalDays > 7)
            {
                var weeks = Math.Floor(timeSpan.TotalDays / 7);
                builder.Append($"{weeks}W");

                timeSpan = timeSpan.Subtract(TimeSpan.FromDays(weeks * 7));
            }

            if (timeSpan.Days > 0)
            {
                builder.Append($"{timeSpan.Days}D");

                timeSpan = timeSpan.Subtract(TimeSpan.FromDays(timeSpan.Days));
            }

            if (timeSpan.TotalSeconds > 0)
            {
                builder.Append("T");
            }

            if (timeSpan.Hours > 0)
            {
                builder.Append($"{timeSpan.Hours}H");

                timeSpan = timeSpan.Subtract(TimeSpan.FromHours(timeSpan.Hours));
            }

            if (timeSpan.Minutes > 0)
            {
                builder.Append($"{timeSpan.Minutes}M");

                timeSpan = timeSpan.Subtract(TimeSpan.FromMinutes(timeSpan.Minutes));
            }

            if (timeSpan.TotalSeconds > 0)
            {
                builder.Append($"{timeSpan.TotalSeconds}S");
            }

            var result = builder.ToString();
            if (result == "P")
            {
                return string.Empty;
            }

            return result;
        }

        private static TimeSpan ConvertResult(DurationVisitor visitor)
        {
            return TimeSpan.Zero
                .Add(TimeSpan.FromDays(visitor.Weeks * 7))
                .Add(TimeSpan.FromDays(visitor.Days))
                .Add(TimeSpan.FromHours(visitor.Hours))
                .Add(TimeSpan.FromMinutes(visitor.Minutes))
                .Add(TimeSpan.FromSeconds(visitor.Seconds));
        }
    }
}
