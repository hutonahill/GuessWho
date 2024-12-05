namespace GuessWho;



public class Person(HashSet<Attribute> hasAttributes, string name) {
    public HashSet<Attribute> HasAttributes { get; private set; } = hasAttributes;

    public string Name { get; private set; } = name;
}