using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class SettingsManager
{

    private const string SETTINGS_TAG = "settings";
    private const string DENSITY_TAG = "density";
    private const string QUALITY_TAG = "samplingQuality";
    private const string THRESHOLD_TAG = "threshold";
    private const string SOUND_EFFECT_TAG = "soundEffect";

    public static VolumeSettings LoadSettingsFromXML(string name) {
        string path = Directory.GetCurrentDirectory() + VolumeBehaviour.VOLUMETRIC_DATA_PATH + name + ".xml";
        VolumeSettings returnedSettings = new VolumeSettings();

        XmlDocument doc = new XmlDocument
        {
            PreserveWhitespace = false
        };

        try { 
            doc.Load(path);
            XmlNodeList settingNoes = doc.GetElementsByTagName(SETTINGS_TAG);
            if (settingNoes.Count > 0)
            {
                XmlNode settings = settingNoes[0];
                foreach (XmlNode setting in settings.ChildNodes) {
                    Debug.Log(setting.Name);
                    switch (setting.Name) {
                        case DENSITY_TAG:
                            returnedSettings.density = float.Parse(setting.InnerText);
                            break;
                        case QUALITY_TAG:
                            returnedSettings.samplingQuality = int.Parse(setting.InnerText);
                            break;
                        case THRESHOLD_TAG:
                            returnedSettings.threshold = float.Parse(setting.InnerText);
                            break;
                        case SOUND_EFFECT_TAG:
                            returnedSettings.soundEffectName = setting.InnerText;
                            break;
                    }
                }
            }
            else 
            {
                Debug.LogError("Error, no setting elements found for: " + name + " in: " + path);
            }
        }

        //no settings exist
        catch (FileNotFoundException)
        { 
            Debug.Log("File not found: " + name + " in: " + path);
            returnedSettings.density = 1.0f;
            returnedSettings.samplingQuality = 64;
            returnedSettings.threshold = 0.0f;
        }

        return returnedSettings;
    }

    public static void SaveSettingsToXML(string name, VolumeSettings settings) {
        string path = Directory.GetCurrentDirectory() + VolumeBehaviour.VOLUMETRIC_DATA_PATH + name + ".xml";

        XmlWriter writer = XmlWriter.Create(path, new XmlWriterSettings() { 
            NewLineOnAttributes = true,
            Indent = true
        });

        writer.WriteStartDocument();
        writer.WriteStartElement(SETTINGS_TAG);

        writer.WriteStartElement(DENSITY_TAG);
        writer.WriteString("" + settings.density);
        writer.WriteEndElement();

        writer.WriteStartElement(THRESHOLD_TAG);
        writer.WriteString("" + settings.threshold);
        writer.WriteEndElement();

        writer.WriteStartElement(QUALITY_TAG);
        writer.WriteString("" + settings.samplingQuality);
        writer.WriteEndElement();

        writer.WriteStartElement(SOUND_EFFECT_TAG);
        writer.WriteString("" + settings.soundEffectName);
        writer.WriteEndElement();

        writer.WriteEndDocument();
        writer.Close();
    }
}

public struct VolumeSettings
{
    public int samplingQuality;
    public float threshold;
    public float density;
    public string soundEffectName;
}
