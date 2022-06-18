using CashierLineSimulator;
class Program
{
    /// <summary>
    /// Program to simulate grocery line check out given input stored in a text file. File path is the only arguement required for this function
    /// </summary>
    /// <param name="args">Takes one arguement, path of the input file.</param>
    static void Main(string[] args)
    {
        //Check raw input
        if (args.Length == 1)
        {
            try
            {
                //Loading input by initialize GroceryLine class. 
                Console.WriteLine(String.Format("Preparing Grocery Line..."));
                GroceryLine gl = new GroceryLine(args[0]);
                Console.WriteLine(String.Format("Grocery Line Loaded"));
                Console.WriteLine(String.Format("Number Of Registers: {0}\n" +
                    "Number Of Customers: {1}.\n", gl.NumOfRegisters, gl.Customers.Count));


                //Process input and calculate total time
                Console.WriteLine(String.Format("Check out starts..."));
                int result = GetTime(gl);
                Console.WriteLine(String.Format("Finished at: t={0} minutes", result));

            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Error: {0}", e.ToString()));
            }
        }
        else
        {
            Console.Write("Error: Blank or invalid input. Only one arguement required. Please try again.");
        }


    }

    /// <summary>
    /// Calculate time required to complete check out given a GroceryLine object. At each new customer's arrival, we first check out items at each register given the time allowed. Then the new customer is assigned to appropriate register based on their type. After all new customers are assigned we complete check out for the remaining items at each register.
    /// </summary>
    /// <param name="gl">GroceryLine Object</param>
    /// <returns>Total time required to complete check out</returns>
    /// <exception cref="InvalidDataException">Thrown when customer type found to be other than "A" or "B"</exception>
    private static int GetTime(GroceryLine gl)
    {
        if (gl != null && gl.Customers.Any())
        {
            //Array that holds customer item checkout time array for each register
            List<List<int>> checkoutLine = new List<List<int>>();
            for (int i = 0; i < gl.NumOfRegisters; i++) checkoutLine.Add(new List<int>());
            //Array of number of customers remained at each register
            int[] numOfCustomersRemained = new int[gl.NumOfRegisters];
            int currentTime = 0;
            Console.WriteLine(String.Format("T={0}: Openning register...", currentTime));

            //Process each customer in order of arrival, then number of items, then type (A first)
            foreach (Customer c in gl.Customers.OrderBy(x => x.Arrival).ThenBy(x => x.NumOfItems).ThenBy(x => x.Type))
            {
                if (currentTime == 0) currentTime = c.Arrival;
                Console.WriteLine(String.Format("T={0}: Processing customer {1}{2}{3}", c.Arrival, c.Type, c.Arrival, c.NumOfItems));

                //Initialize time delta since last customer
                int timeDelta = c.Arrival - currentTime;
                //Initialize time multuplier to 1
                int multiplier = 1;
                //Max possible number of customers in a line
                int minNumOfCustomerRemained = gl.Customers.Count;
                //Register number for Type A cusomter (0-indexed)
                int minNumOfCustomerRemainedIndex = gl.NumOfRegisters;
                //Max possible number of items for the last customer in a line
                int minNumOfItemsForLast = int.MaxValue;
                //Register number for Type B customer (0-indexed)
                int minNumOfItemsForLastIndex = gl.NumOfRegisters;

                //Check out items at each register to time of next customer's arrival starting from the last register
                for (int i = gl.NumOfRegisters - 1; i >= 0; i--)
                {
                    //Reset multiplier
                    if (i == gl.NumOfRegisters - 1) multiplier = 2;
                    else multiplier = 1;

                    int timeDeduction = timeDelta;
                    int currentCustomer = checkoutLine[i].Count - numOfCustomersRemained[i];
                    //Based on timeDuction (time allowed until next customer's arrival) check out items for active customers in this register
                    while (timeDeduction > 0 && currentCustomer < checkoutLine[i].Count)
                    {
                        if (timeDeduction < checkoutLine[i][currentCustomer])
                        {
                            checkoutLine[i][currentCustomer] = checkoutLine[i][currentCustomer] - timeDeduction;
                            timeDeduction = 0;
                        }
                        else
                        {
                            timeDeduction = timeDeduction - checkoutLine[i][currentCustomer];
                            checkoutLine[i][currentCustomer] = 0;
                            numOfCustomersRemained[i]--;
                        }
                        currentCustomer++;
                    }

                    //Find out the register to go based on number of customers still in line for the next Type A customer
                    if (numOfCustomersRemained[i] <= minNumOfCustomerRemained)
                    {
                        minNumOfCustomerRemainedIndex = i;
                        minNumOfCustomerRemained = numOfCustomersRemained[i];
                    }
                    //Find out the register to go based on number of items of the last customer in line for the next Type B customer
                    if (checkoutLine[i].LastOrDefault(0) / multiplier <= minNumOfItemsForLast / multiplier)
                    {
                        minNumOfItemsForLastIndex = i;
                        minNumOfItemsForLast = checkoutLine[i].LastOrDefault(0) / multiplier;
                    }

                }

                //Assign the next customer based on their type
                if (c.Type == "A")
                {
                    if (minNumOfCustomerRemainedIndex == gl.NumOfRegisters - 1) multiplier = 2;
                    var newLine = checkoutLine[minNumOfCustomerRemainedIndex];
                    newLine.Add(c.NumOfItems * multiplier);
                    checkoutLine[minNumOfCustomerRemainedIndex] = newLine;
                    numOfCustomersRemained[minNumOfCustomerRemainedIndex]++;
                    Console.WriteLine(String.Format("Customer {0}{1}{2} assigned to register {3}", c.Type, c.Arrival, c.NumOfItems, minNumOfCustomerRemainedIndex + 1));
                }
                else if (c.Type == "B")
                {
                    if (minNumOfItemsForLastIndex == gl.NumOfRegisters - 1) multiplier = 2;
                    var newLine = checkoutLine[minNumOfItemsForLastIndex];
                    newLine.Add(c.NumOfItems * multiplier);
                    checkoutLine[minNumOfItemsForLastIndex] = newLine;
                    numOfCustomersRemained[minNumOfItemsForLastIndex]++;
                    Console.WriteLine(String.Format("Customer {0}{1}{2} assigned to register {3}", c.Type, c.Arrival, c.NumOfItems, minNumOfItemsForLastIndex + 1));
                }
                //If we have a customer of an unknown type, stop check out.
                else
                {
                    throw new InvalidDataException("Error: Customer other than type \"A\" or \"B\" found");
                }

                currentTime = c.Arrival;
            }

            //When we have no more new customers, we just complete checkout all items at each register and find out the longest time it may take for any
            int longestRemindedLine = 0;
            foreach (List<int> register in checkoutLine)
            {
                int registerSum = register.Sum();
                if (registerSum > longestRemindedLine) longestRemindedLine = registerSum;
            }
            //Total time required is time we used to assign all customers (while checking out items) plus the longest time to check out remaining items.
            return currentTime + longestRemindedLine;
        }
        return 0;
    }
}


