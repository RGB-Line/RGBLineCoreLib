using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using RGBLineCoreLib.Data;
using RGBLineCoreLib.Manager;


namespace RGBLineCoreLib.Functor
{
    /// <summary>
    /// 기본적인 Red and Blue Note를 그리기 위한 Class
    /// </summary>
    /// <remarks>
    /// Editor에서는 해당 Class를 상속받아 사용하지만, Runtime에서는 해당 Class를 직접 사용함
    /// </remarks>
    public class RedAndBlueNote : MonoBehaviour, IRedAndBlueNote
    {
        [SerializeField] private Sprite[] m_sprite_Notes = null;

        private INoteItem m_noteItem;

        [SerializeField] private SpriteRenderer m_spriteRenderer = null;
        [SerializeField] private BoxCollider2D m_judgeBox = null;


        public void Awake()
        {
            //m_spriteRenderer = GetComponent<SpriteRenderer>();
            //m_judgeBox = GetComponent<BoxCollider2D>();
        }

        public Guid AttachedNoteID
        {
            get
            {
                return m_noteItem.NoteID;
            }
        }
        public Transform Transform
        {
            get
            {
                return transform;
            }
        }

        public virtual void Render(in INoteItem noteItem, in StageData.NoteData.NoteType noteType)
        {
            m_noteItem = noteItem;

            switch(noteType)
            {
                case StageData.NoteData.NoteType.Common:
                    m_spriteRenderer.sprite = m_sprite_Notes[0];
                    break;

                case StageData.NoteData.NoteType.Double:
                    m_spriteRenderer.sprite = m_sprite_Notes[1];

                    //switch (StageDataInterface.NoteDataInterface.GetNoteData(m_noteItem.NoteID).flipNoteDirection)
                    //{
                    //    case StageData.NoteData.FlipNoteDirection.Left:
                    //        m_spriteRenderer.flipX = false;
                    //        break;

                    //    case StageData.NoteData.FlipNoteDirection.Right:
                    //        m_spriteRenderer.flipX = true;
                    //        break;
                    //}
                    break;
            }

            StageData.NoteData curNoteData = StageDataInterface.NoteDataInterface.GetNoteData(m_noteItem.NoteID);
            transform.position = new Vector3()
            {
                x = m_noteItem.GetNoteXPos(curNoteData.StartOffsetFrame + curNoteData.MinorOffsetTime, curNoteData.AttachedLineID),
                y = GridManager.Instance.GetYPosFromFrame(StageDataInterface.NoteDataInterface.GetAttachedRegionData(m_noteItem.NoteID).StartOffsetFrame) +
                    GridManager.Instance.GetYPosFromFrame(StageDataInterface.NoteDataInterface.GetAttachedLineData(m_noteItem.NoteID).CurvedLinePoints[0].Y) +
                    GridManager.Instance.GetYPosFromFrame(curNoteData.StartOffsetFrame) +
                    GridManager.Instance.GetUnitFrameSize() * curNoteData.MinorOffsetTime,
                z = -7.5f
            };
            if(noteType == StageData.NoteData.NoteType.Double)
            {
                m_spriteRenderer.transform.rotation = Quaternion.Euler(0.0f, 0.0f, m_noteItem.GetNoteRotation(curNoteData.StartOffsetFrame + curNoteData.MinorOffsetTime, curNoteData.AttachedLineID));
            }

            StageData.StageConfigData curStageConfigData = StageDataInterface.StageConfigDataInterface.GetStageConfigData();
            StageMetadata stageMetadata = StageMetadataInterface.GetStageMetadata();
            float velocity = (GridManager.Instance.GetTotalFrameCount() * (curStageConfigData.LengthPerBit / curStageConfigData.BitSubDivision)) / stageMetadata.MusicLength;
            //float velocity = GridManager.Instance.GetYPosFromFrame(GridManager.Instance.GetTotalFrameCount()) / GridManager.Instance.GetTotalFrameCount();

            GameConfigData gameConfigData = GameConfigDataBuffer.Instance.ConfigData;
            m_judgeBox.size = new Vector2()
            {
                x = 1.0f,
                y = velocity * (gameConfigData.NoteHitJudgeStrandard.HitJudgingRanges[2] / 1000.0f) * 4.0f
            };
        }

        public virtual void Dispose()
        {
            m_noteItem = null;
            m_spriteRenderer.sprite = null;
            m_judgeBox.size = Vector2.one;
        }
    }
}