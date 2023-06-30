using System;

// Token: 0x020004EB RID: 1259
public class EventScheduleWipeOffset : EventSchedule
{
	// Token: 0x060028D1 RID: 10449 RVA: 0x000FC0E4 File Offset: 0x000FA2E4
	public override void RunSchedule()
	{
		if (WipeTimer.serverinstance == null)
		{
			return;
		}
		if (WipeTimer.serverinstance.GetTimeSpanUntilWipe().TotalHours > (double)EventScheduleWipeOffset.hoursBeforeWipeRealtime)
		{
			return;
		}
		base.RunSchedule();
	}

	// Token: 0x04002113 RID: 8467
	[ServerVar(Name = "event_hours_before_wipe")]
	public static float hoursBeforeWipeRealtime = 24f;
}
