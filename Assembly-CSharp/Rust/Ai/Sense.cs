using System;

namespace Rust.Ai
{
	// Token: 0x02000B4F RID: 2895
	public static class Sense
	{
		// Token: 0x06004611 RID: 17937 RVA: 0x0019863C File Offset: 0x0019683C
		public static void Stimulate(Sensation sensation)
		{
			int inSphere = BaseEntity.Query.Server.GetInSphere(sensation.Position, sensation.Radius, Sense.query, new Func<BaseEntity, bool>(Sense.IsAbleToBeStimulated));
			float num = sensation.Radius * sensation.Radius;
			for (int i = 0; i < inSphere; i++)
			{
				if ((Sense.query[i].transform.position - sensation.Position).sqrMagnitude <= num)
				{
					Sense.query[i].OnSensation(sensation);
				}
			}
		}

		// Token: 0x06004612 RID: 17938 RVA: 0x001986BF File Offset: 0x001968BF
		private static bool IsAbleToBeStimulated(BaseEntity ent)
		{
			return ent is BasePlayer || ent is BaseNpc;
		}

		// Token: 0x04003EFA RID: 16122
		private static BaseEntity[] query = new BaseEntity[512];
	}
}
