
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FadeInTextOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text toFade;

    private const float fadeSpeed = 4f;
    private bool hovering = false;

    void Start()
    {
        toFade.color = GetColorWithAlpha(0);
    }

    void Update()
    {
        float newAlpha = toFade.color.a + ((hovering ? fadeSpeed : -fadeSpeed) * Time.deltaTime);
        toFade.color = GetColorWithAlpha( Mathf.Clamp01(newAlpha) );
    }

    private Color GetColorWithAlpha(float alpha)
    {
        return new Color(toFade.color.r, toFade.color.g, toFade.color.b, alpha);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}
