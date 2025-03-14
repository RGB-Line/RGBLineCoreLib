using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using RGBLineCoreLib.Manager;
using RGBLineCoreLib.Data;


namespace RGBLineCoreLib.Functor
{
    public class RedLineCornerNote : MonoBehaviour, IRedLineCornerNote
    {
        private Guid m_noteID = Guid.Empty;

        private BoxCollider2D m_judgeBox = null;


        public void Awake()
        {
            m_judgeBox = GetComponent<BoxCollider2D>();
        }

        public Guid NoteID
        {
            get
            {
                return m_noteID;
            }
        }
        public Transform Transform
        {
            get
            {
                return transform;
            }
        }

        public void Render()
        {
            m_noteID = Guid.NewGuid();

            NoteManager.Instance.AddRedLineCornerNoteItem(m_noteID, this);

            StageData.StageConfigData curStageConfigData = StageDataInterface.StageConfigDataInterface.GetStageConfigData();
            StageMetadata stageMetadata = StageMetadataInterface.GetStageMetadata();
            float velocity = (GridManager.Instance.GetTotalFrameCount() * (curStageConfigData.LengthPerBit / curStageConfigData.BitSubDivision)) / stageMetadata.MusicLength;

            GameConfigData gameConfigData = GameConfigDataBuffer.Instance.ConfigData;
            m_judgeBox.size = new Vector2()
            {
                x = 1.0f,
                y = velocity * (gameConfigData.NoteHitJudgeStrandard.HitJudgingRanges[2] / 1000.0f) * 4.0f
            };
        }

        public void Dispose()
        {
            m_noteID = Guid.Empty;
            m_judgeBox.size = Vector2.one;
        }
    }
}