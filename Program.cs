

using System.Diagnostics;

namespace GuessWho;


public static class Program {

    private static List<Person> People = new List<Person>();

    private static bool running = true;

    private static List<Attribute>? lastGuess = null;
    
    public static void Main() {
        // populate the people list.
        // this list should contain an instance of Person for every person on the board
        
        Dictionary<string, Action<string[]>> CommandRegistry = new Dictionary<string, Action<string[]>>();

        CommandRegistry["stop"] = Stop;
        CommandRegistry["guess"] = MakeGuess;
        CommandRegistry["resolve"] = ResolveGuess;
        
        while (running) {
            string? userInput = Input("> ");
            
            string[]? splitInput = userInput?.Split(" ");

            string? command = splitInput?[0].ToLower();
            
            if (command != null && splitInput != null && CommandRegistry.ContainsKey(command)) {
                CommandRegistry[command](splitInput);
            }
            else {
                Print("Command Not Found.");
            }
            
        }
        
    }

    private static void MakeGuess(string[] _input) {
        Dictionary<Attribute, HashSet<Person>> attributesToPeople = new Dictionary<Attribute, HashSet<Person>>();

        foreach (Person person in People) {
            foreach (Attribute attribute in person.HasAttributes) {
                // If this is the first person with this attribute, we need to 
                // create the set we will store the people in.
                if (!attributesToPeople.ContainsKey(attribute)) {
                    attributesToPeople[attribute] = new HashSet<Person>();
                }

                attributesToPeople[attribute].Add(person);
            }
        }
        
        List<Attribute> allAttributes = attributesToPeople.Keys.ToList();
        int totalPeople = People.Count;
        int target = totalPeople / 2;
        List<Attribute> bestSubset = null;
        int closestCount = 0;

        // Iterate over all possible subsets of the attributes (using bitwise operations)
        for (int i = 1; i < (1 << allAttributes.Count); i++) {
            // Initialize an empty list for the current subset and a counter for covered people
            List<Attribute> currentSubset = new List<Attribute>();
            int coveredPeople = 0;

            // Loop through each attribute (checking each bit of i)
            for (int j = 0; j < allAttributes.Count; j++) {
                // Check if the j-th bit of i is set (1), meaning this attribute is included in the current subset
                if ((i & (1 << j)) != 0) {
                    // Add the attribute to the current subset
                    currentSubset.Add(allAttributes[j]);

                    // For each person associated with the current attribute, increase the coveredPeople count
                    foreach (var _person in attributesToPeople[allAttributes[j]]) {
                        coveredPeople++;
                    }
                }
            }

            // Check if this subset is closer to the target number of covered people
            if (Math.Abs(coveredPeople - target) < Math.Abs(closestCount - target)) {
                // Update closestCount and bestSubset if the current subset covers people closer to the target
                closestCount = coveredPeople;
                bestSubset = currentSubset;
            }
        }
        
        // record the guess so we can resolve it
        lastGuess = bestSubset;
        
        // create a question for the user to ask.
        string output = "Does your person have ";

        Debug.Assert(bestSubset != null, nameof(bestSubset) + " != null");

        string separator = "or, ";
        
        foreach (Attribute attribute in bestSubset) {
            output += attribute.ToString() + separator;
        }

        output = string.Concat(output.AsSpan(0, output.Length - separator.Length), "?");
        
        Print(output);

    }

    private static void ResolveGuess(string[] input) {
        
        
        List<string> trueInputs = new List<string> {
            "true", "y", "yes", "1"
        };
        List<string> falseInputs = new List<string> {
            "false", "n", "no", "0"
        };

        if (input.Length < 2) {
            Print("You must input yes or no after the resolve command.");
            return;
        }

        if (lastGuess == null) {
            Print("you must make a guess to resolve it.");
            return;
        }

        string doResolve = input[1].ToLower();

        List<Person> peopleToRemove = new List<Person>();
        
        // if true, remove people without the attribute
        if (trueInputs.Contains(doResolve)) {
            foreach (Person person in People) {
                if (!person.HasAttributes.Intersect(lastGuess).Any()) {
                    peopleToRemove.Add(person);
                }
            }
        }
        // if false, remove people with the attribute
        else if (falseInputs.Contains(doResolve)) {
            foreach (Person person in People) {
                if (person.HasAttributes.Intersect(lastGuess).Any()) {
                    peopleToRemove.Add(person);
                }
            }
        }
        // anything else, we don't know what to do.
        else {
            Print("Invalid flag. Please input yes or no after the resolve command.");
            return;
        }

        foreach (Person person in peopleToRemove) {
            People.Remove(person);
        }

        lastGuess = null;
    }

    private static void Stop(string[] _input) {
        running = false;
    }

    public static void Print(string? msg = null) {
        if (msg == null) {
            Console.WriteLine();
        }
        else {
            Console.WriteLine(msg);
        }
    }

    public static string? Input(string? prompt = null) {
        if (prompt != null) {
            Console.Write(prompt);
        }
        
        return Console.ReadLine();
    }
}