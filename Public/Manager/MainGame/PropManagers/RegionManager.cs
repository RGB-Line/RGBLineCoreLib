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
            public RegionTransitionEffector m_regionTransitionEffector;
        }


        [SerializeField] private RegionSpawnAssets m_regionSpawnAssets;

        private Guid m_prevRegionID = Guid.Empty;


        public void Awake()
        {
            SetInstance(this);
        }
        //public void FixedUpdate()
        //{
        //    if(!BIsInstanceNull)
        //    {
        //        Guid curRegionID = GetRegionIDFromYPos(Camera.main.transform.position.y);
        //        if (m_prevRegionID != curRegionID)
        //        {
        //            m_regionSpawnAssets.m_regionTransitionEffector.StartTransition((int)StageDataInterface.RegionDataInterface.GetRegionData(curRegionID).CurColorType,
        //                                                                            new Vector2());
        //        }
        //    }
        //}

        public void SpawnRegionProps()
        {

        }
        public void DespawnRegionProps()
        {

        }

        protected override void Dispose(bool bisDisposing)
        {
            if(bisDisposing)
            {

            }
        }

        private Guid GetRegionIDFromYPos(in float targetYPos)
        {
            float curFrame = GridManager.Instance.GetFrameFromYPos(targetYPos);

            Guid result = Guid.Empty;
            Guid[] regionIDs = StageDataInterface.RegionDataInterface.GetRegionIDs();
            float unitFrameSize = GridManager.Instance.GetUnitFrameSize();
            foreach (Guid regionID in regionIDs)
            {
                StageData.RegionData regionData = StageDataInterface.RegionDataInterface.GetRegionData(regionID);
                if (regionData.StartOffsetFrame + regionData.MinorOffsetTime * unitFrameSize < curFrame)
                {
                    result = regionID;
                }
                else
                {
                    break;
                }
            }

            return result;
        }
    }
}