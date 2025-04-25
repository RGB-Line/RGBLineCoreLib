using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CommonUtilLib.ThreadSafe;

using RGBLineCoreLib.Data;
using RGBLineCoreLib.Functor;


namespace RGBLineCoreLib.Manager
{
    /// <summary>
    /// 게임 내 Region을 관리하는 Manager Class
    /// </summary>
    public sealed class RegionManager : SingleTonForGameObject<RegionManager>
    {
        [Serializable] public struct RegionSpawnAssets
        {
            public RegionTransitionEffector m_regionTransitionEffector;
        }


        [SerializeField] private RegionSpawnAssets m_regionSpawnAssets = default;

        private Guid m_prevRegionID = Guid.Empty;


        public void Awake()
        {
            SetInstance(this);
        }

        /// <summary>
        /// 게임 내에 Region Item들을 생성할 때 호출하면 된다
        /// </summary>
        /// <remarks>
        /// Editor에서 Region Item들을 다시 그릴 경우에는 필히 먼저 DespawnRegionProps()을 호출하고, 그 다음에 SpawnRegionProps()을 호출해야 한다
        /// </remarks>
        public void SpawnRegionProps()
        {

        }
        /// <summary>
        /// 게임 내에 Region Item들을 제거할 때 호출하면 된다
        /// </summary>
        /// <remarks>
        /// Scene Unload 전에 필히 DespawnRegionProps() + GC.Collect()를 호출해 Memory 관리에 유의한다
        /// </remarks>
        public void DespawnRegionProps()
        {

        }

        protected override void Dispose(bool bisDisposing)
        {
            if(bisDisposing)
            {

            }
        }

        internal Guid GetRegionIDFromYPos(in float targetYPos)
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