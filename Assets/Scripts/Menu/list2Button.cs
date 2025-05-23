using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class list2Button : MonoBehaviour
{

    [SerializeField] private GameObject[] characters;
    [SerializeField] private Button button;
    [SerializeField] private Transform playerButtonParent;

    [SerializeField] private Transform opponentButtonParent;

    [SerializeField] private GameObject characterSelectGameObject;


    void Start()
    {
        if (!GameManager.isOnline)
        {
            GenerateOpponentCharacterButtons();
        }
        else opponentButtonParent.gameObject.SetActive(false);


        GeneratePlayerCharacterButtons();
    }

    #region Player
    public void GeneratePlayerCharacterButtons()
    {
        List<CharacterData> characters = GameManager.Instance.CharacterList;



        foreach (var character in characters)
        {
            if (character.characterName.EndsWith("(OP)"))
            {
                continue;
            }

            Button newButton = Instantiate(button, playerButtonParent);
            newButton.name = character.characterName;

            Image image = newButton.GetComponent<Image>();
            image.sprite = character.icon;

            CharacterData captured = character;
            newButton.onClick.AddListener(() => OnPlayerSelected(captured));
        }
    }
    private void OnPlayerSelected(CharacterData character)
    {
        GameManager.selectedCharacter = character.characterName;
        characterSelectGameObject.GetComponent<CharacterSelect>().UpdateCharacterDisplay();

    }
    #endregion

    #region Opponent
    public void GenerateOpponentCharacterButtons()
    {
        List<CharacterData> characters = GameManager.Instance.CharacterList;



        foreach (var character in characters)
        {
            if (!character.characterName.EndsWith("(OP)"))
            {
                continue;
            }

            Button newButton = Instantiate(button, opponentButtonParent);
            newButton.name = character.characterName;

            Image image = newButton.GetComponent<Image>();
            image.sprite = character.icon;

            CharacterData captured = character;
            newButton.onClick.AddListener(() => OnOpponentSelected(captured));
        }
    }

    private void OnOpponentSelected(CharacterData character)
    {
        GameManager.opponentCharacter = character.characterName;
        characterSelectGameObject.GetComponent<CharacterSelect>().UpdateCharacterDisplay();

    }   
    #endregion
}

