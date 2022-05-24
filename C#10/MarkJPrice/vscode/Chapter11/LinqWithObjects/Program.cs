using static System.Console;

// a string array is a sequence that implements IEnumerable<string>
string[] names = new[] {"Michael", "Pam", "Jim", "Dwight", "Angela", "Kevin", "Toby", "Creed"};

WriteLine("Deferred execution");

// Question: Which names end with an M?
// (Written using a LINQ extension method)
var query1 = names.Where(name => name.EndsWith("m"));

// Question: Which names end with