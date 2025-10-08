using UnityEngine;
using UnityEngine.SceneManagement;

namespace DigestQuest
{
    public class SceneController : MonoBehaviour
    {
        public void GoToTitleScreen()
        {
            SceneManager.LoadScene("TitleScreen");
        }

        public void StartGame()
        {
           Debug.Log("startgame function has been called");
            SceneManager.LoadScene("Mouth");
            Debug.Log("will now load scene MOUTH");
        }

        public void GoToStomach()
        {
            SceneManager.LoadScene("Stomach");
            Debug.Log("will now load scene STOMACH");
        }

        public void GoToIntestine()
        {
            SceneManager.LoadScene("Intestine");
            Debug.Log("will now load scene INTESTINE");
        }

        public void GoToEndScreen()
        {
            SceneManager.LoadScene("End");
            Debug.Log("will now load scene END");
        }
    }
}