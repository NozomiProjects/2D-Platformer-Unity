using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private TMP_Text coinText;
    [SerializeField] public PlayerController playerController;

    private int coinCount = 0;
    private int gemCount = 0;
    private bool isGameOver = false;
    private Vector3 playerPosition;

    // Level Complete
    [SerializeField] public GameObject levelCompletePanel;
    [SerializeField] public TMP_Text leveCompletePanelTitle;
    [SerializeField] public TMP_Text levelCompleteCoins;

    private int totalCoins = 0;

    private void Awake()
    {
        // Asegúrate de que solo haya una instancia de GameManager
        if (instance == null)
        {
            instance = this;
            Application.targetFrameRate = 60;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Verificar que las referencias no sean nulas
        if (coinText == null)
        {
            Debug.LogError("coinText is not assigned in GameManager.");
        }
        if (playerController == null)
        {
            Debug.LogError("playerController is not assigned in GameManager.");
        }
        if (levelCompletePanel == null)
        {
            Debug.LogError("levelCompletePanel is not assigned in GameManager.");
        }
        if (leveCompletePanelTitle == null)
        {
            Debug.LogError("leveCompletePanelTitle is not assigned in GameManager.");
        }
        if (levelCompleteCoins == null)
        {
            Debug.LogError("levelCompleteCoins is not assigned in GameManager.");
        }

        UpdateGUI();
        UIManager.instance.fadeFromBlack = true;
        playerPosition = playerController.transform.position;

        FindTotalPickups();
    }

    public void IncrementCoinCount()
    {
        coinCount++;
        UpdateGUI();
    }

    public void IncrementGemCount()
    {
        gemCount++;
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        if (coinText != null)
        {
            coinText.text = coinCount.ToString();
        }
    }

    public void Death()
    {
        if (!isGameOver)
        {
            // Disable Mobile Controls
            UIManager.instance.DisableMobileControls();
            // Initiate screen fade
            UIManager.instance.fadeToBlack = true;

            // Disable the player object
            playerController.gameObject.SetActive(false);

            // Start death coroutine to wait and then respawn the player
            StartCoroutine(DeathCoroutine());

            // Update game state
            isGameOver = true;

            // Log death message
            Debug.Log("Died");
        }
    }

    public void FindTotalPickups()
    {
        pickup[] pickups = GameObject.FindObjectsOfType<pickup>();

        foreach (pickup pickupObject in pickups)
        {
            if (pickupObject.pt == pickup.pickupType.coin)
            {
                totalCoins += 1;
            }
        }
    }

    public void LevelComplete()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            leveCompletePanelTitle.text = "LEVEL COMPLETE";
            levelCompleteCoins.text = "COINS COLLECTED: " + coinCount.ToString() + " / " + totalCoins.ToString();
        }
    }

    public IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1f);
        playerController.transform.position = playerPosition;

        // Wait for 2 seconds
        yield return new WaitForSeconds(1f);

        // Check if the game is still over (in case player respawns earlier)
        if (isGameOver)
        {
            SceneManager.LoadScene(1);
        }
    }
}