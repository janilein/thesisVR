namespace callback {

    using UnityEngine;
    using UnityEngine.UI;

    public class CallbackRotateUI : CallbackRotate {

        private Text degreesText;
        private Slider slider;
        private float degrees;

        private void Awake() {
            Manager = GameObject.Find("GUI").transform.GetComponent<menumanager.UIManager>();
            degreesText = GameObject.Find("GUI/Canvas/RotateButtons/Text").GetComponent<Text>();
            slider = GameObject.Find("GUI/Canvas/RotateButtons/Slider").GetComponent<Slider>();
        }

        public void SliderValueChanged() {
            degreesText.text = slider.value + " degrees";
            degrees = slider.value;
        }

        public void PitchObjectLeft() {
            ObjectManager.RotateObject(new Vector3(0f, degrees, 0f));
        }

        public void PitchObjectRight() {
            ObjectManager.RotateObject(new Vector3(0f, -degrees, 0f));
        }

        public void RollObjectLeft() {
            ObjectManager.RotateObject(new Vector3(degrees, 0f, 0f));
        }

        public void RollObjectRight() {
            ObjectManager.RotateObject(new Vector3(-degrees, 0f, 0f));
        }

        public void YawObjectLeft() {
            ObjectManager.RotateObject(new Vector3(0f, 0f, degrees));
        }

        public void YawObjectRight() {
            ObjectManager.RotateObject(new Vector3(0f, 0f , -degrees));
        }

        public void Back() {

            Manager.ToggleObjectActive("GUI/Canvas/RotateButtons", false);
            Manager.ToggleObjectActive("GUI/Canvas/DefaultButtons", true);
        }
    }
}
