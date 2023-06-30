using System;
using UnityEngine;

// Token: 0x020008FA RID: 2298
public class CameraUpdateHook : MonoBehaviour
{
	// Token: 0x060037CB RID: 14283 RVA: 0x0014D448 File Offset: 0x0014B648
	private void Awake()
	{
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(delegate(Camera args)
		{
			Action preRender = CameraUpdateHook.PreRender;
			if (preRender == null)
			{
				return;
			}
			preRender();
		}));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(delegate(Camera args)
		{
			Action postRender = CameraUpdateHook.PostRender;
			if (postRender == null)
			{
				return;
			}
			postRender();
		}));
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(delegate(Camera args)
		{
			Action preCull = CameraUpdateHook.PreCull;
			if (preCull == null)
			{
				return;
			}
			preCull();
		}));
	}

	// Token: 0x04003320 RID: 13088
	public static Action PreCull;

	// Token: 0x04003321 RID: 13089
	public static Action PreRender;

	// Token: 0x04003322 RID: 13090
	public static Action PostRender;

	// Token: 0x04003323 RID: 13091
	public static Action RustCamera_PreRender;
}
