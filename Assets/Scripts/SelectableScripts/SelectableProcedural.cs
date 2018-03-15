using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableProcedural : SelectableObject {

    public override void HighlightObject()
    {
        Debug.Log("enable halo");
        Component halo = GetComponent("Halo");
        halo.GetType().GetProperty("enabled").SetValue(halo, true, null);
    }

    public override void RemoveHighlight()
    {
        Debug.Log("disable halo");
        Component halo = GetComponent("Halo");
        halo.GetType().GetProperty("enabled").SetValue(halo, false, null);
    }
}
