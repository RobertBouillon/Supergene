using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
  public static class Time
  {
    public static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
    public static readonly TimeSpan FiveSeconds = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan TenSeconds = TimeSpan.FromSeconds(10);
    public static readonly TimeSpan ThirtySeconds = TimeSpan.FromSeconds(30);

    public static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
    public static readonly TimeSpan FiveMinutes = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan TenMinutes = TimeSpan.FromMinutes(10);
    public static readonly TimeSpan ThirtyMinutes = TimeSpan.FromMinutes(30);

    public static readonly TimeSpan OneHour = TimeSpan.FromHours(1);
    public static readonly TimeSpan FiveHours = TimeSpan.FromHours(5);
    public static readonly TimeSpan TenHours = TimeSpan.FromHours(10);
    public static readonly TimeSpan ThirtyHours = TimeSpan.FromHours(30);

    public static readonly TimeSpan OneDay = TimeSpan.FromDays(1);
    public static readonly TimeSpan FiveDays = TimeSpan.FromDays(5);
    public static readonly TimeSpan TenDays = TimeSpan.FromDays(10);
    public static readonly TimeSpan ThirtyDays = TimeSpan.FromDays(30);

  }
}
