using System;
using System.Text;
using ProtoBuf;

// Token: 0x020003F7 RID: 1015
public static class GrowableGeneEncoding
{
	// Token: 0x060022EF RID: 8943 RVA: 0x000E05A2 File Offset: 0x000DE7A2
	public static void EncodeGenesToItem(global::GrowableEntity sourceGrowable, global::Item targetItem)
	{
		if (sourceGrowable == null || sourceGrowable.Genes == null)
		{
			return;
		}
		GrowableGeneEncoding.EncodeGenesToItem(GrowableGeneEncoding.EncodeGenesToInt(sourceGrowable.Genes), targetItem);
	}

	// Token: 0x060022F0 RID: 8944 RVA: 0x000E05C7 File Offset: 0x000DE7C7
	public static void EncodeGenesToItem(int genes, global::Item targetItem)
	{
		if (targetItem == null)
		{
			return;
		}
		targetItem.instanceData = new ProtoBuf.Item.InstanceData
		{
			ShouldPool = false,
			dataInt = genes
		};
	}

	// Token: 0x060022F1 RID: 8945 RVA: 0x000E05E8 File Offset: 0x000DE7E8
	public static int EncodeGenesToInt(GrowableGenes genes)
	{
		int num = 0;
		for (int i = 0; i < genes.Genes.Length; i++)
		{
			num = GrowableGeneEncoding.Set(num, i, (int)genes.Genes[i].Type);
		}
		return num;
	}

	// Token: 0x060022F2 RID: 8946 RVA: 0x000E0620 File Offset: 0x000DE820
	public static int EncodePreviousGenesToInt(GrowableGenes genes)
	{
		int num = 0;
		for (int i = 0; i < genes.Genes.Length; i++)
		{
			num = GrowableGeneEncoding.Set(num, i, (int)genes.Genes[i].PreviousType);
		}
		return num;
	}

	// Token: 0x060022F3 RID: 8947 RVA: 0x000E0658 File Offset: 0x000DE858
	public static void DecodeIntToGenes(int data, GrowableGenes genes)
	{
		for (int i = 0; i < 6; i++)
		{
			genes.Genes[i].Set((GrowableGenetics.GeneType)GrowableGeneEncoding.Get(data, i), false);
		}
	}

	// Token: 0x060022F4 RID: 8948 RVA: 0x000E0688 File Offset: 0x000DE888
	public static void DecodeIntToPreviousGenes(int data, GrowableGenes genes)
	{
		for (int i = 0; i < 6; i++)
		{
			genes.Genes[i].SetPrevious((GrowableGenetics.GeneType)GrowableGeneEncoding.Get(data, i));
		}
	}

	// Token: 0x060022F5 RID: 8949 RVA: 0x000E06B8 File Offset: 0x000DE8B8
	public static string DecodeIntToGeneString(int data)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 6; i++)
		{
			stringBuilder.Append(GrowableGene.GetColourCodedDisplayCharacter((GrowableGenetics.GeneType)GrowableGeneEncoding.Get(data, i)));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060022F6 RID: 8950 RVA: 0x000E06F0 File Offset: 0x000DE8F0
	private static int Set(int storage, int slot, int value)
	{
		int num = slot * 5;
		int num2 = 31 << num;
		return (storage & ~num2) | (value << num);
	}

	// Token: 0x060022F7 RID: 8951 RVA: 0x000E0714 File Offset: 0x000DE914
	private static int Get(int storage, int slot)
	{
		int num = slot * 5;
		int num2 = 31 << num;
		return (storage & num2) >> num;
	}
}
