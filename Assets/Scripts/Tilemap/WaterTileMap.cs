using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTileMap : MonoBehaviour
{
    public List<string> targetTags = new List<string> { "Player" ,"GravityBlock"};
    public float buoyancyForce = 10f;

    private Dictionary<Rigidbody2D, float> originalGravity = new();

    private void OnTriggerStay2D(Collider2D other)
    {
        if (targetTags.Contains(other.tag))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                if (!originalGravity.ContainsKey(rb))
                {
                    originalGravity[rb] = rb.gravityScale;
                }

                // 현재 중력 방향 기준 반대 방향으로 부력 적용
                float direction = Mathf.Sign(rb.gravityScale); // +1이면 중력 아래, -1이면 중력 위
                Vector2 buoyancyDir = direction > 0 ? Vector2.up : Vector2.down;
                rb.AddForce(buoyancyDir * buoyancyForce, ForceMode2D.Force);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (targetTags.Contains(other.tag))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                if (originalGravity.TryGetValue(rb, out float original))
                {
                    rb.gravityScale = original;
                    originalGravity.Remove(rb);
                }
            }
        }
    }
}
