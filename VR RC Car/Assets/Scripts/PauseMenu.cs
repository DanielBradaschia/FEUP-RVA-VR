using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    InputMaster controls;
    int currentOption;

    [SerializeField]
    Vector2[] positions;
    [SerializeField]
    Image Indicator;
    [SerializeField]
    Light directionalL;
    [SerializeField]
    Light spotL;
    [SerializeField]
    Transform player;
    [SerializeField]
    GameObject canvas;

    void Pause()
    {
        Time.timeScale = 0f;
        transform.position = new Vector3(player.position.x, player.position.y, player.position.z);
        directionalL.enabled = false;
        spotL.enabled = true;
        canvas.SetActive(true);
        isPaused = true;
    }

    void Unpause()
    {
        Time.timeScale = 1f;
        directionalL.enabled = true;
        spotL.enabled = false;
        canvas.SetActive(false);
        isPaused = false;
    }

    void PauseButton()
    {
        if (!isPaused)
        {
            Pause();
        }
        else
        {
            Unpause();
        }
    }

    void SwapOption(float value)
    {
        currentOption += (int)value;

        if (currentOption == positions.Length)
            currentOption = 0;
        if (currentOption == -1)
            currentOption = positions.Length - 1;

        Indicator.GetComponent<RectTransform>().anchoredPosition = positions[currentOption];
    }

    void SelectOption()
    {
        switch (currentOption)
        {
            case 0:
                Unpause();
                break;
            case 1:
                Debug.Log("Show how to play the game");
                break;
            case 2:
                SceneManager.LoadScene(0);
                break;
            case 3:
                Application.Quit();
                Debug.Log("Quit Game");
                break;
        }
    }
    

    void Awake()
    {
        isPaused = false;
        controls = new InputMaster();
        currentOption = 0;
        Indicator.GetComponent<RectTransform>().anchoredPosition = positions[currentOption];

        controls.Menu.Pause.performed += _ => PauseButton();
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
