using System;

namespace Amica.vNext.Http
{
	[AttributeUsage (AttributeTargets.Property, Inherited = true)]
	public class RemoteAttribute : Attribute
	{
		private Meta _field;

		public RemoteAttribute (Meta field)
		{
			_field = field;
		}

		public Meta Field { get { return _field; } }
	}

	public enum Meta
	{
		DocumentId,
		ETag,
		LastUpdated,
		DateCreated,
	}
}

