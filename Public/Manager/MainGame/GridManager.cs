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
    /// 게임 내 Grid, 즉, Frame을 관리하는 Manager Class
    /// </summary>
    public sealed class GridManager : SingleTonForGameObject<GridManager>
    {
        public void Awake()
        {
            SetInstance(this);
        }

        /// <summary>
        /// 해당 Stage에 존재하는 총 Frame 수를 반환한다
        /// </summary>
        public int GetTotalFrameCount()
        {
            StageData.StageConfigData stageConfigData = StageDataInterface.StageConfigDataInterface.GetStageConfigData();
            float BPS = (stageConfigData.BPM * stageConfigData.BitSubDivision) / 60f;

            return (int)(BPS * StageMetadataInterface.GetStageMetadata().MusicLength);
        }

        /// <summary>
        /// 해당 Frame에 해당하는 Y Position을 반환한다
        /// </summary>
        public float GetYPosFromFrame(in float targetFrame)
        {
            return targetFrame * GetUnitFrameSize();
        }
        /// <summary>
        /// 단일 Frame의 크기를 반환한다(Y Position 기준)
        /// </summary>
        public float GetUnitFrameSize()
        {
            StageData.StageConfigData stageConfigData = StageDataInterface.StageConfigDataInterface.GetStageConfigData();
            return stageConfigData.LengthPerBit / stageConfigData.BitSubDivision;
        }

        /// <summary>
        /// Y Position에 해당하는 Frame을 반환한다
        /// </summary>
        public float GetFrameFromYPos(in float targetYPos)
        {
            return targetYPos / GetUnitFrameSize();
        }

        protected override void Dispose(bool bisDisposing)
        {
            if(bisDisposing)
            {

            }
        }
    }
}