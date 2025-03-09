using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


namespace RGBLineCoreLib
{
    public sealed class GridManager : SingleTonForGameObject<GridManager>
    {
        public void Awake()
        {
            SetInstance(this);
        }

        public int GetTotalFrameCount()
        {
            StageData.StageConfigData stageConfigData = StageDataInterface.StageConfigDataInterface.GetStageMetadata();
            float BPS = (stageConfigData.BPM * stageConfigData.BitSubDivision) / 60f;

            return (int)(BPS * StageMetadataInterface.GetStageMetadata().MusicLength);
        }

        public float GetYPosFromFrame(in float targetFrame)
        {
            return targetFrame * GetUnitFrameSize();
        }
        public float GetUnitFrameSize()
        {
            StageData.StageConfigData stageConfigData = StageDataInterface.StageConfigDataInterface.GetStageMetadata();
            return stageConfigData.LengthPerBit / stageConfigData.BitSubDivision;
        }

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