using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Amica.vNext.Http
{
	/// <summary>
	/// Eve contract resolver. Does not serialize Eve meta fields (which are read only for the API)
	/// </summary>
	public class EveContractResolver : DefaultContractResolver
	{
		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
			var properties = base.CreateProperties(type, memberSerialization);

	        // only serializer properties that start with the specified character
			properties = properties.Where(p => !p.PropertyName.StartsWith("_")).ToList();

			return properties;
		}
	}
}