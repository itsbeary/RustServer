using System;
using UnityEngine;

// Token: 0x0200036C RID: 876
public class AIMemoryBank<T>
{
	// Token: 0x06001FD6 RID: 8150 RVA: 0x000D694E File Offset: 0x000D4B4E
	public AIMemoryBank(MemoryBankType type, int slots)
	{
		this.Init(type, slots);
	}

	// Token: 0x06001FD7 RID: 8151 RVA: 0x000D695E File Offset: 0x000D4B5E
	public void Init(MemoryBankType type, int slots)
	{
		this.type = type;
		this.slotCount = slots;
		this.slots = new T[this.slotCount];
		this.slotSetTimestamps = new float[this.slotCount];
	}

	// Token: 0x06001FD8 RID: 8152 RVA: 0x000D6990 File Offset: 0x000D4B90
	public void Set(T item, int index)
	{
		if (index < 0 || index >= this.slotCount)
		{
			return;
		}
		this.slots[index] = item;
		this.slotSetTimestamps[index] = Time.realtimeSinceStartup;
	}

	// Token: 0x06001FD9 RID: 8153 RVA: 0x000D69BC File Offset: 0x000D4BBC
	public T Get(int index)
	{
		if (index < 0 || index >= this.slotCount)
		{
			return default(T);
		}
		return this.slots[index];
	}

	// Token: 0x06001FDA RID: 8154 RVA: 0x000D69EC File Offset: 0x000D4BEC
	public float GetTimeSinceSet(int index)
	{
		if (index < 0 || index >= this.slotCount)
		{
			return 0f;
		}
		return Time.realtimeSinceStartup - this.slotSetTimestamps[index];
	}

	// Token: 0x06001FDB RID: 8155 RVA: 0x000D6A10 File Offset: 0x000D4C10
	public void Remove(int index)
	{
		if (index < 0 || index >= this.slotCount)
		{
			return;
		}
		this.slots[index] = default(T);
	}

	// Token: 0x06001FDC RID: 8156 RVA: 0x000D6A40 File Offset: 0x000D4C40
	public void Clear()
	{
		for (int i = 0; i < 4; i++)
		{
			this.Remove(i);
		}
	}

	// Token: 0x04001944 RID: 6468
	private MemoryBankType type;

	// Token: 0x04001945 RID: 6469
	private T[] slots;

	// Token: 0x04001946 RID: 6470
	private float[] slotSetTimestamps;

	// Token: 0x04001947 RID: 6471
	private int slotCount;
}
