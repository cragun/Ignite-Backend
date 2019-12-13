using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Attributes
{
    /// <summary>
    /// This attribute is used on navigation properties to let the service know they need to be attached to the entity on update
    /// </summary>
    public class AttachOnUpdateAttribute : Attribute
    {
    }
}
