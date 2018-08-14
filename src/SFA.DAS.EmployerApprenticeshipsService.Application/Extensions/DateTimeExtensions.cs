﻿using System;

namespace SFA.DAS.EmployerCommitments.Application.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime FirstOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }
    }
}