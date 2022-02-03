using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneTest : MonoBehaviour {

    public void TestLoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
        WorldGenerationMaster.Instance.SetActive(sceneName == "MapScene");
    }

}
