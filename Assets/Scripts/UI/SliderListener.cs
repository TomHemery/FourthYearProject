using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderListener : MonoBehaviour
{
    public static SliderListener Instance { get; private set; } = null;

    public Slider DensitySlider;
    public Slider QualitySlider;
    public Slider ThresholdSlider;

    public Text DensityText;
    public Text QualityText;
    public Text ThresholdText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        DensityText.text = "" + DensitySlider.value;
        QualityText.text = "" + QualitySlider.value;
        ThresholdText.text = "" + ThresholdSlider.value;
    }

    public void OnDensitySliderChanged() {
        foreach (VolumeBehaviour v in VolumeBehaviour.AllRenderingVolumes) v.SetDensity(DensitySlider.value);
        DensityText.text = "" + DensitySlider.value;
    }

    public void OnQualitySliderChanged() {
        foreach (VolumeBehaviour v in VolumeBehaviour.AllRenderingVolumes) v.SetQuality((int)QualitySlider.value);
        QualityText.text = "" + QualitySlider.value;
    }

    public void OnThresholdSliderChanged() {
        foreach (VolumeBehaviour v in VolumeBehaviour.AllRenderingVolumes) v.SetThreshold(ThresholdSlider.value);
        ThresholdText.text = "" + ThresholdSlider.value;
    }

    public void SetDensityValue(float density) {
        DensitySlider.value = density;
    }

    public void SetQualityValue(int quality) {
        QualitySlider.value = quality;
    }

    public void SetThresholdValue(float threshold) {
        ThresholdSlider.value = threshold;
    }
}
