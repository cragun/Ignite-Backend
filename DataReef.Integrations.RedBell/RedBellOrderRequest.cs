namespace DataReef.Integrations.RedBell
{
    public class RedBellOrderRequest
    {
        public string City { get; set; }

        public string State { get; set; }

        public string Address { get; set; }

        public string LoanNum { get; set; }

        public int MonthsBack { get; set; }

        public string PoolName { get; set; }
        public string Zip { get; set; }
    }
}
