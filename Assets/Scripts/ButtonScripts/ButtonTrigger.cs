using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    [Header("探索するTAG")]
    public List<string> triggerTags = new List<string> { "Player", "GravityBlock" };

    [Header("スプライト")]
    public Sprite normalSprite;
    public Sprite pressedSprite;

    [Header("対象タイルマップ")]
    public GameObject targetTilemap;

    [Header("パーティクル設定")]
    public ParticleSystem particlePrefab;
    public List<Transform> particleSpawnPoints;

    private SpriteRenderer spriteRenderer;
    private int triggerCount = 0;

    private List<ParticleSystem> activeParticles = new List<ParticleSystem>();

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = normalSprite;

        if (targetTilemap != null)
            targetTilemap.SetActive(false);

        //  パーティクルスポンジ位置隠し
        foreach (Transform point in particleSpawnPoints)
        {
            if (point != null)
                point.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerTags.Contains(other.tag))
        {
            triggerCount++;
            spriteRenderer.sprite = pressedSprite;

            if (targetTilemap != null)
                targetTilemap.SetActive(true);

            // パーティクルスポンジ位置表す
            foreach (Transform point in particleSpawnPoints)
            {
                if (point != null)
                    point.gameObject.SetActive(true);
            }

            PlayParticles();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (triggerTags.Contains(other.tag))
        {
            triggerCount--;

            if (triggerCount <= 0)
            {
                spriteRenderer.sprite = normalSprite;

                if (targetTilemap != null)
                    targetTilemap.SetActive(false);

                // パーティクルスポンジ位置隠し
                foreach (Transform point in particleSpawnPoints)
                {
                    if (point != null)
                        point.gameObject.SetActive(false);
                }

                StopParticles();
            }
        }
    }

    void PlayParticles()
    {
        StopParticles();

        if (particlePrefab != null && particleSpawnPoints.Count > 0)
        {
            foreach (Transform point in particleSpawnPoints)
            {
                ParticleSystem ps = Instantiate(particlePrefab, point.position, Quaternion.identity);
                ps.Play();
                activeParticles.Add(ps);
            }
        }
    }

    void StopParticles()
    {
        foreach (ParticleSystem ps in activeParticles)
        {
            if (ps != null)
            {
                ps.Stop();
                Destroy(ps.gameObject);
            }
        }
        activeParticles.Clear();
    }
}
