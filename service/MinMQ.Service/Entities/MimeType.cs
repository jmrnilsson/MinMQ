namespace MinMq.Service.Entities
{
	public class MimeType
	{
		public MimeType(short byteKey, string expression)
		{
			ByteKey = byteKey;
			Expression = expression;
		}

		public short ByteKey { get; }
		public string Expression { get; set; }
	}
}
