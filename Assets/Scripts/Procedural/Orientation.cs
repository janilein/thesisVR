using System;
	public enum Orientation
	{
	left,
	right,
	straight, 
    back
	}

public class OrientationEnumFunctions{

	public static bool LegitOrientation(object orientationName){
		return System.Enum.IsDefined (typeof(Orientation), orientationName);
	}

	public static Orientation GetOrientationFromString(string s){
		s = s.ToLower ();
		foreach (Orientation orientation in Enum.GetValues(typeof(Orientation))) {
			if (s.Equals (orientation.ToString ()))
				return orientation;
		}
		throw new Exception ("Orientation not found for string: " + s);
	}
}

