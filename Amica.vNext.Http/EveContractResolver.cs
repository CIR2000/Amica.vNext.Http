using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Amica.vNext.Http
{
	/// <summary>
	/// Does not serialize remote (Eve) meta fields (which are read only for the API)
	/// </summary>
	public class EveContractResolver : DefaultContractResolver
	{
		protected override JsonProperty CreateProperty (MemberInfo member, MemberSerialization memberSerialization)
		{
			JsonProperty property =	base.CreateProperty (member, memberSerialization);
			// if the property is flagged with the RemoteAttribute it's a meta field, so don't serialize it.
			property.ShouldSerialize = 
				instance => {
				var r = member.GetCustomAttributes (typeof(RemoteAttribute), false);
				return r.Length == 0;
			};

			return property;
		}
	}
}