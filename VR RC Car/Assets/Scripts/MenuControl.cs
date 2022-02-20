using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class MenuControl : MonoBehaviour
{
    InputMaster controls;
    int currentOption;

    public Vector2[] positions;
    public Image Indicator;

    void SwapOption(float value)
    {
        Debug.Log(value);
        currentOption += (int)value;

        if (currentOption == positions.Length)
            currentOption = 0;
        if (currentOption == -1)
            currentOption = positions.Length - 1;

        Indicator.GetComponent<RectTransform>().anchoredPosition = positions[currentOption];
    }

    void SelectOption()
    {
        switch(currentOption)
        {
            case 0:
                SceneManager.LoadScene(1);
                break;
            case 1:
                Debug.Log("Show how to play the game");
                break;
            case 2:
                Application.Quit();
                Debug.Log("Quit Game");
                break;
        }
    }


    void Awake()
    {
        controls = new InputMaster();
        currentOption = 0;
        Indicator.GetComponent<RectTransform>().anchoredPosition = positions[currentOption];

        controls.Menu.ChooseOption.performed += ctx => SwapOption(ctx.ReadValue<float>());
        controls.Menu.Select.performed += _ => SelectOption();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }
}
