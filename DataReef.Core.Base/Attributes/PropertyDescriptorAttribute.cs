using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyDescriptorAttribute : Attribute
	{
		public PropertyDescriptorAttribute(string propertyName)
			: this(propertyName: propertyName, propertyType: null)
		{

		}

		public PropertyDescriptorAttribute(Type propertyType)
			: this(propertyName: "", propertyType: propertyType)
		{

		}

		public PropertyDescriptorAttribute(string propertyName, Type propertyType)
		{
			this.PropertyName = propertyName;
			this.PropertyType = propertyType;
		}

		public string PropertyName
		{
			get;
			private set;
		}

		public Type PropertyType
		{
			get;
			private set;
		}
	}
}
