using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	public void Reload()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
