using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSelector : MonoBehaviour {

    public LayerMask teleportMask;
    private LotManager lotManager;
    private SteamVR_TrackedObject trackedObj;
    private Vector3 hitPoint;
    private LineRenderer lineRenderer;
    private Material laserMaterial;
    private bool pointerEnabled = true;
    private bool shouldSelect = false;
    private float lookDistance = 100f;

    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    // Use this for initialization
    void Awake () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        lineRenderer = GetComponent<LineRenderer>();
        laserMaterial = lineRenderer.material;

        lotManager = GameObject.Find("ObjectManager").transform.GetComponent<LotManager>();
        if (!lotManager)
        {
            Debug.Log("Didn't find lotManager script!");
        }
    }

    private void ShowLaser(RaycastHit hit)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, trackedObj.transform.position);
        lineRenderer.SetPosition(1, hit.point);
        laserMaterial.color = Color.green;
    }

    private void ShowLaserNoHit()
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, trackedObj.transform.position);
        lineRenderer.SetPosition(1, trackedObj.transform.forward * lookDistance + trackedObj.transform.position);
        laserMaterial.color = Color.red;
    }

    // Update is called once per frame
    void Update () {
        if (pointerEnabled)
        {
            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
                RaycastHit hit;
                if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, lookDistance, teleportMask))
                {
                    hitPoint = hit.point;
                    ShowLaser(hit);
                    shouldSelect = true;
                }
                else
                {
                    shouldSelect = false;
                    ShowLaserNoHit();
                }
                if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldSelect)
                {
                    Debug.Log("Lot selected");
                    SelectLot(hit);
                }
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
    }

    private void SelectLot(RaycastHit hit)
    {
        lotManager.lot = hit.collider.gameObject;
        hit.collider.gameObject.GetComponent<SelectableObject>().HightlightObject();
    }
}
