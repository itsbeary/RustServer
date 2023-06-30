using System;

// Token: 0x0200031A RID: 794
public struct ClientPerformanceReport
{
	// Token: 0x040017EB RID: 6123
	public int request_id;

	// Token: 0x040017EC RID: 6124
	public string user_id;

	// Token: 0x040017ED RID: 6125
	public float fps_average;

	// Token: 0x040017EE RID: 6126
	public int fps;

	// Token: 0x040017EF RID: 6127
	public int frame_id;

	// Token: 0x040017F0 RID: 6128
	public float frame_time;

	// Token: 0x040017F1 RID: 6129
	public float frame_time_average;

	// Token: 0x040017F2 RID: 6130
	public long memory_system;

	// Token: 0x040017F3 RID: 6131
	public long memory_collections;

	// Token: 0x040017F4 RID: 6132
	public long memory_managed_heap;

	// Token: 0x040017F5 RID: 6133
	public float realtime_since_startup;

	// Token: 0x040017F6 RID: 6134
	public bool streamer_mode;

	// Token: 0x040017F7 RID: 6135
	public int ping;

	// Token: 0x040017F8 RID: 6136
	public int tasks_invokes;

	// Token: 0x040017F9 RID: 6137
	public int tasks_load_balancer;

	// Token: 0x040017FA RID: 6138
	public int workshop_skins_queued;
}
