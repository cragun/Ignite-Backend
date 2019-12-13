using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataReef.TM.DataAccess.Prototype
{    
    public class ContextTreePrototype
    {
        public Type Root = typeof(Person);

        public Dictionary<Type, Expression> Tree = new Dictionary<Type, Expression>();

        public void Init()
        {             
        }
        
    }
}
