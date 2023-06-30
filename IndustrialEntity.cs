using System;

// Token: 0x020004DA RID: 1242
public class IndustrialEntity : IOEntity
{
	// Token: 0x06002873 RID: 10355 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void RunJob()
	{
	}

	// Token: 0x040020B7 RID: 8375
	public static IndustrialEntity.IndustrialProcessQueue Queue = new IndustrialEntity.IndustrialProcessQueue();

	// Token: 0x02000D37 RID: 3383
	public class IndustrialProcessQueue : ObjectWorkQueue<IndustrialEntity>
	{
		// Token: 0x06005083 RID: 20611 RVA: 0x001A916C File Offset: 0x001A736C
		protected override void RunJob(IndustrialEntity job)
		{
			if (job != null)
			{
				job.RunJob();
			}
		}
	}
}
