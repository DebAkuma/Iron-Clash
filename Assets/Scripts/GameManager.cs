using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Global Variables")]
    public static bool isOnline;
    public static bool isHost;
    public static int playRounds = 3;
    public static string mapName;

    [Header("Character")]

    public static string selectedCharacter = "Eddy" ;
    public static string opponentCharacter = "Jin(OP)";

    [Header("Character Library")]
    [SerializeField] private List<CharacterData> characterList = new List<CharacterData>();
    public List<CharacterData> CharacterList => characterList;  // Public getter

    private Dictionary<string, CharacterData> characterLibrary = new Dictionary<string, CharacterData>();

    [SerializeField] private Transform[] spawnPoints;
    public static GameManager Instance;

    [Header("Rounds")]
    public int playerWins = 0;
    public int opponentWins = 0;

    private int maxWins = 2;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private GameObject resultPannel;

    [Header("Audio")]
    [SerializeField] private AudioClip[] gameSounds;
    [SerializeField]private AudioSource audioSource;

    [Header("Round Win Indicators")]
    [SerializeField] private GameObject[] playerRoundIndicators;
    [SerializeField] private GameObject[] opponentRoundIndicators;


    #region Starting Part
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildCharacterLibrary();
            SceneManager.sceneLoaded += OnSceneLoaded;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") return;

        if (scene.name == "Map1" || scene.name == "Map2" || scene.name == "Map3")
        {

            // Taking spwanpoint for spawning Players
            GameObject[] respawnObjects = GameObject.FindGameObjectsWithTag("Respawn");

            // Create new array and extract Transforms
            spawnPoints = new Transform[respawnObjects.Length];
            for (int i = 0; i < respawnObjects.Length; i++)
            {
                spawnPoints[i] = respawnObjects[i].transform;
            }

            if (isOnline)
            {
                return;
            }
            else
                SpawnCharacters(selectedCharacter, opponentCharacter);

            GameObject camObj = GameObject.FindWithTag("MainCamera");
            if (camObj != null)
            {
                CameraController camScript = camObj.GetComponent<CameraController>();
                camScript.FindPlayer();
            }
  
            GameUI ui = FindObjectOfType<GameUI>();
            ui.SetCharacterIcons(selectedCharacter, opponentCharacter);

            CheckRounds();
            // Rebind UI
            GameObject textObj = GameObject.FindWithTag("GameText");
            endText = textObj.GetComponent<TextMeshProUGUI>();
            StartCoroutine(ShowFightText());


            GameObject resultPanelObj = GameObject.FindWithTag("ResultUI");
           
            resultPannel = resultPanelObj;

            Button mainMenuBtn = resultPannel.transform.Find("OptionMenu/MainMenu").GetComponent<Button>();
            mainMenuBtn.onClick.RemoveAllListeners();
            mainMenuBtn.onClick.AddListener(LoadMainMenu);

            Button replayBtn = resultPannel.transform.Find("OptionMenu/Replay").GetComponent<Button>();
            replayBtn.onClick.RemoveAllListeners();
            replayBtn.onClick.AddListener(ResetGame);

            resultPannel.SetActive(false);
        

            // Re-fetch round indicators using tags
            playerRoundIndicators = GameObject.FindGameObjectsWithTag("WinIndicator");
            opponentRoundIndicators = GameObject.FindGameObjectsWithTag("OpponentWinIndicator");

            ResetRoundIndicators();
            UpdateRoundIndicators(); // In case you re-enter a scene after a win
        }
    }

    #endregion
    private IEnumerator ShowFightText()
    {
        if (endText != null)
        {
            endText.text = "FIGHT";
            endText.enabled = true;
            PlaySound(0);
            yield return new WaitForSeconds(.5f);
            endText.enabled = false;
        }
    }

    public void OnPlayerDefeated()
    {
        opponentWins++;
        StartCoroutine(EndRound("K.O", 1));
    }

    public void OnOpponentDefeated()
    {
        playerWins++;
        StartCoroutine(EndRound("K.O", 1));
    }

    private IEnumerator EndRound(string result, int soundIndex)
    {
        if (endText != null)
        {
            endText.text = result;
            endText.enabled = true;
        }

        PlaySound(soundIndex);
        yield return new WaitForSeconds(2f);

        if (playerWins >= maxWins)
        {
            UpdateRoundIndicators();
            endText.text = "PLAYER WINS!";
            PlaySound(3);
            yield return new WaitForSeconds(2f);
            Time.timeScale = 0f;
            ShowResultPanel();
        }
        else if (opponentWins >= maxWins)
        {
            UpdateRoundIndicators();
            endText.text = "OPPONENT WINS!";
            PlaySound(4);
            yield return new WaitForSeconds(2f);
            Time.timeScale = 0f;
            ShowResultPanel();
        }
        else
        {
            endText.text = "NEXT ROUND";
            PlaySound(2);
            yield return new WaitForSeconds(1.5f);
            endText.enabled = false;
            Time.timeScale = 0f;
            HideResultPanel();
            ReloadScene();
        }
    }

    private void ShowResultPanel()
    {
        if (resultPannel != null)
        {
            resultPannel.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HideResultPanel()
    {
        if (resultPannel != null)
        {
            resultPannel.SetActive(false);
        }
    }

    private void PlaySound(int index)
    {
        audioSource.PlayOneShot(gameSounds[index]);
    }

    private void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetGame()
    {
        playerWins = 0;
        opponentWins = 0;
        ResetRoundIndicators();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        playerWins = 0;
        opponentWins = 0;
        ResetRoundIndicators();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdateRoundIndicators()
    {
        for (int i = 0; i < maxWins; i++)
        {
            if (i < playerWins && i < playerRoundIndicators.Length)
            {
                Transform upper = playerRoundIndicators[i].transform.Find("UpperImage");
                upper.gameObject.SetActive(true);
            }

            if (i < opponentWins && i < opponentRoundIndicators.Length)
            {
                Transform upper = opponentRoundIndicators[i].transform.Find("UpperImage");
                upper.gameObject.SetActive(true);
            }
        }
    }

    private void ResetRoundIndicators()
    {
        foreach (GameObject indicator in playerRoundIndicators)
        {
            if (indicator == null) continue;
            Transform upper = indicator.transform.Find("UpperImage");
            if (upper != null) upper.gameObject.SetActive(false);
        }

        foreach (GameObject indicator in opponentRoundIndicators)
        {
            if (indicator == null) continue;
            Transform upper = indicator.transform.Find("UpperImage");
            if (upper != null) upper.gameObject.SetActive(false);
        }
    }

    private void CheckRounds()
    {
        if (playRounds == 3) maxWins = 2;
        else if (playRounds == 5) maxWins = 3;

    }

    private void BuildCharacterLibrary()
    {
        characterLibrary.Clear();
        foreach (CharacterData data in characterList)
        {
            if (!characterLibrary.ContainsKey(data.characterName))
            {
                characterLibrary.Add(data.characterName, data);
            }
        }
    }
    public CharacterData GetCharacter(string name)
    {
        if (characterLibrary.TryGetValue(name, out var data))
            return data;
        return null;
    }


    private void SpawnCharacters(string playerName, string opponentName)
    {
        characterLibrary.TryGetValue(playerName, out var playerData);
        characterLibrary.TryGetValue(opponentName, out var opponentData);

        List<int> indices = new(spawnPoints.Length);
        for (int i = 0; i < spawnPoints.Length; i++) indices.Add(i);

        int oppIndex = Random.Range(0, indices.Count);
        Transform oppSpawn = spawnPoints[indices[oppIndex]];
        indices.RemoveAt(oppIndex);

        int playerIndex = indices[Random.Range(0, indices.Count)];
        Transform playerSpawn = spawnPoints[playerIndex];

        GameObject oppGO = Instantiate(opponentData.prefab, oppSpawn.position, oppSpawn.rotation);
        GameObject playerGO = Instantiate(playerData.prefab, playerSpawn.position, playerSpawn.rotation);

        FaceEachOther(playerGO.transform, oppGO.transform);

        // Call AI setup methods
        CallSearchMethods(playerGO, oppGO);
    }

    private void FaceEachOther(Transform a, Transform b)
    {
        Vector3 dirToB = (b.position - a.position).normalized;
        Vector3 dirToA = (a.position - b.position).normalized;

        dirToB.y = 0f;
        dirToA.y = 0f;

        if (dirToB != Vector3.zero)
            a.rotation = Quaternion.LookRotation(dirToB);

        if (dirToA != Vector3.zero)
            b.rotation = Quaternion.LookRotation(dirToA);
    }
    private void CallSearchMethods(GameObject player, GameObject opponent)
    {
        player.GetComponent<FightingController>()?.SearchOpponent();
        opponent.GetComponent<OpponentAi>()?.SearchPlayer();
    }


}





[System.Serializable]
public class CharacterData
{
    public string characterName;
    public GameObject prefab;
    public Sprite icon;
    public Sprite image;
}