using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void Exit()
    {
        PlayerPrefs.DeleteKey("EntryLogged");
        Application.Quit();
    }
}
