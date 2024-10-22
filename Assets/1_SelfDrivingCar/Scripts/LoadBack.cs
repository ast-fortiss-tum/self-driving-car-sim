using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBack : MonoBehaviour
{

#if UNITY_EDITOR 
    private void Awake()
    {
        if (LoadingSceneIntegration.otherScene > 0)
        {
            // Debug.Log("Returning again to the scene: " + LoadingSceneIntegration.otherScene);
            SceneManager.LoadScene(LoadingSceneIntegration.otherScene);
        } else {
            SceneManager.LoadScene(1);
        }
    }
#endif

}
