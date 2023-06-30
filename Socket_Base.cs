using System;
using UnityEngine;

// Token: 0x02000283 RID: 643
public class Socket_Base : PrefabAttribute
{
	// Token: 0x06001D1F RID: 7455 RVA: 0x000C96F4 File Offset: 0x000C78F4
	public Socket_Base()
	{
		this.cachedType = base.GetType();
	}

	// Token: 0x06001D20 RID: 7456 RVA: 0x000C5FD3 File Offset: 0x000C41D3
	public Vector3 GetSelectPivot(Vector3 position, Quaternion rotation)
	{
		return position + rotation * this.worldPosition;
	}

	// Token: 0x06001D21 RID: 7457 RVA: 0x000C974E File Offset: 0x000C794E
	public OBB GetSelectBounds(Vector3 position, Quaternion rotation)
	{
		return new OBB(position + rotation * this.worldPosition, Vector3.one, rotation * this.worldRotation, new Bounds(this.selectCenter, this.selectSize));
	}

	// Token: 0x06001D22 RID: 7458 RVA: 0x000C9789 File Offset: 0x000C7989
	protected override Type GetIndexedType()
	{
		return typeof(Socket_Base);
	}

	// Token: 0x06001D23 RID: 7459 RVA: 0x000C9798 File Offset: 0x000C7998
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.position = base.transform.position;
		this.rotation = base.transform.rotation;
		this.socketMods = base.GetComponentsInChildren<SocketMod>(true);
		SocketMod[] array = this.socketMods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].baseSocket = this;
		}
	}

	// Token: 0x06001D24 RID: 7460 RVA: 0x000C97FF File Offset: 0x000C79FF
	public virtual bool TestTarget(Construction.Target target)
	{
		return target.socket != null;
	}

	// Token: 0x06001D25 RID: 7461 RVA: 0x000C9810 File Offset: 0x000C7A10
	public virtual bool IsCompatible(Socket_Base socket)
	{
		return !(socket == null) && (socket.male || this.male) && (socket.female || this.female) && socket.cachedType == this.cachedType;
	}

	// Token: 0x06001D26 RID: 7462 RVA: 0x000C985D File Offset: 0x000C7A5D
	public virtual bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		return this.IsCompatible(socket);
	}

	// Token: 0x06001D27 RID: 7463 RVA: 0x000C9868 File Offset: 0x000C7A68
	public virtual Construction.Placement DoPlacement(Construction.Target target)
	{
		Quaternion quaternion = Quaternion.LookRotation(target.normal, Vector3.up) * Quaternion.Euler(target.rotation);
		Vector3 vector = target.position;
		vector -= quaternion * this.position;
		return new Construction.Placement
		{
			rotation = quaternion,
			position = vector
		};
	}

	// Token: 0x06001D28 RID: 7464 RVA: 0x000C98C4 File Offset: 0x000C7AC4
	public virtual bool CheckSocketMods(Construction.Placement placement)
	{
		SocketMod[] array = this.socketMods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ModifyPlacement(placement);
		}
		foreach (SocketMod socketMod in this.socketMods)
		{
			if (!socketMod.DoCheck(placement))
			{
				if (socketMod.FailedPhrase.IsValid())
				{
					Construction.lastPlacementError = "Failed Check: (" + socketMod.FailedPhrase.translated + ")";
				}
				return false;
			}
		}
		return true;
	}

	// Token: 0x040015AC RID: 5548
	public bool male = true;

	// Token: 0x040015AD RID: 5549
	public bool maleDummy;

	// Token: 0x040015AE RID: 5550
	public bool female;

	// Token: 0x040015AF RID: 5551
	public bool femaleDummy;

	// Token: 0x040015B0 RID: 5552
	public bool femaleNoStability;

	// Token: 0x040015B1 RID: 5553
	public bool monogamous;

	// Token: 0x040015B2 RID: 5554
	[NonSerialized]
	public Vector3 position;

	// Token: 0x040015B3 RID: 5555
	[NonSerialized]
	public Quaternion rotation;

	// Token: 0x040015B4 RID: 5556
	private Type cachedType;

	// Token: 0x040015B5 RID: 5557
	public Vector3 selectSize = new Vector3(2f, 0.1f, 2f);

	// Token: 0x040015B6 RID: 5558
	public Vector3 selectCenter = new Vector3(0f, 0f, 1f);

	// Token: 0x040015B7 RID: 5559
	[ReadOnly]
	public string socketName;

	// Token: 0x040015B8 RID: 5560
	[NonSerialized]
	public SocketMod[] socketMods;

	// Token: 0x040015B9 RID: 5561
	public Socket_Base.OccupiedSocketCheck[] checkOccupiedSockets;

	// Token: 0x02000CA6 RID: 3238
	[Serializable]
	public class OccupiedSocketCheck
	{
		// Token: 0x040044B6 RID: 17590
		public Socket_Base Socket;

		// Token: 0x040044B7 RID: 17591
		public bool FemaleDummy;
	}
}
