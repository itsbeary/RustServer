using System;
using UnityEngine;

// Token: 0x020001F3 RID: 499
public class AIThinkManager : BaseMonoBehaviour, IServerComponent
{
	// Token: 0x06001A31 RID: 6705 RVA: 0x000BDE10 File Offset: 0x000BC010
	public static void ProcessQueue(AIThinkManager.QueueType queueType)
	{
		if (queueType != AIThinkManager.QueueType.Human)
		{
		}
		if (queueType == AIThinkManager.QueueType.Human)
		{
			AIThinkManager.DoRemoval(AIThinkManager._removalQueue, AIThinkManager._processQueue);
			AIInformationZone.BudgetedTick();
		}
		else if (queueType == AIThinkManager.QueueType.Pets)
		{
			AIThinkManager.DoRemoval(AIThinkManager._petRemovalQueue, AIThinkManager._petProcessQueue);
		}
		else
		{
			AIThinkManager.DoRemoval(AIThinkManager._animalremovalQueue, AIThinkManager._animalProcessQueue);
		}
		if (queueType == AIThinkManager.QueueType.Human)
		{
			AIThinkManager.DoProcessing(AIThinkManager._processQueue, AIThinkManager.framebudgetms / 1000f, ref AIThinkManager.lastIndex);
			return;
		}
		if (queueType == AIThinkManager.QueueType.Pets)
		{
			AIThinkManager.DoProcessing(AIThinkManager._petProcessQueue, AIThinkManager.petframebudgetms / 1000f, ref AIThinkManager.lastPetIndex);
			return;
		}
		AIThinkManager.DoProcessing(AIThinkManager._animalProcessQueue, AIThinkManager.animalframebudgetms / 1000f, ref AIThinkManager.lastAnimalIndex);
	}

	// Token: 0x06001A32 RID: 6706 RVA: 0x000BDEBC File Offset: 0x000BC0BC
	private static void DoRemoval(ListHashSet<IThinker> removal, ListHashSet<IThinker> process)
	{
		if (removal.Count > 0)
		{
			foreach (IThinker thinker in removal)
			{
				process.Remove(thinker);
			}
			removal.Clear();
		}
	}

	// Token: 0x06001A33 RID: 6707 RVA: 0x000BDF1C File Offset: 0x000BC11C
	private static void DoProcessing(ListHashSet<IThinker> process, float budgetSeconds, ref int last)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		while (last < process.Count && Time.realtimeSinceStartup < realtimeSinceStartup + budgetSeconds)
		{
			IThinker thinker = process[last];
			if (thinker != null)
			{
				try
				{
					thinker.TryThink();
				}
				catch (Exception ex)
				{
					Debug.LogWarning(ex);
				}
			}
			last++;
		}
		if (last >= process.Count)
		{
			last = 0;
		}
	}

	// Token: 0x06001A34 RID: 6708 RVA: 0x000BDF84 File Offset: 0x000BC184
	public static void Add(IThinker toAdd)
	{
		AIThinkManager._processQueue.Add(toAdd);
	}

	// Token: 0x06001A35 RID: 6709 RVA: 0x000BDF91 File Offset: 0x000BC191
	public static void Remove(IThinker toRemove)
	{
		AIThinkManager._removalQueue.Add(toRemove);
	}

	// Token: 0x06001A36 RID: 6710 RVA: 0x000BDF9E File Offset: 0x000BC19E
	public static void AddAnimal(IThinker toAdd)
	{
		AIThinkManager._animalProcessQueue.Add(toAdd);
	}

	// Token: 0x06001A37 RID: 6711 RVA: 0x000BDFAB File Offset: 0x000BC1AB
	public static void RemoveAnimal(IThinker toRemove)
	{
		AIThinkManager._animalremovalQueue.Add(toRemove);
	}

	// Token: 0x06001A38 RID: 6712 RVA: 0x000BDFB8 File Offset: 0x000BC1B8
	public static void AddPet(IThinker toAdd)
	{
		AIThinkManager._petProcessQueue.Add(toAdd);
	}

	// Token: 0x06001A39 RID: 6713 RVA: 0x000BDFC5 File Offset: 0x000BC1C5
	public static void RemovePet(IThinker toRemove)
	{
		AIThinkManager._petRemovalQueue.Add(toRemove);
	}

	// Token: 0x040012B5 RID: 4789
	public static ListHashSet<IThinker> _processQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040012B6 RID: 4790
	public static ListHashSet<IThinker> _removalQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040012B7 RID: 4791
	public static ListHashSet<IThinker> _animalProcessQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040012B8 RID: 4792
	public static ListHashSet<IThinker> _animalremovalQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040012B9 RID: 4793
	public static ListHashSet<IThinker> _petProcessQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040012BA RID: 4794
	public static ListHashSet<IThinker> _petRemovalQueue = new ListHashSet<IThinker>(8);

	// Token: 0x040012BB RID: 4795
	[ServerVar]
	[Help("How many miliseconds to budget for processing AI entities per server frame")]
	public static float framebudgetms = 2.5f;

	// Token: 0x040012BC RID: 4796
	[ServerVar]
	[Help("How many miliseconds to budget for processing animal AI entities per server frame")]
	public static float animalframebudgetms = 2.5f;

	// Token: 0x040012BD RID: 4797
	[ServerVar]
	[Help("How many miliseconds to budget for processing pet AI entities per server frame")]
	public static float petframebudgetms = 1f;

	// Token: 0x040012BE RID: 4798
	private static int lastIndex = 0;

	// Token: 0x040012BF RID: 4799
	private static int lastAnimalIndex = 0;

	// Token: 0x040012C0 RID: 4800
	private static int lastPetIndex;

	// Token: 0x02000C5F RID: 3167
	public enum QueueType
	{
		// Token: 0x0400436E RID: 17262
		Human,
		// Token: 0x0400436F RID: 17263
		Animal,
		// Token: 0x04004370 RID: 17264
		Pets
	}
}
