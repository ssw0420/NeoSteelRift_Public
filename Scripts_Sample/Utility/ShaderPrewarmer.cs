using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderPrewarmer : MonoBehaviour
{
    [SerializeField]
    private ShaderVariantCollection shaderVariantCollection;

    void Awake()
    {
        if (shaderVariantCollection != null)
        {
            shaderVariantCollection.WarmUp();
        }
    }
}
