using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Menus")]
    [SerializeField] private GameObject menuActive;
    [SerializeField] private GameObject menuPause;
    [SerializeField] private GameObject menuWin;
    [SerializeField] private GameObject menuLose;

    [Header("Player UI")]
    public Image playerHPBar;
    public GameObject playerDamageScreen;
    public TMP_Text gameGoalCountText;

    [Header("Player Reference")]
    public GameObject player;

    public bool isPaused;

    private int gameGoalCount;
    private float timeScaleOrig;

    private void Awake()
    {
        instance = this;

        Time.timeScale = 1f;
        timeScaleOrig = Time.timeScale;

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else
            {
                StateUnpause();
            }
        }
    }

    public void StatePause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    public void UpdateGameGoal(int amount)
    {
        gameGoalCount += amount;

        if (gameGoalCountText != null)
        {
            gameGoalCountText.text = gameGoalCount.ToString("F0");
        }

        if (amount < 0 && gameGoalCount <= 0)
        {
            YouWin();
        }
    }

    public void YouWin()
    {
        StatePause();
        menuActive = menuWin;

        if (menuActive != null)
        {
            menuActive.SetActive(true);
        }
    }

    public void YouLose()
    {
        StatePause();
        menuActive = menuLose;

        if (menuActive != null)
        {
            menuActive.SetActive(true);
        }
    }
}