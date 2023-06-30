using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConVar;
using Cronos;
using Facepunch;
using Facepunch.Math;
using Newtonsoft.Json;
using ProtoBuf;
using TimeZoneConverter;
using UnityEngine;

// Token: 0x02000432 RID: 1074
public class WipeTimer : global::BaseEntity
{
	// Token: 0x06002459 RID: 9305 RVA: 0x000E7C5F File Offset: 0x000E5E5F
	public override void InitShared()
	{
		base.InitShared();
		if (base.isServer)
		{
			WipeTimer.serverinstance = this;
		}
		if (base.isClient)
		{
			WipeTimer.clientinstance = this;
		}
	}

	// Token: 0x0600245A RID: 9306 RVA: 0x000E7C83 File Offset: 0x000E5E83
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			WipeTimer.serverinstance = null;
		}
		if (base.isClient)
		{
			WipeTimer.clientinstance = null;
		}
		base.DestroyShared();
	}

	// Token: 0x0600245B RID: 9307 RVA: 0x000E7CA7 File Offset: 0x000E5EA7
	public override void ServerInit()
	{
		base.ServerInit();
		this.RecalculateWipeFrequency();
		base.InvokeRepeating(new Action(this.TryAndUpdate), 1f, 4f);
	}

	// Token: 0x0600245C RID: 9308 RVA: 0x000E7CD4 File Offset: 0x000E5ED4
	public void RecalculateWipeFrequency()
	{
		string tags = Server.tags;
		if (tags.Contains("monthly"))
		{
			this.wipeFrequency = WipeTimer.WipeFrequency.Monthly;
			return;
		}
		if (tags.Contains("biweekly"))
		{
			this.wipeFrequency = WipeTimer.WipeFrequency.BiWeekly;
			return;
		}
		if (tags.Contains("weekly"))
		{
			this.wipeFrequency = WipeTimer.WipeFrequency.Weekly;
			return;
		}
		this.wipeFrequency = WipeTimer.WipeFrequency.Monthly;
	}

	// Token: 0x0600245D RID: 9309 RVA: 0x000E7D2D File Offset: 0x000E5F2D
	public void TryAndUpdate()
	{
		if (Server.tags != this.oldTags)
		{
			this.RecalculateWipeFrequency();
			this.oldTags = Server.tags;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600245E RID: 9310 RVA: 0x000E7D5C File Offset: 0x000E5F5C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk && info.msg.landmine == null)
		{
			info.msg.landmine = Facepunch.Pool.Get<ProtoBuf.Landmine>();
			info.msg.landmine.triggeredID = (ulong)this.GetTicksUntilWipe();
		}
	}

	// Token: 0x0600245F RID: 9311 RVA: 0x000E7DAC File Offset: 0x000E5FAC
	public TimeSpan GetTimeSpanUntilWipe()
	{
		DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow.AddDays((double)WipeTimer.daysToAddTest).AddHours((double)WipeTimer.hoursToAddTest);
		return this.GetWipeTime(dateTimeOffset) - dateTimeOffset;
	}

	// Token: 0x06002460 RID: 9312 RVA: 0x000E7DE8 File Offset: 0x000E5FE8
	public long GetTicksUntilWipe()
	{
		return this.GetTimeSpanUntilWipe().Ticks;
	}

	// Token: 0x06002461 RID: 9313 RVA: 0x000E7E04 File Offset: 0x000E6004
	[ServerVar]
	public static void PrintWipe(ConsoleSystem.Arg arg)
	{
		if (WipeTimer.serverinstance == null)
		{
			arg.ReplyWith("WipeTimer not found!");
			return;
		}
		WipeTimer.serverinstance.RecalculateWipeFrequency();
		WipeTimer.serverinstance.TryAndUpdate();
		TimeZoneInfo timeZone = WipeTimer.GetTimeZone();
		string text2;
		string text = (TZConvert.TryWindowsToIana(timeZone.Id, out text2) ? text2 : timeZone.Id);
		DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow.AddDays((double)WipeTimer.daysToAddTest).AddHours((double)WipeTimer.hoursToAddTest);
		DateTimeOffset wipeTime = WipeTimer.serverinstance.GetWipeTime(dateTimeOffset);
		TimeSpan timeSpan = wipeTime - dateTimeOffset;
		string cronString = WipeTimer.GetCronString(WipeTimer.serverinstance.wipeFrequency, (int)(WipeTimer.serverinstance.useWipeDayOverride ? WipeTimer.serverinstance.wipeDayOfWeekOverride : ((DayOfWeek)WipeTimer.wipeDayOfWeek)), WipeTimer.wipeHourOfDay);
		CronExpression cronExpression = WipeTimer.GetCronExpression(WipeTimer.serverinstance.wipeFrequency, (int)(WipeTimer.serverinstance.useWipeDayOverride ? WipeTimer.serverinstance.wipeDayOfWeekOverride : ((DayOfWeek)WipeTimer.wipeDayOfWeek)), WipeTimer.wipeHourOfDay);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Format("Frequency: {0}", WipeTimer.serverinstance.wipeFrequency));
		stringBuilder.AppendLine(string.Concat(new string[] { "Timezone: ", timeZone.StandardName, " (ID=", timeZone.Id, ", IANA=", text, ")" }));
		stringBuilder.AppendLine(string.Format("Wipe day of week: {0}", (DayOfWeek)WipeTimer.wipeDayOfWeek));
		stringBuilder.AppendLine(string.Format("Wipe hour: {0}", WipeTimer.wipeHourOfDay));
		stringBuilder.AppendLine(string.Format("Test time: {0:O}", dateTimeOffset));
		stringBuilder.AppendLine(string.Format("Wipe time: {0:O}", wipeTime));
		stringBuilder.AppendLine(string.Format("Time until wipe: {0:g}", timeSpan));
		stringBuilder.AppendLine(string.Format("Ticks until wipe: {0}", timeSpan.Ticks));
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("Cron: " + cronString);
		stringBuilder.AppendLine("Next 10 occurrences:");
		int num = 0;
		foreach (DateTimeOffset dateTimeOffset2 in cronExpression.GetOccurrences(dateTimeOffset, dateTimeOffset.AddYears(2), timeZone, true, false).Take(10))
		{
			stringBuilder.AppendLine(string.Format("  {0}. {1:O}", num, dateTimeOffset2));
			num++;
		}
		arg.ReplyWith(stringBuilder.ToString());
	}

	// Token: 0x06002462 RID: 9314 RVA: 0x000E80C0 File Offset: 0x000E62C0
	[ServerVar]
	public static void PrintTimeZones(ConsoleSystem.Arg arg)
	{
		List<string> list = (from z in TimeZoneInfo.GetSystemTimeZones()
			select z.Id).ToList<string>();
		IReadOnlyCollection<string> knownWindowsTimeZoneIds = TZConvert.KnownWindowsTimeZoneIds;
		IReadOnlyCollection<string> knownIanaTimeZoneNames = TZConvert.KnownIanaTimeZoneNames;
		arg.ReplyWith(JsonConvert.SerializeObject(new
		{
			systemTzs = list,
			windowsTzs = knownWindowsTimeZoneIds,
			ianaTzs = knownIanaTimeZoneNames
		}));
	}

	// Token: 0x06002463 RID: 9315 RVA: 0x000E811C File Offset: 0x000E631C
	public DateTimeOffset GetWipeTime(DateTimeOffset nowTime)
	{
		if (WipeTimer.wipeUnixTimestampOverride > 0L && WipeTimer.wipeUnixTimestampOverride > (long)Epoch.Current)
		{
			return Epoch.ToDateTime(WipeTimer.wipeUnixTimestampOverride);
		}
		DateTimeOffset dateTimeOffset;
		try
		{
			dateTimeOffset = WipeTimer.GetCronExpression(this.wipeFrequency, (int)(this.useWipeDayOverride ? this.wipeDayOfWeekOverride : ((DayOfWeek)WipeTimer.wipeDayOfWeek)), WipeTimer.wipeHourOfDay).GetNextOccurrence(nowTime, WipeTimer.GetTimeZone(), false) ?? DateTimeOffset.MaxValue;
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
			dateTimeOffset = DateTimeOffset.MaxValue;
		}
		return dateTimeOffset;
	}

	// Token: 0x06002464 RID: 9316 RVA: 0x000E81BC File Offset: 0x000E63BC
	private static CronExpression GetCronExpression(WipeTimer.WipeFrequency frequency, int dayOfWeek, float hourOfDay)
	{
		string cronString = WipeTimer.GetCronString(frequency, dayOfWeek, hourOfDay);
		if (cronString == WipeTimer.cronExprCacheKey && WipeTimer.cronExprCache != null)
		{
			return WipeTimer.cronExprCache;
		}
		WipeTimer.cronExprCache = CronExpression.Parse(cronString);
		WipeTimer.cronExprCacheKey = cronString;
		return WipeTimer.cronExprCache;
	}

	// Token: 0x06002465 RID: 9317 RVA: 0x000E8208 File Offset: 0x000E6408
	private static string GetCronString(WipeTimer.WipeFrequency frequency, int dayOfWeek, float hourOfDay)
	{
		if (!string.IsNullOrWhiteSpace(WipeTimer.wipeCronOverride))
		{
			return WipeTimer.wipeCronOverride;
		}
		ValueTuple<WipeTimer.WipeFrequency, int, float> valueTuple = new ValueTuple<WipeTimer.WipeFrequency, int, float>(frequency, dayOfWeek, hourOfDay);
		ValueTuple<WipeTimer.WipeFrequency, int, float> valueTuple2 = valueTuple;
		ValueTuple<WipeTimer.WipeFrequency, int, float>? valueTuple3 = WipeTimer.cronCacheKey;
		bool flag;
		if (valueTuple3 == null)
		{
			flag = false;
		}
		else
		{
			ValueTuple<WipeTimer.WipeFrequency, int, float> valueOrDefault = valueTuple3.GetValueOrDefault();
			flag = valueTuple2.Item1 == valueOrDefault.Item1 && valueTuple2.Item2 == valueOrDefault.Item2 && valueTuple2.Item3 == valueOrDefault.Item3;
		}
		if (flag && WipeTimer.cronCache != null)
		{
			return WipeTimer.cronCache;
		}
		WipeTimer.cronCache = WipeTimer.BuildCronString(frequency, dayOfWeek, hourOfDay);
		WipeTimer.cronCacheKey = new ValueTuple<WipeTimer.WipeFrequency, int, float>?(valueTuple);
		return WipeTimer.cronCache;
	}

	// Token: 0x06002466 RID: 9318 RVA: 0x000E82A8 File Offset: 0x000E64A8
	private static string BuildCronString(WipeTimer.WipeFrequency frequency, int dayOfWeek, float hourOfDay)
	{
		int num = Mathf.FloorToInt(hourOfDay);
		int num2 = Mathf.FloorToInt((hourOfDay - (float)num) * 60f);
		switch (frequency)
		{
		case WipeTimer.WipeFrequency.Monthly:
			return string.Format("{0} {1} * * {2}#1", num2, num, dayOfWeek);
		case WipeTimer.WipeFrequency.Weekly:
			return string.Format("{0} {1} * * {2}", num2, num, dayOfWeek);
		case WipeTimer.WipeFrequency.BiWeekly:
			return string.Format("{0} {1} 1-7,15-21,29-31 * {2}", num2, num, dayOfWeek);
		default:
			throw new NotSupportedException(string.Format("WipeFrequency {0}", frequency));
		}
	}

	// Token: 0x06002467 RID: 9319 RVA: 0x000E834C File Offset: 0x000E654C
	private static TimeZoneInfo GetTimeZone()
	{
		if (string.IsNullOrWhiteSpace(WipeTimer.wipeTimezone))
		{
			return TimeZoneInfo.Local;
		}
		if (WipeTimer.wipeTimezone == WipeTimer.timezoneCacheKey && WipeTimer.timezoneCache != null)
		{
			return WipeTimer.timezoneCache;
		}
		if (TZConvert.TryGetTimeZoneInfo(WipeTimer.wipeTimezone, out WipeTimer.timezoneCache))
		{
			WipeTimer.timezoneCacheKey = WipeTimer.wipeTimezone;
			return WipeTimer.timezoneCache;
		}
		return TimeZoneInfo.Local;
	}

	// Token: 0x04001C44 RID: 7236
	[ServerVar(Help = "0=sun,1=mon,2=tues,3=wed,4=thur,5=fri,6=sat")]
	public static int wipeDayOfWeek = 4;

	// Token: 0x04001C45 RID: 7237
	[ServerVar(Help = "Which hour to wipe? 14.5 = 2:30pm")]
	public static float wipeHourOfDay = 19f;

	// Token: 0x04001C46 RID: 7238
	[ServerVar(Help = "The timezone to use for wipes. Defaults to the server's time zone if not set or invalid. Value should be a TZ identifier as seen here: https://en.wikipedia.org/wiki/List_of_tz_database_time_zones")]
	public static string wipeTimezone = "Europe/London";

	// Token: 0x04001C47 RID: 7239
	[ServerVar(Help = "Unix timestamp (seconds) for the upcoming wipe. Overrides all other convars if set to a time in the future.")]
	public static long wipeUnixTimestampOverride = 0L;

	// Token: 0x04001C48 RID: 7240
	[ServerVar(Help = "Custom cron expression for the wipe schedule. Overrides all other convars (except wipeUnixTimestampOverride) if set. Uses Cronos as a parser: https://github.com/HangfireIO/Cronos/")]
	public static string wipeCronOverride = "";

	// Token: 0x04001C49 RID: 7241
	public bool useWipeDayOverride;

	// Token: 0x04001C4A RID: 7242
	public DayOfWeek wipeDayOfWeekOverride = DayOfWeek.Thursday;

	// Token: 0x04001C4B RID: 7243
	public WipeTimer.WipeFrequency wipeFrequency;

	// Token: 0x04001C4C RID: 7244
	[ServerVar(Name = "days_to_add_test")]
	public static int daysToAddTest = 0;

	// Token: 0x04001C4D RID: 7245
	[ServerVar(Name = "hours_to_add_test")]
	public static float hoursToAddTest = 0f;

	// Token: 0x04001C4E RID: 7246
	public static WipeTimer serverinstance;

	// Token: 0x04001C4F RID: 7247
	public static WipeTimer clientinstance;

	// Token: 0x04001C50 RID: 7248
	private string oldTags = "";

	// Token: 0x04001C51 RID: 7249
	private static string cronExprCacheKey = null;

	// Token: 0x04001C52 RID: 7250
	private static CronExpression cronExprCache = null;

	// Token: 0x04001C53 RID: 7251
	private static ValueTuple<WipeTimer.WipeFrequency, int, float>? cronCacheKey = null;

	// Token: 0x04001C54 RID: 7252
	private static string cronCache = null;

	// Token: 0x04001C55 RID: 7253
	private static string timezoneCacheKey = null;

	// Token: 0x04001C56 RID: 7254
	private static TimeZoneInfo timezoneCache = null;

	// Token: 0x02000CF9 RID: 3321
	public enum WipeFrequency
	{
		// Token: 0x04004628 RID: 17960
		Monthly,
		// Token: 0x04004629 RID: 17961
		Weekly,
		// Token: 0x0400462A RID: 17962
		BiWeekly
	}
}
