﻿using System;

namespace BingeBuddyNg.Core
{
    public static class Util
    {
        public static TEnum SafeParseEnum<TEnum>(string value, TEnum defaultValue) where TEnum : struct
        {
            if (Enum.TryParse<TEnum>(value, out TEnum result) == false)
                return defaultValue;

            return result;
        }
    }
}