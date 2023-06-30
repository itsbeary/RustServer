using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000567 RID: 1383
public class RealmedRemove : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x06002A9B RID: 10907 RVA: 0x00103A38 File Offset: 0x00101C38
	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (clientside)
		{
			GameObject[] array = this.removedFromClient;
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array[i], true);
			}
			Component[] array2 = this.removedComponentFromClient;
			for (int i = 0; i < array2.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array2[i], true);
			}
		}
		if (serverside)
		{
			GameObject[] array = this.removedFromServer;
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array[i], true);
			}
			Component[] array2 = this.removedComponentFromServer;
			for (int i = 0; i < array2.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array2[i], true);
			}
		}
		if (!bundling)
		{
			process.RemoveComponent(this);
		}
	}

	// Token: 0x06002A9C RID: 10908 RVA: 0x00103AD0 File Offset: 0x00101CD0
	public bool ShouldDelete(Component comp, bool client, bool server)
	{
		return (!client || this.doNotRemoveFromClient == null || !this.doNotRemoveFromClient.Contains(comp)) && (!server || this.doNotRemoveFromServer == null || !this.doNotRemoveFromServer.Contains(comp));
	}

	// Token: 0x040022D7 RID: 8919
	public GameObject[] removedFromClient;

	// Token: 0x040022D8 RID: 8920
	public Component[] removedComponentFromClient;

	// Token: 0x040022D9 RID: 8921
	public GameObject[] removedFromServer;

	// Token: 0x040022DA RID: 8922
	public Component[] removedComponentFromServer;

	// Token: 0x040022DB RID: 8923
	public Component[] doNotRemoveFromServer;

	// Token: 0x040022DC RID: 8924
	public Component[] doNotRemoveFromClient;
}
