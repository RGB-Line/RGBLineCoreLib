using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


namespace RGBLineCoreLib
{
    public sealed class RegionManager : SingleTonForGameObject<RegionManager>
    {
        [Serializable] public struct RegionSpawnAssets
        {

        }


        [SerializeField] private RegionSpawnAssets m_regionSpawnAssets;


        public void Awake()
        {
            SetInstance(this);
        }

        internal void SpawnRegionProps()
        {

        }
        internal void DespawnRegionProps()
        {

        }

        protected override void Dispose(bool bisDisposing)
        {
            if(bisDisposing)
            {

            }
        }
    }
}