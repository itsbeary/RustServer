using System;
using EasyRoads3Dv3;
using UnityEngine;

// Token: 0x0200098E RID: 2446
public class ERVegetationStudio : ScriptableObject
{
	// Token: 0x06003A11 RID: 14865 RVA: 0x00007A44 File Offset: 0x00005C44
	public static bool VegetationStudio()
	{
		return false;
	}

	// Token: 0x06003A12 RID: 14866 RVA: 0x00007A44 File Offset: 0x00005C44
	public static bool VegetationStudioPro()
	{
		return false;
	}

	// Token: 0x06003A13 RID: 14867 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void CreateVegetationMaskLine(GameObject go, float grassPerimeter, float plantPerimeter, float treePerimeter, float objectPerimeter, float largeObjectPerimeter)
	{
	}

	// Token: 0x06003A14 RID: 14868 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void UpdateVegetationMaskLine(GameObject go, ERVSData[] vsData, float grassPerimeter, float plantPerimeter, float treePerimeter, float objectPerimeter, float largeObjectPerimeter)
	{
	}

	// Token: 0x06003A15 RID: 14869 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void UpdateHeightmap(Bounds bounds)
	{
	}

	// Token: 0x06003A16 RID: 14870 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void RemoveVegetationMaskLine(GameObject go)
	{
	}

	// Token: 0x06003A17 RID: 14871 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void CreateBiomeArea(GameObject go, float distance, float blendDistance, float noise)
	{
	}

	// Token: 0x06003A18 RID: 14872 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void UpdateBiomeArea(GameObject go, ERVSData[] vsData, float distance, float blendDistance, float noise)
	{
	}

	// Token: 0x06003A19 RID: 14873 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void RemoveBiomeArea(GameObject go)
	{
	}
}
