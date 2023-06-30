using System;
using UnityEngine.Rendering;

// Token: 0x02000716 RID: 1814
public class CommandBufferDesc
{
	// Token: 0x17000425 RID: 1061
	// (get) Token: 0x060032FB RID: 13051 RVA: 0x0013920C File Offset: 0x0013740C
	// (set) Token: 0x060032FC RID: 13052 RVA: 0x00139214 File Offset: 0x00137414
	public CameraEvent CameraEvent { get; private set; }

	// Token: 0x17000426 RID: 1062
	// (get) Token: 0x060032FD RID: 13053 RVA: 0x0013921D File Offset: 0x0013741D
	// (set) Token: 0x060032FE RID: 13054 RVA: 0x00139225 File Offset: 0x00137425
	public int OrderId { get; private set; }

	// Token: 0x17000427 RID: 1063
	// (get) Token: 0x060032FF RID: 13055 RVA: 0x0013922E File Offset: 0x0013742E
	// (set) Token: 0x06003300 RID: 13056 RVA: 0x00139236 File Offset: 0x00137436
	public Action<CommandBuffer> FillDelegate { get; private set; }

	// Token: 0x06003301 RID: 13057 RVA: 0x0013923F File Offset: 0x0013743F
	public CommandBufferDesc(CameraEvent cameraEvent, int orderId, CommandBufferDesc.FillCommandBuffer fill)
	{
		this.CameraEvent = cameraEvent;
		this.OrderId = orderId;
		this.FillDelegate = new Action<CommandBuffer>(fill.Invoke);
	}

	// Token: 0x02000E4B RID: 3659
	// (Invoke) Token: 0x0600527C RID: 21116
	public delegate void FillCommandBuffer(CommandBuffer cb);
}
