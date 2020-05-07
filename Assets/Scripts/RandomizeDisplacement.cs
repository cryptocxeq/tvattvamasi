using UnityEngine;

public class RandomizeDisplacement : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    public Vector2 xDirRange = new Vector2(-1.5f, 1.5f);
    public Vector2 yDirRange = new Vector2(-1.5f, 1.5f);

    int xd = Shader.PropertyToID("_XDir");
    int yd = Shader.PropertyToID("_YDir");

    private void Start()
    {
        float xdir = Random.Range(xDirRange.x, xDirRange.y);
        float ydir = Random.Range(yDirRange.x, yDirRange.y);
        meshRenderer.material.SetFloat(xd, xdir);
        meshRenderer.material.SetFloat(yd, ydir);      
    }

    //private void Update()
    //{
    //    if (randomiseDisplacement == true)
    //    {
    //        bool r1 = Random.Range(1, 100) > 95;
    //        bool r2 = Random.Range(1, 100) > 95;

    //        if (r1 == true)
    //        {
    //            float r4 = Random.Range(-0.01f, 0.01f);
    //            float xdf = Mathf.Clamp(meshRenderer.material.GetFloat(xd) + r4, -1.5f, 1.5f);
    //            meshRenderer.material.SetFloat(xd, xdf);
    //        }

    //        if (r2 == true)
    //        {
    //            float r4 = Random.Range(-0.01f, 0.01f);
    //            float ydf = Mathf.Clamp(meshRenderer.material.GetFloat(yd) + r4, -1.5f, 1.5f);
    //            meshRenderer.material.SetFloat(yd, ydf);
    //        }
    //    }
    //}
}
