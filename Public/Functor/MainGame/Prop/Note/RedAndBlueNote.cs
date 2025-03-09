using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib
{
    public class RedAndBlueNote : MonoBehaviour, IRedAndBlueNote
    {
        [SerializeField] private Sprite[] m_sprite_Notes;

        private INoteItem m_noteItem;

        private SpriteRenderer m_spriteRenderer = null;


        public void Awake()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public Transform Transform
        {
            get
            {
                return transform;
            }
        }

        public void Render(in INoteItem noteItem, in StageData.NoteData.NoteType noteType)
        {
            m_noteItem = noteItem;

            switch(noteType)
            {
                case StageData.NoteData.NoteType.Common:
                    m_spriteRenderer.sprite = m_sprite_Notes[0];
                    break;

                case StageData.NoteData.NoteType.Flip:
                    m_spriteRenderer.sprite = m_sprite_Notes[1];

                    switch (StageDataInterface.NoteDataInterface.GetNoteData(m_noteItem.NoteID).flipNoteDirection)
                    {
                        case StageData.NoteData.FlipNoteDirection.Left:
                            m_spriteRenderer.flipX = false;
                            break;

                        case StageData.NoteData.FlipNoteDirection.Right:
                            m_spriteRenderer.flipX = true;
                            break;
                    }
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
        }
    }
}