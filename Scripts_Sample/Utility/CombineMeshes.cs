using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSW.Test
{
    [ RequireComponent (typeof( MeshFilter ))]
    [ RequireComponent (typeof( MeshRenderer ))]
    public class CombineMeshes : MonoBehaviour
    {
        void Start()
        {
            Combine();
        }

        public void Combine()
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            for (int i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
            }
            transform.GetComponent<MeshFilter>().mesh = new Mesh();
            transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            transform.gameObject.SetActive(true);
        }
    }
}