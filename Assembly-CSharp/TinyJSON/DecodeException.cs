using System;

namespace TinyJSON
{
	// Token: 0x020009DA RID: 2522
	public sealed class DecodeException : Exception
	{
		// Token: 0x06003BFF RID: 15359 RVA: 0x00161C9D File Offset: 0x0015FE9D
		public DecodeException(string message)
			: base(message)
		{
		}

		// Token: 0x06003C00 RID: 15360 RVA: 0x00161CA6 File Offset: 0x0015FEA6
		public DecodeException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
