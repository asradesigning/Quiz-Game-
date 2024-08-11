using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionController : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown; // Reference to the Dropdown component

    private Resolution[] availableResolutions;

    void Start()
    {
        // Fetch all available screen resolutions
        availableResolutions = Screen.resolutions;

        // Clear current options
        resolutionDropdown.ClearOptions();

        // Create a list to hold the resolution options in string format
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            string option = availableResolutions[i].width + " x " + availableResolutions[i].height;
            options.Add(option);

            // Check if this resolution is the current screen resolution
            if (availableResolutions[i].width == Screen.currentResolution.width &&
                availableResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // Add the resolutions to the dropdown
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Add listener to handle resolution change
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int resolutionIndex)
    {
        // Get the resolution from the available resolutions list
        Resolution selectedResolution = availableResolutions[resolutionIndex];

        // Set the resolution
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
    }
}
