using System.Collections;
using System.Collections.Generic;

public class Quantity{

    public int inip { get; set; }
    public int endp { get; set; }
    public string unit { get; set; }
    public int amount { get; set; }

    public Quantity(string unit, int amount, int inip, int endp)
    {
        this.unit = unit;
        this.amount = amount;
        this.inip = inip;
        this.endp = endp;
    }
}
