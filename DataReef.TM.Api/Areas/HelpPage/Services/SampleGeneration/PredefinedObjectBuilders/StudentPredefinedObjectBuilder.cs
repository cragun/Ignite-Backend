//using DataReef.Core.Attributes;
//using DataReef.TM.Api.Areas.HelpPage.Services.SampleGeneration.ObjectGenerator;
//using DataReef.TM.Models;

//namespace DataReef.TM.Api.Areas.HelpPage.Services.SampleGeneration.PredefinedObjectBuilders
//{
//    [Service("StudentPredefinedObjectBuilder", typeof(IPredefinedObjectBuilder))]
//    public class StudentPredefinedObjectBuilder : BasePredefinedObjectBuilder<Student>
//    {
//        protected override Student InternalBuild()
//        {
//            return new Student
//            {
//                LastName = "John",
//                FirstName = "Doe",
//                Addresses = new[]
//                {
//                    new Address
//                    {
//                        Name = "The address"
//                    }
//                }
//                // ... and so on
//            };
//        }
//    }
//}