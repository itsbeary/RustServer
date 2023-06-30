using System;
using UnityEngine;

// Token: 0x020004D8 RID: 1240
[RequireComponent(typeof(IOEntity))]
public class IOEntityMovementChecker : FacepunchBehaviour
{
	// Token: 0x0600286D RID: 10349 RVA: 0x000FA9B5 File Offset: 0x000F8BB5
	protected void Awake()
	{
		this.ioEntity = base.GetComponent<IOEntity>();
	}

	// Token: 0x0600286E RID: 10350 RVA: 0x000FA9C3 File Offset: 0x000F8BC3
	protected void OnEnable()
	{
		base.InvokeRepeating(new Action(this.CheckPosition), UnityEngine.Random.Range(0f, 0.25f), 0.25f);
	}

	// Token: 0x0600286F RID: 10351 RVA: 0x000FA9EB File Offset: 0x000F8BEB
	protected void OnDisable()
	{
		base.CancelInvoke(new Action(this.CheckPosition));
	}

	// Token: 0x06002870 RID: 10352 RVA: 0x000FAA00 File Offset: 0x000F8C00
	private void CheckPosition()
	{
		if (this.ioEntity.isClient)
		{
			return;
		}
		if (Vector3.SqrMagnitude(base.transform.position - this.prevPos) > 0.0025000002f)
		{
			this.prevPos = base.transform.position;
			if (this.ioEntity.HasConnections())
			{
				this.ioEntity.SendChangedToRoot(true);
				this.ioEntity.ClearConnections();
			}
		}
	}

	// Token: 0x040020B3 RID: 8371
	private IOEntity ioEntity;

	// Token: 0x040020B4 RID: 8372
	private Vector3 prevPos;

	// Token: 0x040020B5 RID: 8373
	private const float MAX_MOVE = 0.05f;

	// Token: 0x040020B6 RID: 8374
	private const float MAX_MOVE_SQR = 0.0025000002f;
}
