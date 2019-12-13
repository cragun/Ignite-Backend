namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public enum FinancingType
    {
        Fixed, //the amount to finance is fixed
        Calculated, //the amount is calculated as a percentage of purchase price
        Refi, //the amount to finance is variable and will be set by the system to the current balance.  There can only be one ref product starting the same month
    }
}
