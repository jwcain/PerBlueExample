using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles displaying the 'victory' screen to the player
/// </summary>
public class EndScreenController : Bestagon.Behaviours.ProtectedSceneSingleton<EndScreenController>
{
    public float delayStartTime = 3f;
    public float animTime = 3f;
    public TMPro.TMP_Text thanks;
    public UnityEngine.UI.Image[] imagesToFade;
    public TMPro.TMP_Text[] textToFade;

    public IEnumerator FadeInAnim()
    {
        MusicHandler.EndScreenOverride();
        yield return new WaitForSeconds(delayStartTime);
        yield return AnimUtility.LerpOverTime(0f, 1f, animTime, (float val) => { thanks.color = new Color(thanks.color.r, thanks.color.g, thanks.color.b, val); });
        yield return new WaitForSeconds(3f);
        yield return AnimUtility.LerpOverTime(0f, 1f, animTime, SetAlphas);
    }

    private void OnEnable()
    {
        thanks.color = new Color(thanks.color.r, thanks.color.g, thanks.color.b, 0f);
        SetAlphas(0f);
        StartCoroutine(FadeInAnim());
    }

    void SetAlphas(float alpha)
    {
        foreach (var image in imagesToFade)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
        foreach (var textobj in textToFade)
        {
            textobj.color = new Color(textobj.color.r, textobj.color.g, textobj.color.b, alpha);
        }
    }

    protected override void Destroy()
    {
        //throw new System.NotImplementedException();
    }
}
