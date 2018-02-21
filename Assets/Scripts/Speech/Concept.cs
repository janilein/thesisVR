using System.Collections;
using System.Collections.Generic;

public class Concept{

    public string form { get; set; }
    public string type { get; set; }
    public int inip { get; set; }
    public int endp { get; set; }

    public Concept(string form, string type, int inip, int endp)
    {
        this.form = form;
        this.type = type;
        this.inip = inip;
        this.endp = endp;
    }

}
