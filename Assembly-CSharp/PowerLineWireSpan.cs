using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000692 RID: 1682
public class PowerLineWireSpan : MonoBehaviour
{
	// Token: 0x06003021 RID: 12321 RVA: 0x001216C4 File Offset: 0x0011F8C4
	public void Init(PowerLineWire wire)
	{
		if (this.start && this.end)
		{
			this.WireLength = Vector3.Distance(this.start.position, this.end.position);
			for (int i = 0; i < this.connections.Count; i++)
			{
				Vector3 vector = this.start.TransformPoint(this.connections[i].outOffset);
				Vector3 vector2 = this.end.TransformPoint(this.connections[i].inOffset);
				this.WireLength = (vector - vector2).magnitude;
				GameObject gameObject = this.wirePrefab.Instantiate(base.transform);
				gameObject.name = "WIRE";
				gameObject.transform.position = Vector3.Lerp(vector, vector2, 0.5f);
				gameObject.transform.LookAt(vector2);
				gameObject.transform.localScale = new Vector3(1f, 1f, Vector3.Distance(vector, vector2));
				gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x040027BE RID: 10174
	public GameObjectRef wirePrefab;

	// Token: 0x040027BF RID: 10175
	public Transform start;

	// Token: 0x040027C0 RID: 10176
	public Transform end;

	// Token: 0x040027C1 RID: 10177
	public float WireLength;

	// Token: 0x040027C2 RID: 10178
	public List<PowerLineWireConnection> connections = new List<PowerLineWireConnection>();
}
