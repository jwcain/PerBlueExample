using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the animated tutorial marker, fakding out once it is interacted with
/// </summary>
public class MarkerTutorial : MonoBehaviour
{
    [SerializeField] float spinSpeed = 10f;
    [SerializeField] SpriteRenderer[] renderers;
    [SerializeField] float fadoutTime = 1f;
    [SerializeField] float scaleIncreaseOverTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("CustomLevelOverride") is var overrideLevel && overrideLevel != null)
            this.gameObject.SetActive(false);
        else
            PlayerController.onPlayerActionStart += CauseFade;
    }
    bool alreadyFaded = false;
    void CauseFade(Piece held)
    {
        if (alreadyFaded)
            return;
        if (this.gameObject.activeInHierarchy == false)
            return;
        alreadyFaded = true;
        PlayerController.onPlayerActionStart -= CauseFade;
        StartCoroutine(Fadeout(fadoutTime));
    }

    IEnumerator Fadeout(float time)
    {
        float[] startingAlphas = new float[renderers.Length];
        for (int i = 0; i < startingAlphas.Length; i++)
        {
            startingAlphas[i] = renderers[i].color.a;
        }
        void SetAlpha(SpriteRenderer fadoutImage, float alpha)
        {
            fadoutImage.color = new Color(fadoutImage.color.r, fadoutImage.color.g, fadoutImage.color.b, alpha);
        }
        float timer = time;

        while (timer >= 0.0f)
        {
            timer -= Time.deltaTime;
            for (int i = 0; i < startingAlphas.Length; i++)
                SetAlpha(renderers[i], Mathf.Lerp(0f, startingAlphas[i], timer / time));
            this.gameObject.transform.localScale += Vector3.one * (scaleIncreaseOverTime * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < startingAlphas.Length; i++)
        {
            SetAlpha(renderers[i], 0f);
        }
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localRotation = Quaternion.Euler(0f, 0f, this.transform.localRotation.eulerAngles.z + (spinSpeed * Time.deltaTime));
    }
}
