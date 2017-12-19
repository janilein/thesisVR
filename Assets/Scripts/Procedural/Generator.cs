﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Generator {

    public virtual void GenerateWorldObject(WorldObject obj, Vector3 currentDirection) { }
    public virtual void GenerateWorldObject(WorldObject obj, Vector3 currentDirection, ref Vector3 currentPosition, string pointDirection) { }
}
