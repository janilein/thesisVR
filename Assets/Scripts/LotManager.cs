using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public static void setLot(GameObject lot)
    {
        if (lot.Equals(instance.lot))
            return;
        else
        {
            if (instance.lot)
            {
                instance.lot.GetComponent<SelectableObject>().RemoveHighlight();
            }

            instance.lot = lot;
            lot.GetComponent<SelectableObject>().HighlightObject();
            
            
        }
    }

    public static void DeselectLot()
    {
        if (instance.lot)
        {
            instance.lot.GetComponent<SelectableObject>().RemoveHighlight();
            instance.lot = null;
        }
    }


}
