using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobbleSprite: MonoBehaviour
{
    public MeshRenderer[] sprites;

    private int activeIndex;

    void Start()
    {
        activeIndex = 0;
        sprites[0].enabled = true;

        for (int i = 1; i < sprites.Length; ++i)
        {
            sprites[i].enabled = false;
        }

        GlobalObjects.wobbleSpriteTimerStatic.wobbleSpritesToUpdate.Add(this);
    }

    public void changeSprite()
    {
        if(_visible && sprites.Length > 1)
        {
            int nextActiveIndex = Tools.GetRandomIntExcluding(0, sprites.Length - 1, activeIndex);
            sprites[activeIndex].enabled = false;
            sprites[nextActiveIndex].enabled = true;
            activeIndex = nextActiveIndex;
        }
    }

    private bool _visible = true;
    public bool visible
    {
		set
        {
            if (value && !_visible)
            {
                sprites[activeIndex].enabled = true;
                _visible = true;
            }
            else if (!value && _visible)
            {
                sprites[activeIndex].enabled = false;
                _visible = false;
            }
        }

		get { return _visible; }
    }

	private void OnEnable()
	{
        visible = true;
	}

	private void OnDisable()
	{
        visible = false;
	}

    public void forceHideAndDisable()
    {
		for (int i = 0; i < sprites.Length; ++i)
        {
            sprites[i].enabled = false;
        }
        enabled = false;
    }
}
