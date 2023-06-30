using System;

// Token: 0x020003B0 RID: 944
public static class FishStateExtensions
{
	// Token: 0x06002151 RID: 8529 RVA: 0x000DACB2 File Offset: 0x000D8EB2
	public static bool Contains(this BaseFishingRod.FishState state, BaseFishingRod.FishState check)
	{
		return (state & check) == check;
	}

	// Token: 0x06002152 RID: 8530 RVA: 0x000DACBA File Offset: 0x000D8EBA
	public static BaseFishingRod.FishState FlipHorizontal(this BaseFishingRod.FishState state)
	{
		if (state.Contains(BaseFishingRod.FishState.PullingLeft))
		{
			state |= BaseFishingRod.FishState.PullingRight;
			state &= ~BaseFishingRod.FishState.PullingLeft;
		}
		else if (state.Contains(BaseFishingRod.FishState.PullingRight))
		{
			state |= BaseFishingRod.FishState.PullingLeft;
			state &= ~BaseFishingRod.FishState.PullingRight;
		}
		return state;
	}
}
