using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//responsible for loading up the game world after pressing Start Game
namespace GC
{
    public class TitleScreenManager : MonoBehaviour
    {
        public static TitleScreenManager instance;

        [SerializeField] int worldSceneIndex = 1;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator LoadNewGame()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);
            yield return null;
        }

        public void StartNewGame()
        {
            StartCoroutine(TitleScreenManager.instance.LoadNewGame());
        }

    }
}


