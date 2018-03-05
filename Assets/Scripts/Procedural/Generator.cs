using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Generator {

    public virtual GameObject GenerateWorldObject(WorldObject obj, Vector3 currentDirection, string JSON = null) { return null; }
    public virtual void GenerateWorldObject(WorldObject obj, Vector3 currentDirection, ref Vector3 currentPosition, string pointDirection) { }
    public virtual void GenerateWorldObject(WorldObject obj, ref Vector2 currentDirection, ref Vector3 currentPosition, string pointDirection) { }

}
