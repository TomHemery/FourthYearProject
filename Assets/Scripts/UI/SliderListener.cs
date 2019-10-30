using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderListener : MonoBehaviour
{

    public Slider DensitySlider;
    public Slider QualitySlider;
    public Slider SpacingSlider;
    public Slider XSliceSlider;

    public Text DensityText;
    public Text QualityText;
    public Text SpacingText;
    public Text XSliceText;

    void Start()
    {
        Debug.Log("Slider on start");

        OnDensitySliderChanged();
        OnQualitySliderChanged();
        OnSpacingSliderChanged();
        OnXSliceSliderChanged();
    }

    public void OnDensitySliderChanged() {
        VolumeManager.Instance.SetDensity(DensitySlider.value);
        DensityText.text = "" + DensitySlider.value;
    }

    public void OnQualitySliderChanged() {
        VolumeManager.Instance.SetQuality((int)QualitySlider.value);
        QualityText.text = "" + QualitySlider.value;
    }

    public void OnSpacingSliderChanged() {
        VolumeManager.Instance.SetXScale(SpacingSlider.value);
        SpacingText.text = "" + SpacingSlider.value;
    }

    public void OnXSliceSliderChanged()
    {
        VolumeManager.Instance.SetXCrop(XSliceSlider.value);
        XSliceText.text = "" + XSliceSlider.value;
    }
}
