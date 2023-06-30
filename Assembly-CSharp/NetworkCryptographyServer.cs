using System;
using Network;

// Token: 0x0200062A RID: 1578
public class NetworkCryptographyServer : NetworkCryptography
{
	// Token: 0x06002E88 RID: 11912 RVA: 0x001174ED File Offset: 0x001156ED
	protected override void EncryptionHandler(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst)
	{
		if (connection.encryptionLevel > 1U)
		{
			EACServer.Encrypt(connection, src, ref dst);
			return;
		}
		Craptography.XOR(2397U, src, ref dst);
	}

	// Token: 0x06002E89 RID: 11913 RVA: 0x0011750D File Offset: 0x0011570D
	protected override void DecryptionHandler(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst)
	{
		if (connection.encryptionLevel > 1U)
		{
			EACServer.Decrypt(connection, src, ref dst);
			return;
		}
		Craptography.XOR(2397U, src, ref dst);
	}
}
