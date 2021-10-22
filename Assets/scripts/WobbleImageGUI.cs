using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WobbleImageGUI : MonoBehaviour
{
    private float spf = 1f/4f;
    private float timer = 0;
    public Image image;

    public Sprite[] sprites;

    private int spriteIndex = 0;

	private void Start()
	{
        image = GetComponent<Image>();
        image.sprite = sprites[spriteIndex];
	}

	// Update is called once per frame
	void Update()
    {
        if (timer > spf)
        {
            timer -= spf;
            spriteIndex = Tools.GetRandomIntExcluding(0, sprites.Length-1, spriteIndex);
            image.sprite = sprites[spriteIndex];
        }

        timer += Time.deltaTime;
    }
}
