using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Controllers/Application/Scene", fileName = "SceneController")]
public class SceneController : Controller
{
    [SerializeField]
    private CanvasGroup loadingPrefab;

    private CanvasGroup loadingInstance;

    private void Awake()
    {
        if (!Application.isPlaying)
            return;
        loadingInstance = Instantiate(loadingPrefab);
        loadingInstance.gameObject.SetActive(false);
        DontDestroyOnLoad(loadingInstance);
    }

    public override void Init()
    {
    }

    public override void DeInit()
    {
    }

    private async void LoadScene(string sceneName)
    {
        if (loadingInstance == null)
            Awake();
        loadingInstance.gameObject.SetActive(true);
        loadingInstance.alpha = 0f;
        loadingInstance.DOFade(1f, .2f);

        await Task.Delay(200);

        var asyncOp = SceneManager.LoadSceneAsync(sceneName);

        //asyncOp.allowSceneActivation = false;
        while (!asyncOp.isDone)
            await Task.Delay(10);

        loadingInstance.DOFade(0f, .2f);

        //asyncOp.allowSceneActivation = true;

        await Task.Delay(200);

        loadingInstance.gameObject.SetActive(false);
    }
}
