namespace DataReef.CodeGenerator
{

    public enum Multiplicity
    {
        One=1,
        Many=2
    }

    public class Relationship
    {
        //  //   <relationship name="binder" optional="YES" minCount="1" maxCount="1" deletionRule="Nullify" destinationEntity="Binder" inverseName="pages" inverseEntity="Binder" syncable="YES"/>


        public string Name { get; set; }
        public bool IsOptional { get; set; }
        public int? MinCount { get; set; }
        public int? MaxCount { get; set; }
        public Multiplicity Multiplicity { get; set; }
        public string DestinationEntity { get; set; }
        public string InverseEntity { get; set; }
        public string InverseName { get; set; }
        public bool IsVirtual { get; set; }

    }
}
