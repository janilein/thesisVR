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
                Speech.SetSpecification(false); //Automatically move to new description
            }

            instance.lot = lot;
            lot.GetComponent<SelectableObject>().HighlightObject();

            if (!SaveManager.loadingGame)
            {   
                string lotFullName = lot.transform.name;
                Transform parent = lot.transform.parent;
                while (parent != null)
                {
                    lotFullName = parent.name + "/" + lotFullName;
                    parent = parent.parent;
                }
                SaveManager.CreateSelectLotCommand(lotFullName);
            }
            
            
        }
    }

    public static float GetDirection()
    {
        if (instance.lot)
        {
            float lotRotation;// = instance.lot.transform.rotation.eulerAngles.y;

            //Now check if we are left or right of the street (by calculating the angle) and then adding + or - 90°
            Vector3 posLot = instance.lot.transform.position;
            Vector3 streetLot = instance.lot.transform.parent.position;
            float degree = Mathf.Rad2Deg * Mathf.Atan2(streetLot.z - posLot.z, streetLot.x - posLot.x);
            degree %= 360;

            //Angle between -90° and 90° --> Add 90, otherwise subtract 90
            if (degree > -180 && degree < 0)
                lotRotation = 180;
            else
                lotRotation = 0;

            return lotRotation;

        }
        return 0;
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
