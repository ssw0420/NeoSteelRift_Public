using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSW.Monster
{
    /// <summary>
    /// This class is responsible for controlling the renderer of the monster.
    /// </summary>
    public class RendererController : MonoBehaviour
    {
        [SerializeField] private Renderer[] _bodyRenderers;
        [SerializeField] private Renderer[] _vfxRenderers;

        public void SetBodyRendererActive(bool isActive)
        {
            foreach (var renderer in _bodyRenderers)
            {
                renderer.enabled = isActive;
            }
        }

        public void SetVfxRendererActive(bool isActive)
        {
            foreach (var renderer in _vfxRenderers)
            {
                renderer.enabled = isActive;
            }
        }

        public void SetAllRenderersActive(bool isActive)
        {
            SetBodyRendererActive(isActive);
            SetVfxRendererActive(isActive);
        }
    }
}
