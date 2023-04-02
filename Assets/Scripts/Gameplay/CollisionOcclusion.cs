using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CollisionOcclusion : MonoBehaviour
{
    public List<Tilemap> tilemaps;
    public List<CompositeCollider2D> compositeColliders;

    void Start()
    {
        Camera collisionCamera = new GameObject("Collision Camera").AddComponent<Camera>();
        collisionCamera.orthographic = true;
        collisionCamera.clearFlags = CameraClearFlags.SolidColor;
        collisionCamera.backgroundColor = new Color(0, 0, 0, 0);
        collisionCamera.cullingMask = 1 << LayerMask.NameToLayer("CollisionOcclusion");
        collisionCamera.transform.position = Camera.main.transform.position;
        collisionCamera.orthographicSize = Camera.main.orthographicSize;
        collisionCamera.depthTextureMode = DepthTextureMode.Depth;

        Material tilemapMaterial = new Material(Shader.Find("Unlit/Transparent"));
        tilemapMaterial.SetTexture("_DepthTexture", collisionCamera.targetTexture);

        for (int i = 0; i < tilemaps.Count; i++)
        {
            tilemaps[i].gameObject.AddComponent<CompositeCollider2D>();
            compositeColliders[i] = tilemaps[i].gameObject.GetComponent<CompositeCollider2D>();
            compositeColliders[i].usedByComposite = true;
            tilemaps[i].gameObject.layer = LayerMask.NameToLayer("CollisionOcclusion");
            tilemaps[i].GetComponent<TilemapRenderer>().material = tilemapMaterial;
        }

        GameObject collisionOcclusion = new GameObject("Collision Occlusion");
        collisionOcclusion.transform.position = transform.position;
        collisionOcclusion.transform.parent = transform;
    }
}