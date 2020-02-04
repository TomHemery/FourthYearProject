using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderListener : MonoBehaviour
{

    public Slider DensitySlider;
    public Slider QualitySlider;

    public Text DensityText;
    public Text QualityText;

    void Start()
    {
        OnDensitySliderChanged();
        OnQualitySliderChanged();
    }

    public void OnDensitySliderChanged() {
        VolumeManager.Instance.SetDensity(DensitySlider.value);
        DensityText.text = "" + DensitySlider.value;
    }

    public void OnQualitySliderChanged() {
        VolumeManager.Instance.SetQuality((int)QualitySlider.value);
        QualityText.text = "" + QualitySlider.value;
    }
}
