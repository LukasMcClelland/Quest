using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonManager : MonoBehaviour {

    public static int n;

    public void ScenarioOneGameButton(string newGameLevel)
    {
        SceneManager.LoadScene(newGameLevel);
    }
    public void ScenarioTwoGameButton(string newGameLevel)
    {
        SceneManager.LoadScene(newGameLevel);
    }
    public void ScenarioThreeGameButton(string newGameLevel)
    {
        n = 1;
        SceneManager.LoadScene(newGameLevel);
    }
    public void ScenarioFourGameButton(string newGameLevel)
    {
        n = 2;
        SceneManager.LoadScene(newGameLevel);
    }
    public void ScenarioFiveGameButton(string newGameLevel)
    {
        n = 3;
        SceneManager.LoadScene(newGameLevel);
    }
    public void ScenarioSixGameButton(string newGameLevel)
    {
        n = 4;
        SceneManager.LoadScene(newGameLevel);
    }

    public void QuitButton(){
		Application.Quit();
	}

	public void ReturnToMainMenu(){
		SceneManager.LoadScene ("MainMenu");
	}
}
