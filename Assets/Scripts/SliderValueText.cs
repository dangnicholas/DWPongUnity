using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueText : MonoBehaviour
{

    //[SerializeField] private Slider slider;
    //[SerializeField] private TextMeshProUGUI sliderText;
    private Slider slider;
    private Text textComp;

    void Awake()
    {
        slider = GetComponentInParent<Slider>();
        textComp = GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start() {
       //UpdateText(slider.value);
        //slider.onValueChanged.AddListener(UpdateText);
        //Debug.Log("NOT WORKING START" + slider.value.ToString());

    }

    public void UpdateText(float val)
    {
        //textComp.text = slider.value.ToString();
        //Debug.Log("NOT WORKING " + slider.value.ToString());
        //sliderText.text = slider.value.ToString();
    }
}
