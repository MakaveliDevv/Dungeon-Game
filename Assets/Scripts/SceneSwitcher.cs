using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string SceneToSwitch;
    public void SwitchScene() 
    {
        SceneManager.LoadScene(SceneToSwitch);
    }
}
