using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Parallax : MonoBehaviour
{
    [Tooltip("Camera to read movement from. If empty, uses Camera.main")]
    public Camera cam;

    [Tooltip("Parallax amount (0 = static, 1 = same speed as camera)")]
    [Range(0f, 2f)]
    public float amount = 0.5f;

    [Tooltip("Per-axis multiplier applied in addition to `amount`")]
    public Vector2 axisMultiplier = new Vector2(1f, 1f);

    [Tooltip("Enable horizontal tiling. Requires multiple child sprite copies arranged horizontally")]
    public bool tileX = true;

    [Tooltip("Enable vertical tiling. Requires multiple child sprite copies arranged vertically")]
    public bool tileY = false;

    [Tooltip("If true, keeps the layer's Y position fixed (useful for mountains that shouldn't move when player jumps)")]
    public bool lockY = false;

    List<Transform> tiles = new List<Transform>();
    float spriteWidth = 0f;
    float spriteHeight = 0f;
    Vector3 startPos;
    float fixedZ;

    void Start()
    {
        if (cam == null) cam = Camera.main;

        startPos = transform.position;
        fixedZ = transform.position.z;

        // collect child tiles; if none, treat this GameObject as single tile
        if (transform.childCount > 0)
        {
            foreach (Transform t in transform)
            {
                if (t.GetComponent<SpriteRenderer>() != null)
                    tiles.Add(t);
            }
        }

        if (tiles.Count == 0)
        {
            // use self as single tile (no tiling behavior)
            tiles.Add(transform);
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                spriteWidth = sr.bounds.size.x;
                spriteHeight = sr.bounds.size.y;
            }
            else
            {
                spriteWidth = 0f;
                spriteHeight = 0f;
            }
        }
        else
        {
            // measure from first tile
            var sr = tiles[0].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                spriteWidth = sr.bounds.size.x;
                spriteHeight = sr.bounds.size.y;
            }
        }
    }

    void LateUpdate()
    {
        if (cam == null)
        {
            if (Camera.main == null) return;
            cam = Camera.main;
        }

        Vector3 camPos = cam.transform.position;

        float distX = camPos.x * amount * axisMultiplier.x;
        float distY = camPos.y * amount * axisMultiplier.y;

        Vector3 targetPos = new Vector3(startPos.x + distX, startPos.y + distY, fixedZ);
        if (lockY) targetPos.y = startPos.y;
        transform.position = targetPos;

        // handle simple wrapping for existing child tiles to avoid gaps
        if (tiles.Count > 1)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                Transform t = tiles[i];

                if (tileX && spriteWidth > 0f)
                {
                    float diff = camPos.x - t.position.x;
                    if (diff > spriteWidth)
                    {
                        t.position += Vector3.right * spriteWidth * tiles.Count;
                    }
                    else if (diff < -spriteWidth)
                    {
                        t.position -= Vector3.right * spriteWidth * tiles.Count;
                    }
                }

                if (tileY && spriteHeight > 0f)
                {
                    float diffY = camPos.y - t.position.y;
                    if (diffY > spriteHeight)
                    {
                        t.position += Vector3.up * spriteHeight * tiles.Count;
                    }
                    else if (diffY < -spriteHeight)
                    {
                        t.position -= Vector3.up * spriteHeight * tiles.Count;
                    }
                }
            }
        }
    }
}
