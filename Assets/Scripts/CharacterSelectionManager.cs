using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    public void SelectMage()
    {
        PlayerPrefs.SetInt("SelectedCharacter", 0);
        SceneManager.LoadScene(2);
    }

    public void SelectKnight()
    {
        PlayerPrefs.SetInt("SelectedCharacter", 1);
        SceneManager.LoadScene(2);
    }
}
