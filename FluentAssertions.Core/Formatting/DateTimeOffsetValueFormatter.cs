﻿using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions.Common;

namespace FluentAssertions.Formatting
{
    public class DateTimeOffsetValueFormatter : IValueFormatter
    {
        public DateTimeOffsetValueFormatter()
        {
            TimeZoneOffset = TimeZoneInfo.Local.BaseUtcOffset;
        }

        public DateTimeOffsetValueFormatter(TimeSpan offset)
        {
            TimeZoneOffset = offset;
        }

        public TimeSpan TimeZoneOffset { get; private set; }

        /// <summary>
        /// Indicates whether the current <see cref="IValueFormatter"/> can handle the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value for which to create a <see cref="System.String"/>.</param>
        /// <returns>
        /// <c>true</c> if the current <see cref="IValueFormatter"/> can handle the specified value; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle(object value)
        {
            return (value is DateTime) || (value is DateTimeOffset);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="value">The value for which to create a <see cref="System.String"/>.</param>
        /// <param name="useLineBreaks"> </param>
        /// <param name="processedObjects">
        /// A collection of objects that 
        /// </param>
        /// <param name="nestedPropertyLevel">
        /// The level of nesting for the supplied value. This is used for indenting the format string for objects that have
        /// no <see cref="object.ToString()"/> override.
        /// </param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(object value, bool useLineBreaks, IList<object> processedObjects = null, int nestedPropertyLevel = 0)
        {
            DateTimeOffset dateTime;

            if (value is DateTime)
            {
                dateTime = ((DateTime)value).ToDateTimeOffset();
            }
            else
            {
                dateTime = (DateTimeOffset) value;
            }

            var fragments = new List<string>();

            if (HasDate(dateTime))
            {
                fragments.Add(dateTime.ToString("yyyy-MM-dd"));
            }

            if (HasTime(dateTime))
            {
                string format = (HasMilliSeconds(dateTime)) ? "HH:mm:ss.fff" : "HH:mm:ss";
                fragments.Add(dateTime.ToString(format));
            }

            if (!fragments.Any())
            {
                fragments.Add("0001-01-01 00:00:00.000");
            }

            if (HasDate(dateTime))
            {
                fragments.Add(FormatOffset(dateTime.Offset));
            }

            return "<" + string.Join(" ", fragments.Where(f => !string.IsNullOrWhiteSpace(f))) + ">";
        }

        private string FormatOffset(TimeSpan offset)
        {
            if (offset == TimeZoneOffset)
            {
                return null;
            }

            if (offset == TimeSpan.Zero)
            {
                return "UTC";
            }

            TimeSpan absoluteOffset = (offset < TimeSpan.Zero) ? -offset : offset;

            string formattedOffset;

            if (IsWholeHour(absoluteOffset))
            {
                formattedOffset = absoluteOffset.Hours.ToString();
            }
            else if (IsWholeMinutes(absoluteOffset))
            {
                formattedOffset = string.Format("{0}:{1:00}", absoluteOffset.Hours, absoluteOffset.Minutes);
            }
            else
            {
                throw new ArgumentException("Offset is supposed to be in whole minutes");
            }

            string sign = (offset < TimeSpan.Zero) ? "-" : "+";
            return "UTC" + sign + formattedOffset;
        }

        private static bool IsWholeHour(TimeSpan absoluteOffset)
        {
            return absoluteOffset.Ticks == (absoluteOffset.Hours * TimeSpan.TicksPerHour);
        }

        private static bool IsWholeMinutes(TimeSpan absoluteOffset)
        {
            return absoluteOffset.Ticks == 
                ((absoluteOffset.Hours * TimeSpan.TicksPerHour) + (absoluteOffset.Minutes * TimeSpan.TicksPerMinute));
        }

        private static bool HasTime(DateTimeOffset dateTime)
        {
            return (dateTime.Hour != 0) || (dateTime.Minute != 0) || (dateTime.Second != 0);
        }

        private static bool HasDate(DateTimeOffset dateTime)
        {
            return (dateTime.Day != 1) || (dateTime.Month != 1) || (dateTime.Year != 1);
        }

        private static bool HasMilliSeconds(DateTimeOffset dateTime)
        {
            return (dateTime.Millisecond > 0);
        }
    }
}