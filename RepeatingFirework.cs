using System;
using Network;

// Token: 0x0200001D RID: 29
public class RepeatingFirework : BaseFirework
{
	// Token: 0x06000074 RID: 116 RVA: 0x00003EB1 File Offset: 0x000020B1
	public override void Begin()
	{
		base.Begin();
		base.InvokeRepeating(new Action(this.SendFire), 0f, this.timeBetweenRepeats);
		base.CancelInvoke(new Action(this.OnExhausted));
	}

	// Token: 0x06000075 RID: 117 RVA: 0x00003EEC File Offset: 0x000020EC
	public void SendFire()
	{
		base.ClientRPC(null, "RPCFire");
		this.numFired++;
		if (this.numFired >= this.maxRepeats)
		{
			base.CancelInvoke(new Action(this.SendFire));
			this.numFired = 0;
			this.OnExhausted();
		}
	}

	// Token: 0x06000076 RID: 118 RVA: 0x00003F40 File Offset: 0x00002140
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RepeatingFirework.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x04000069 RID: 105
	public float timeBetweenRepeats = 1f;

	// Token: 0x0400006A RID: 106
	public int maxRepeats = 12;

	// Token: 0x0400006B RID: 107
	public SoundPlayer launchSound;

	// Token: 0x0400006C RID: 108
	private int numFired;
}
