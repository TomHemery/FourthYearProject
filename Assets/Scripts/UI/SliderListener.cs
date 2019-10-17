using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderListener : MonoBehaviour
{

    public Slider DensitySlider;
    public Slider QualitySlider;
    public Slider SpacingSlider;

    public Text DensityText;
    public Text QualityText;
    public Text SpacingText;

    void Start()
    {
        OnDensitySliderChanged();
        OnQualitySliderChanged();
    }

    public void OnDensitySliderChanged() {
        VolumeLoader.Instance.SetDensity(DensitySlider.value);
        DensityText.text = "" + DensitySlider.value;
    }

    public void OnQualitySliderChanged() {
        VolumeLoader.Instance.SetQuality((int)QualitySlider.value);
        QualityText.text = "" + QualitySlider.value;
    }

    public void OnSpacingSliderChanged() {
        VolumeLoader.Instance.SetXScale(SpacingSlider.value);
        SpacingText.text = "" + SpacingSlider.value;
    }
}
