namespace MinMq.Service.Entities
{
	public class MimeType
	{
		public MimeType(short mimeTypeId, string expression)
		{
			MimeTypeId = mimeTypeId;
			Expression = expression;
		}

		public short MimeTypeId { get; }
		public string Expression { get; set; }
	}
}
