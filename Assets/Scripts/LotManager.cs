using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LotManager : MonoBehaviour
{
    private static LotManager lotManager;

    public GameObject lot = null;

    public static LotManager instance {
        get {
            if (!lotManager)
            {
                lotManager = FindObjectOfType(typeof(LotManager)) as LotManager;
                EditorGUIUtility.PingObject(lotManager);

                if (!lotManager)
                {
                    Debug.LogError("There needs to be one active LotManger script on a GameObject in your scene.");
                }
            }

            return lotManager;
        }
    }

    public static GameObject getLot()
    {
        return instance.lot;
    }


}
