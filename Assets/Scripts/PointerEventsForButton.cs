using spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointerEventsForButton : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler {

    private GameObject preview = null;
    private SpawningManager spawningManager;

    private void Awake() {
        preview = GameObject.Find("GUI/Canvas/SpawnButtons/Preview");
        spawningManager = GameObject.Find("ObjectManager").GetComponent<SpawningManagerUI>();
        if (!spawningManager) {
            Debug.Log("No UI Spawning manager found");
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        

        preview.SetActive(true);
        GameObject enteredObject = eventData.pointerEnter;
        string enteredButton = enteredObject.transform.GetComponentInChildren<Text>().text;
        string activePart = spawningManager.GetActivePart();
        //Debug.Log("Active part: " + activePart);
        string loadPath = activePart + "/" + enteredButton;
        Debug.Log("Load path: " + loadPath);
        //Sprite sprite = Resources.Load(loadPath) as Sprite ;
        Sprite sprite = Resources.Load<Sprite>(loadPath);
        if (!sprite) {
            Debug.Log("Sprite is null");
        }
        preview.GetComponent<Image>().sprite = sprite;

        Debug.Log("entered button: " + enteredButton);
    }

    public void OnPointerExit(PointerEventData eventData) {
        preview.SetActive(false);
        //throw new System.NotImplementedException();
    }
}
