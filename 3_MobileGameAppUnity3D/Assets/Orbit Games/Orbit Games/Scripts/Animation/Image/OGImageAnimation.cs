using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OGImageAnimation : MonoBehaviour
{
    public enum StartBehaviour {
        Restart, Random, Continue
    }

    [Header("References")]
    public Image image;
    public List<Sprite> sprites = new List<Sprite>();
    
    [Header("Settings")]
    public float secondsPerSprite = 1 / 15f;
    public float mutateSpeed = 0f;
    public StartBehaviour startBehaviour;
    public bool repeat = true;

    private float currentTime = 0f;
    private int currentSprite = 0;
    private float currentSecondsPerSprite;

    private void OnEnable()
    {
        switch (startBehaviour)
        {
            case StartBehaviour.Restart:
                currentTime = 0f;
                currentSprite = 0;
                break;
            case StartBehaviour.Random:
                currentTime = 0f;
                currentSprite = Random.Range(0, sprites.Count - 1);
                break;
            case StartBehaviour.Continue:
                break;
            default:
                break;
        }
        currentSecondsPerSprite = secondsPerSprite + (Random.value - 0.5f) * 2f * mutateSpeed;
        image.sprite = sprites[currentSprite];
    }

    private void Update()
    {
        if (currentSecondsPerSprite == 0f) return;
        
        currentTime += Time.deltaTime;
        while (currentTime > currentSecondsPerSprite)
        {
            currentTime -= currentSecondsPerSprite;
            if (repeat)
            {
                currentSprite = (currentSprite + 1) % sprites.Count;
            }
            else
            {
                currentSprite = Mathf.Min(sprites.Count - 1, currentSprite + 1);
            }
        }
        image.sprite = sprites[currentSprite];
    }


}
