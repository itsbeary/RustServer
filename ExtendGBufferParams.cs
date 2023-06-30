using System;

// Token: 0x0200071D RID: 1821
[Serializable]
public struct ExtendGBufferParams
{
	// Token: 0x040029CC RID: 10700
	public bool enabled;

	// Token: 0x040029CD RID: 10701
	public static ExtendGBufferParams Default = new ExtendGBufferParams
	{
		enabled = false
	};
}
