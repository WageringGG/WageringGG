﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using stellar_dotnet_sdk;
using System;
using System.Collections.Generic;

namespace WageringGG.Server
{
    public static class Extensions
    {
        public static IEnumerable<string> GetErrors(this ModelStateDictionary state)
        {
            List<string> errors = new List<string>();
            foreach (var entry in state.Values)
            {
                foreach (var error in entry.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }
            return errors;
        }

        public static bool WithinTimeBounds(this TimeBounds bounds)
        {
            DateTime date = DateTime.Now;
            if (bounds.MinTime < date.Ticks && bounds.MaxTime > date.Ticks)
                return true;
            return false;
        }
    }
}
