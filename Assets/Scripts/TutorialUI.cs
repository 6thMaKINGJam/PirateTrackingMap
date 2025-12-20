using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialUI : MonoBehaviour
{
    public void OnClickStart()
    {
        SceneManager.LoadScene("StoryEndScene");
    }
}
