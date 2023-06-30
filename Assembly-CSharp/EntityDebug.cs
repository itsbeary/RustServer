using System;
using System.Diagnostics;

// Token: 0x020003C8 RID: 968
public class EntityDebug : EntityComponent<BaseEntity>
{
	// Token: 0x060021D2 RID: 8658 RVA: 0x000DC514 File Offset: 0x000DA714
	private void Update()
	{
		if (!base.baseEntity.IsValid() || !base.baseEntity.IsDebugging())
		{
			base.enabled = false;
			return;
		}
		if (this.stopwatch.Elapsed.TotalSeconds < 0.5)
		{
			return;
		}
		bool isClient = base.baseEntity.isClient;
		if (base.baseEntity.isServer)
		{
			base.baseEntity.DebugServer(1, (float)this.stopwatch.Elapsed.TotalSeconds);
		}
		this.stopwatch.Reset();
		this.stopwatch.Start();
	}

	// Token: 0x04001A31 RID: 6705
	internal Stopwatch stopwatch = Stopwatch.StartNew();
}
