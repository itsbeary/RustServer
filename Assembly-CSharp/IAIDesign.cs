using System;
using ProtoBuf;

// Token: 0x02000394 RID: 916
internal interface IAIDesign
{
	// Token: 0x06002083 RID: 8323
	void LoadAIDesign(ProtoBuf.AIDesign design, global::BasePlayer player);

	// Token: 0x06002084 RID: 8324
	void StopDesigning();

	// Token: 0x06002085 RID: 8325
	bool CanPlayerDesignAI(global::BasePlayer player);
}
