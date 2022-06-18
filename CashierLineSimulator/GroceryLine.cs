
namespace CashierLineSimulator
{
    internal class GroceryLine
    {
        public int NumOfRegisters { get; set; }
        public List<Customer> Customers { get; set; }

        public GroceryLine(string filePath)
        {
            //Default values
            NumOfRegisters = 0;
            Customers = new List<Customer>();

            //Load from file
            FileInfo file = new FileInfo(filePath);
            using (StreamReader stream = file.OpenText())
            {
                string l = stream.ReadLine();
                while (l != null)
                {
                    //Parse from each line
                    ParseLine(l);
                    l = stream.ReadLine();
                }
            }
        }

        /// <summary>
        /// Take a line of input and parse information based on the format. Single integer represent number of registers, A letter followed by two integers represent a customer of type "A" or "B", arrival time and number of items to check out.
        /// </summary>
        /// <param name="line">Input line in string</param>
        private void ParseLine(string line)
        {
            string trimmedLine = line.Trim();
            int length = trimmedLine.Length;
            switch (length)
            {
                case 1:
                    NumOfRegisters = Convert.ToInt32(trimmedLine);
                    break;
                default:
                    string[] strs = trimmedLine.Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                    if (strs.Length == 3)
                    {
                        string type = ValidateType(strs[0]);
                        int arrival = ValidateArrival(Convert.ToInt32(strs[1]));
                        int numOfItems = ValidateNumOfItems(Convert.ToInt32(strs[2]));
                        Customer newCustomer = new Customer(type, arrival, numOfItems);
                        if (newCustomer != null && Customers != null)
                        {
                            Customers.Add(newCustomer);
                        }
                    }
                    else
                    {
                        Console.WriteLine(String.Format("Warning: Received input \"{0}\". One or more of the three Customer input (Type, Arrival, NumOfItems) missing. This cusotmer is skipped", trimmedLine));
                    }
                    break;
            }
        }

        /// <summary>
        /// Validate and conform customer type. Conforming to "" and displaying warning message if found invalid is a design choice I took which can be changed based on business requirement
        /// </summary>
        /// <param name="type">Type of customer</param>
        /// <returns>Type of customer after validation</returns>
        private string ValidateType(string type)
        {
            if (type == null)
            {
                Console.WriteLine(String.Format("Warning: type = null detected. Conformed to \"\". This customer may not be processed."));
                return "";
            }
            if (type != "A" && type != "B")
            {
                Console.WriteLine(String.Format("Warning: type = {0} detected. Conformed to \"\". This customer may not be correctly processed.", type));
                return "";
            }
            return type;

        }

        /// <summary>
        /// Validate and conform customer arrival time. Conforming to 0 and displaying warning message if found invalid is a design choice I took which can be changed based on business requirement
        /// </summary>
        /// <param name="type">Customer arrival time</param>
        /// <returns>Customer arrival time after validation</returns>
        private int ValidateArrival(int arrival)
        {
            if (arrival <= 0)
            {
                Console.WriteLine(String.Format("Warning: arrival = {0} detected. Conformed to 0. This customer may not be correctly processed.", arrival));
                return 0;
            }
            return arrival;

        }

        /// <summary>
        /// Validate and conform customer's number of items. Conforming to 0 and displaying warning message if found invalid is a design choice I took which can be changed based on business requirement
        /// </summary>
        /// <param name="type">Customer's number of items</param>
        /// <returns>Customer's number of items after validation</returns>
        private int ValidateNumOfItems(int numOfItems)
        {
            if (numOfItems <= 0)
            {
                Console.WriteLine(String.Format("Warning: numOfItems = {0} detected. Conformed to 0. This customer may not be correctly processed.", numOfItems));
                return 0;
            }
            return numOfItems;

        }

    }
}
