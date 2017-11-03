namespace callback {

    using UnityEngine;
    using UnityEngine.UI;

    public class CallbackMoveUI : CallbackMove {

        private Text distanceText;
        private Slider slider;
        private float distance;

        private void Awake() {
            Manager = GameObject.Find("GUI").transform.GetComponent<menumanager.UIManager>();
            distanceText = GameObject.Find("GUI/Canvas/MoveButtons/Text").GetComponent<Text>();
            slider = GameObject.Find("GUI/Canvas/MoveButtons/Slider").GetComponent<Slider>();
        }

        public void SliderValueChanged() {
            distanceText.text = slider.value + " units";
            distance = slider.value;
        }

        public void MoveObjectLeft() {
            ObjectManager.MoveObject(0, 0, distance);
            Debug.Log("Callback move object left called");
        }

        public void MoveObjectRight() {
            ObjectManager.MoveObject(0, 0, -distance);
            Debug.Log("Callback move object right called");
        }

        public void MoveObjectUp() {
            ObjectManager.MoveObject(0, distance, 0);
            Debug.Log("Callback move object up called");
        }

        public void MoveObjectDown() {
            ObjectManager.MoveObject(0, -distance, 0);
            Debug.Log("Callback move object down called");
        }

        public void MoveObjectFurther() {
            ObjectManager.MoveObject(distance, 0, 0);
            Debug.Log("Callback move object further called");
        }

        public void MoveObjectNearer() {
            ObjectManager.MoveObject(-distance, 0, 0);
            Debug.Log("Callback move object nearer called");
        }

        public void Back() {
            Manager.ToggleObjectActive("GUI/Canvas/MoveButtons", false);
            Manager.ToggleObjectActive("GUI/Canvas/DefaultButtons", true);

            Debug.Log("Callback back called");
        }

    }

}