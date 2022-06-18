
namespace CashierLineSimulator
{
    internal class Customer
    {
        public string Type { get; set; }
        public int Arrival { get; set; }
        public int NumOfItems { get; set; }

        public Customer(string type, int arrival, int numOfItem)
        {
            Type = type;
            Arrival = arrival;
            NumOfItems = numOfItem;
        }
    }
}
