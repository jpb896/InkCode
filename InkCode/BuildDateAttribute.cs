﻿using System;
using System.Globalization;

namespace InkCode
{
    [AttributeUsage(AttributeTargets.Assembly)]
    internal class BuildDateAttribute : Attribute
    {
        public BuildDateAttribute(string value)
        {
            DateTime = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public DateTime DateTime { get; }
    }
}
