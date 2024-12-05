namespace GuessWho;

// attributes are things like has glasses, or red hair or black skin.
// things the people have that we can ask questions about.

public interface Attribute {
    
    public string ToString();
}

// attributes should be singletons. 
// I would rather make them static, but c# has no option to do this.
public class RedHair : Attribute {
    // private constructor to prevent creating multiple instances
    private RedHair() { }

    // Public static property to access the single instance
    public static RedHair Instance { get; private set; } = new RedHair();
    
    public override string ToString() {
        return "red hair";
    }
}
