using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderListener : MonoBehaviour
{

    public Slider DensitySlider;
    public Slider QualitySlider;
    public Slider ThresholdSlider;

    public Text DensityText;
    public Text QualityText;
    public Text ThresholdText;

    void Start()
    {
        OnDensitySliderChanged();
        OnQualitySliderChanged();
        OnThresholdSliderChanged();
    }

    public void OnDensitySliderChanged() {
        foreach (VolumeBehaviour v in VolumeBehaviour.AllVolumes) v.SetDensity(DensitySlider.value);
        DensityText.text = "" + DensitySlider.value;
    }

    public void OnQualitySliderChanged() {
        foreach (VolumeBehaviour v in VolumeBehaviour.AllVolumes) v.SetQuality((int)QualitySlider.value);
        QualityText.text = "" + QualitySlider.value;
    }

    public void OnThresholdSliderChanged() {
        foreach (VolumeBehaviour v in VolumeBehaviour.AllVolumes) v.SetThreshold(ThresholdSlider.value);
        ThresholdText.text = "" + ThresholdSlider.value;
    }
}
