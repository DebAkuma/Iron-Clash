using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [Header("Display Image")]
    [SerializeField] private Image showPlayerCharacter;
    [SerializeField] private Image showOpponentCharacter;

    [Header("Ready Icons")]
    [SerializeField] private Image readyPlayer;
    [SerializeField] private Image readyOpponent;

    [Header("Map UI")]
    [SerializeField] private Image mapImage;
    [SerializeField] private TextMeshProUGUI mapName;
    [SerializeField] private List<MapData> mapList = new List<MapData>();

    public int currentMapIndex;

    [Header("Character UI")]
    [SerializeField] private TextMeshProUGUI selectedPlayerName;
    [SerializeField] private TextMeshProUGUI selectedOpponentName;


    private void Start()
    {
        readyPlayer.gameObject.SetActive(false);
        currentMapIndex = 1;
        MapUiUpdate(currentMapIndex);

        if (!GameManager.isOnline)
        {
            readyOpponent.gameObject.SetActive(false);
        }

    }

    // Called when player clicks "Next Map"
    public void MapUp()
    {
        currentMapIndex++;
        if (currentMapIndex > mapList.Count) currentMapIndex = 1;
        MapUiUpdate(currentMapIndex);
    }

    // Called when player clicks "Previous Map"
    public void MapDown()
    {
        currentMapIndex--;
        if (currentMapIndex < 1) currentMapIndex = mapList.Count;
        MapUiUpdate(currentMapIndex);
    }

    private void MapUiUpdate(int index)
    {
        foreach (var map in mapList)
        {
            if (index == map.mapNumber)
            {
                mapName.text = map.mapName;
                mapImage.sprite = map.mapImage;
                GameManager.mapName = map.mapName;
                return;
            }
        }
    }


    public void UpdateCharacterDisplay()
    {
        List<CharacterData> characters = GameManager.Instance.CharacterList;
        foreach (var character in characters)
        {
            if (GameManager.opponentCharacter == character.characterName)
            {
                var tempText = character.characterName.Replace("(OP)", "");
                showOpponentCharacter.sprite = character.image;
                selectedOpponentName.text = tempText;
            }

            else if (GameManager.selectedCharacter == character.characterName)
            {
                showPlayerCharacter.sprite = character.image;
                selectedPlayerName.text = character.characterName;
            }
        }
    }

    public void ReadyButton()
    {
        readyPlayer.gameObject.SetActive(true);
        SceneManager.LoadScene("Map"+currentMapIndex);
    }


}


// PUBLIC GAME Map CLASS 
[System.Serializable]
public class MapData
{
    public string mapName;
    public Sprite mapImage;
    public int mapNumber;
}
