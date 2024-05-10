using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deckdown
{
    public class OnTriggerMatChange : MonoBehaviour
    {
        [SerializeField] MeshRenderer m_Renderer;

        [SerializeField] Material m_EnableMaterial;
        [SerializeField] Material m_DisableMaterial;

        private void OnTriggerEnter(Collider other)
        {
            m_Renderer.material = m_EnableMaterial;
        }

        private void OnTriggerExit(Collider other)
        {
            m_Renderer.material = m_DisableMaterial;
        }
    }
}
