using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <copyright file="LoadingscreenManager.cs">
/// Copyright (c) 2019 All Rights Reserved
/// </copyright>
/// <author>Kevin Hummel</author>
/// <date>18/03/2019 21:41 PM </date>
/// <summary>
/// This class is needed if data is too heavy too load and a loadingscreen is needed.
/// </summary>
public class LoadingscreenManager : MonoBehaviour
{
    public static LoadingscreenManager Instance { get; set; }
    public bool IsLoading { get; set; }
    public bool IsSetupScene
    {
        get
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                return true;
            }
            else return false;

        }
    }

    [SerializeField] private bool _loadOnStart = false;
    [SerializeField] private string _levelToLoad = null;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (_loadOnStart) LoadScene(_levelToLoad);
    }

    private void Update()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public void LoadScene(string p_input)
    {
        SceneManager.LoadScene(p_input);
    }

    public void OpenLoadingscreen(float p_current, float p_max, string p_text)
    {
        UIManager t_uiManager = UIManager.Instance;

        t_uiManager.OpenLoading(p_text);
        IsLoading = true;

        t_uiManager.LoadingBar.maxValue = p_max;
        t_uiManager.LoadingBar.value = p_current;

        if (t_uiManager.LoadingBar.value == p_max)
            StartCoroutine(CloseLoadingscreen());
    }

    public IEnumerator CloseLoadingscreen()
    {
        UIManager t_uiManager = UIManager.Instance;

        IsLoading = false;

        yield return new WaitForSeconds(1f);

        if (IsLoading == false) t_uiManager.CloseLoading();
    }
}

