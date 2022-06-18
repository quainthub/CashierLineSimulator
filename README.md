# CashierLineSimulator
A simple program to simulate a check out process for a grocery store

In this implementation I split the programs into three .cs files. Program.cs is where the main function is and where 
the program launches given input file path. Algorithm that calcualtes the total check out time is in GetTime function.
I just used a few simple loops to complete the task. The time complexity should be O(n * m) n=numberOfRegister and m=numberOfCustomer. I did not see much of a improvement to use concurrency and that is why I just used sequential code to complete the calculation. The actual file loading happens in the intialization of GroceryLine class. It includes ParseLine function a a few utility validator fucntion to assit with the data loading and validation. Validation logic are of my choice of implementation which can be easily changed. This separation allows us to easily decouple data loading from the core program logic. Each customer is of its own Customer class, which sits separately in the third .cs file. The project also includes a ~/File directory which holds a sample input.txt file. But the executable can be run from any directory as long as a correct input file path is given. 

Please let me know if you have any questions at quaint_xu@yahoo.com.
