using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public Image img;
    public AnimationCurve curve;
    public float fadeTimer = 1f;
    /*private void Awake()
    {
        
    }*/
    void Start()
    {
        StartCoroutine(FadeIn());    
    }

    public void FadeTo(string e)
    {
        StartCoroutine(FadeOut(e));
    }
    IEnumerator FadeIn()
    {
        float t = fadeTimer;
        
        while (t > 0f)
        {
            t -= Time.deltaTime;
            float a=curve.Evaluate(t);
            img.color = new Color(0f,0f,0f,a);
            yield return 0;
        }
    }
    IEnumerator FadeOut(string scene)
    {
        float t = 0f;

        while (t < fadeTimer)
        {
            t += Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }
        SceneManager.LoadScene(scene);
    }
}
