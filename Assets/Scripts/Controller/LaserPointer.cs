using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserPointer : MonoBehaviour {

    
    //public GameObject laserPrefab;
    public Transform cameraRigTransform;
    public GameObject teleportReticlePrefab;
    public Transform headTransform;
    public Vector3 teleportReticleOffset;
    public LayerMask teleportMask;

    private SteamVR_TrackedObject trackedObj;
    //private GameObject laser;
    //private Transform laserTransform;
    private Vector3 hitPoint;
    private GameObject reticle;
    private Transform teleportReticleTransform;
    private bool shouldTeleport;

    private LineRenderer lineRenderer;
    private Material laserMaterial;
    
    private bool pointerEnabled = true;
    private float lookDistance = 100f;

    void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        lineRenderer = GetComponent<LineRenderer>();
        laserMaterial = lineRenderer.material;
    }

    private void Start() {
        //laser = Instantiate(laserPrefab);
        //laserTransform = laser.transform;
        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;
    }

    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void ShowLaser(RaycastHit hit) {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, trackedObj.transform.position);
        lineRenderer.SetPosition(1, hit.point);
        laserMaterial.color = Color.green;
        //laser.SetActive(true);
        //laser.transform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, 0.5f);
        //laserTransform.LookAt(hitPoint);
        //laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

    private void ShowLaserNoHit() {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, trackedObj.transform.position);
        lineRenderer.SetPosition(1, trackedObj.transform.forward * lookDistance + trackedObj.transform.position);
        laserMaterial.color = Color.red;
        //laser.SetActive(true);
        //laser.transform.position = Vector3.Lerp(trackedObj.transform.position, trackedObj.transform.forward * lookDistance, 0.5f);
        //laserTransform.LookAt(trackedObj.transform.position);
        //laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, lookDistance);
    }

    // Update is called once per frame
    void Update () {
        if (pointerEnabled) {
            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {

                RaycastHit hit;

                if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, lookDistance, teleportMask)) {
                    hitPoint = hit.point;
                    ShowLaser(hit);
                    reticle.SetActive(true);
                    teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                    shouldTeleport = true;
                } else {
                    shouldTeleport = false;
                    reticle.SetActive(false);
                    ShowLaserNoHit();
                }
                if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport) {
                    Teleport();
                }
            } else {
                lineRenderer.enabled = false;
                //laser.SetActive(false);
                reticle.SetActive(false);
            }
        }
	}

    private void Teleport() {
        shouldTeleport = false;
        reticle.SetActive(false);
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        difference.y = 0;
        cameraRigTransform.position = hitPoint + difference;
    }
}
