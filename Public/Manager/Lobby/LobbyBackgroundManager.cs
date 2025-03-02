using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


namespace RGBLineCoreLib
{
    public class LobbyBackgroundManager : SingleTonForGameObject<LobbyBackgroundManager>
    {
        [SerializeField] private List<Material> m_mats;
        private int m_randomMatSelectIndex;

        private MeshRenderer m_meshRenderer;


        public void Awake()
        {
            SetInstance(this);

            m_meshRenderer = GetComponent<MeshRenderer>();

            m_randomMatSelectIndex = UnityEngine.Random.Range(0, m_mats.Count);
        }
        public void FixedUpdate()
        {
            m_meshRenderer.material = m_mats[m_randomMatSelectIndex];
        }

        public int RandomMatSelectIndex
        {
            get
            {
                return m_randomMatSelectIndex;
            }
            set
            {
                m_randomMatSelectIndex = value;
            }
        }

        protected override void Dispose(bool bisDisposing)
        {
            if(bisDisposing)
            {

            }
        }
    }
}