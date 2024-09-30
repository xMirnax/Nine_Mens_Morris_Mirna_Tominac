using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField player1NameInput;
    [SerializeField] private TMP_Dropdown player1ColorDropdown;
    [SerializeField] private TMP_InputField player2NameInput;
    [SerializeField] private TMP_Dropdown player2ColorDropdown;
    [SerializeField] private TMP_InputField ringsInputField;
    [SerializeField] private Button applyButton;

    private int gridSize;

    private SettingsManager settingsManager;
    private Dictionary<string, Color> colorMapping;

    private void Start()
    {
        settingsManager = SettingsManager.Instance;
        player1NameInput.text = settingsManager.player1.name;
        player2NameInput.text = settingsManager.player2.name;
        ringsInputField.text = settingsManager.GetGridConfig().numberOfRings.ToString();
        applyButton.onClick.AddListener(ApplySettings);
        ringsInputField.contentType = TMP_InputField.ContentType.IntegerNumber;

        colorMapping = new Dictionary<string, Color>
        {
            { "Red", Color.red },
            { "Blue", Color.blue },
            { "Green", Color.green },
            { "White", Color.white }
        };

        player1NameInput.onValueChanged.AddListener((str) => ValidateData());
        player2NameInput.onValueChanged.AddListener((str) => ValidateData());
        player1ColorDropdown.onValueChanged.AddListener((val) => ValidateData());
        player2ColorDropdown.onValueChanged.AddListener((val) => ValidateData());

        PopulateColorDropdown(player1ColorDropdown);
        PopulateColorDropdown(player2ColorDropdown);

        player2ColorDropdown.value = 1;

        ValidateData();
    }

    private void ValidateData()
    {
        bool invalid = false;
        if(player1NameInput.text == player2NameInput.text)
        {
            invalid = true;
        }

        if(player1ColorDropdown.value == player2ColorDropdown.value)
        {
            invalid = true;
        }

        // TODO We could also validate ring size not to be negative or too big

        applyButton.gameObject.SetActive(!invalid); 
    }

    private void PopulateColorDropdown(TMP_Dropdown dropdown)
    {
        dropdown.ClearOptions();
        List<string> colorNames = new List<string>(colorMapping.Keys);
        dropdown.AddOptions(colorNames); 
    }

    public void ApplySettings()
    {
        PlayerData player1Data = new PlayerData
        {
            name = player1NameInput.text,
            color = GetColorFromDropdown(player1ColorDropdown)
        };
        settingsManager.SetPlayerData(Player.Player1, player1Data);

        PlayerData player2Data = new PlayerData
        {
            name = player2NameInput.text,
            color = GetColorFromDropdown(player2ColorDropdown)
        };
        settingsManager.SetPlayerData(Player.Player2, player2Data);

        int numberOfRings;
        if (int.TryParse(ringsInputField.text, out numberOfRings) && numberOfRings >= 1)
        {
            GridConfig gridConfig = new GridConfig();
            gridConfig.numberOfRings = numberOfRings;
            gridConfig.gridSize = 10;
            gridConfig.ringGap = 10;
            settingsManager.SetGridConfig(gridConfig);
        }
        else
        {
            Debug.LogError("Invalid input for number of rings. It must be a number greater than 0.");
        }
    }

    private Color GetColorFromDropdown(TMP_Dropdown dropdown)
    {
        string selectedColorName = dropdown.options[dropdown.value].text;
        return colorMapping[selectedColorName];
    }
}
