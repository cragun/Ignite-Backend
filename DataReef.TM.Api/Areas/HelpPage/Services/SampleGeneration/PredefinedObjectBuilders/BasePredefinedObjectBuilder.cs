using System;
using DataReef.TM.Api.Areas.HelpPage.Services.SampleGeneration.ObjectGenerator;

namespace DataReef.TM.Api.Areas.HelpPage.Services.SampleGeneration.PredefinedObjectBuilders
{
    public abstract class BasePredefinedObjectBuilder<T> : IPredefinedObjectBuilder
    {
        public bool CanBuild(Type type)
        {
            return type == typeof(T);
        }

        public object Build()
        {
            return this.InternalBuild();
        }

        /// <summary>
        /// The base build method that returns an object of type <typeparamref name="T"/>
        /// </summary>
        /// <returns></returns>
        protected abstract T InternalBuild();
    }
}