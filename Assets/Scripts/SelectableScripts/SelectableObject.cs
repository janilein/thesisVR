using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectableObject : MonoBehaviour {

    public abstract void HighlightObject();
    public abstract void RemoveHighlight();

}
