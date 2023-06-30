using System;
using UnityEngine;

namespace AmplifyOcclusion
{
	// Token: 0x020009CD RID: 2509
	[Serializable]
	public class VersionInfo
	{
		// Token: 0x06003BC3 RID: 15299 RVA: 0x00160B22 File Offset: 0x0015ED22
		public static string StaticToString()
		{
			return string.Format("{0}.{1}.{2}", 2, 0, 0) + VersionInfo.StageSuffix;
		}

		// Token: 0x06003BC4 RID: 15300 RVA: 0x00160B4A File Offset: 0x0015ED4A
		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}", this.m_major, this.m_minor, this.m_release) + VersionInfo.StageSuffix;
		}

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x06003BC5 RID: 15301 RVA: 0x00160B81 File Offset: 0x0015ED81
		public int Number
		{
			get
			{
				return this.m_major * 100 + this.m_minor * 10 + this.m_release;
			}
		}

		// Token: 0x06003BC6 RID: 15302 RVA: 0x00160B9D File Offset: 0x0015ED9D
		private VersionInfo()
		{
			this.m_major = 2;
			this.m_minor = 0;
			this.m_release = 0;
		}

		// Token: 0x06003BC7 RID: 15303 RVA: 0x00160BBA File Offset: 0x0015EDBA
		private VersionInfo(byte major, byte minor, byte release)
		{
			this.m_major = (int)major;
			this.m_minor = (int)minor;
			this.m_release = (int)release;
		}

		// Token: 0x06003BC8 RID: 15304 RVA: 0x00160BD7 File Offset: 0x0015EDD7
		public static VersionInfo Current()
		{
			return new VersionInfo(2, 0, 0);
		}

		// Token: 0x06003BC9 RID: 15305 RVA: 0x00160BE1 File Offset: 0x0015EDE1
		public static bool Matches(VersionInfo version)
		{
			return 2 == version.m_major && version.m_minor == 0 && version.m_release == 0;
		}

		// Token: 0x040036BC RID: 14012
		public const byte Major = 2;

		// Token: 0x040036BD RID: 14013
		public const byte Minor = 0;

		// Token: 0x040036BE RID: 14014
		public const byte Release = 0;

		// Token: 0x040036BF RID: 14015
		private static string StageSuffix = "_dev002";

		// Token: 0x040036C0 RID: 14016
		[SerializeField]
		private int m_major;

		// Token: 0x040036C1 RID: 14017
		[SerializeField]
		private int m_minor;

		// Token: 0x040036C2 RID: 14018
		[SerializeField]
		private int m_release;
	}
}
