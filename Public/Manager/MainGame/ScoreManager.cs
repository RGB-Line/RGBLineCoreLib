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
    /// 게임 내 점수를 관리하는 Manager Class
    /// </summary>
    /// <remarks>
    /// Scene Load 시 ScoreManager GameObject의 경우 필히 StartScoring()를 호출해야 한다
    /// </remarks>
    public sealed class ScoreManager : SingleTonForGameObject<ScoreManager>
    {
        [Flags] private enum PressedKeyState : byte
        {
            None                = 0b0000_0000,
            SingleClicked       = 0b0000_0001,
            DoubleClicked       = 0b0000_0010,
            LongClicked         = 0b0000_0100,
            LeftArrowClicked    = 0b0000_1000,
            RightArrowClicked   = 0b0001_0000,
        }


        [SerializeField] private LineTracker m_lineTracker = null;

        private bool m_bisStartScoring = false;

        private Stack<Guid> m_noteCandidates = new Stack<Guid>();

        private readonly Dictionary<KeyCode, float> m_pressedKeyTable = new Dictionary<KeyCode, float>();

        private readonly List<KeyCode> m_keyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToList();
        private readonly Dictionary<Guid, Tuple<float, float>> m_longNoteTable = new Dictionary<Guid, Tuple<float, float>>();

        [SerializeField] private float m_doubleClickTerm = 0.5f;

        [SerializeField] private List<KeyCode> m_bannedNoteKeyCode = null;

        private float m_curAudioSourceTime = 0.0f;

        private bool m_bisGreenRegion, m_bisOnGreenLine;
        private bool m_bisBlueRegion;

        private float m_curScore = 0.0f;
        private float m_HP = 1.0f;

        private int m_combo = 0;

        private bool m_bisSomethingTroubled = false;


        public void Awake()
        {
            SetInstance(this);

            foreach(KeyCode targetKeyCode in m_bannedNoteKeyCode)
            {
                m_keyCodes.Remove(targetKeyCode);
            }
        }
        public void Update()
        {
            if (!m_bisStartScoring)
            {
                return;
            }

            if (BIsGameOver)
            {
                return;
            }

            // Check and Remove Pressed Key from m_pressedKeyTable
            List<KeyCode> removeKeyList = new List<KeyCode>();
            foreach (KeyCode targetKeyCode in m_pressedKeyTable.Keys)
            {
                if (m_curAudioSourceTime - m_pressedKeyTable[targetKeyCode] >= m_doubleClickTerm)
                {
                    removeKeyList.Add(targetKeyCode);
                }
            }
            foreach (KeyCode targetKeyCode in removeKeyList)
            {
                m_pressedKeyTable.Remove(targetKeyCode);
            }

            // Get Input
            PressedKeyState curPressedKeyState = PressedKeyState.None;
            foreach (KeyCode targetKeyCode in m_keyCodes)
            {
                if (Input.GetKeyDown(targetKeyCode))
                {
                    // Single Click
                    if (!m_pressedKeyTable.ContainsKey(targetKeyCode)
                        && targetKeyCode != KeyCode.LeftArrow
                        && targetKeyCode != KeyCode.RightArrow)
                    {
                        m_pressedKeyTable.Add(targetKeyCode, m_curAudioSourceTime);
                        curPressedKeyState |= PressedKeyState.SingleClicked;
                    }
                    // Double Click
                    else if(m_pressedKeyTable.ContainsKey(targetKeyCode)
                            && targetKeyCode != KeyCode.LeftArrow
                            && targetKeyCode != KeyCode.RightArrow)
                    {
                        curPressedKeyState |= PressedKeyState.DoubleClicked;
                    }

                    // Arrow Click
                    if (targetKeyCode == KeyCode.LeftArrow)
                    {
                        curPressedKeyState |= PressedKeyState.LeftArrowClicked;
                    }
                    else if (targetKeyCode == KeyCode.RightArrow)
                    {
                        curPressedKeyState |= PressedKeyState.RightArrowClicked;
                    }
                }
                // Long Click
                else if (targetKeyCode != KeyCode.LeftArrow && targetKeyCode != KeyCode.RightArrow && Input.GetKey(targetKeyCode))
                {
                    m_pressedKeyTable[targetKeyCode] = m_curAudioSourceTime;
                    curPressedKeyState |= PressedKeyState.LongClicked;
                }
            }

            // Process Notes
            if (m_longNoteTable.Count > 0 && !curPressedKeyState.HasFlag(PressedKeyState.LongClicked))
            {
                m_longNoteTable.Clear();
            }

            Guid[] noteCandidateIDs = m_noteCandidates.ToArray();
            for (int index = 0; index < m_noteCandidates.Count; index++)
            {
                Guid curNoteCandidateID = noteCandidateIDs[index];

                if (m_bisBlueRegion && NoteManager.Instance.GetRedLineCornerNote(curNoteCandidateID) == null && StageDataInterface.NoteDataInterface.GetAttachedLineID(curNoteCandidateID) != m_lineTracker.CurLineID)
                {
                    continue;
                }
                else if (m_bisGreenRegion && !m_bisOnGreenLine)
                {
                    continue;
                }

                if (NoteManager.Instance.GetRedLineCornerNote(curNoteCandidateID) != null)
                {
                    IRedLineCornerNote targetNoteItem = NoteManager.Instance.GetRedLineCornerNote(curNoteCandidateID);
                    bool bisHit = false;
                    if (targetNoteItem.BIsToLeft && curPressedKeyState.HasFlag(PressedKeyState.LeftArrowClicked))
                    {
                        bisHit = true;
                    }
                    else if (!targetNoteItem.BIsToLeft && curPressedKeyState.HasFlag(PressedKeyState.RightArrowClicked))
                    {
                        bisHit = true;
                    }

                    if (bisHit)
                    {
                        m_curScore += GetSingleNoteScore(GetYPos2Time(targetNoteItem.Transform.position.y), m_curAudioSourceTime);
                        m_combo++;

                        m_HP += 0.1f;

                        // Effect
                        targetNoteItem.Transform.gameObject.SetActive(false);
                    }
                }
                else if (StageDataInterface.NoteDataInterface.BIsNoteIDValid(curNoteCandidateID))
                {
                    StageData.NoteData curNoteData = StageDataInterface.NoteDataInterface.GetNoteData(curNoteCandidateID);
                    switch (curNoteData.CurNoteType)
                    {
                        case StageData.NoteData.NoteType.Common:
                            if (curPressedKeyState.HasFlag(PressedKeyState.SingleClicked))
                            {
                                //Debug.Log("Common Note ID : " + curNoteCandidateID);
                                INoteItem targetNoteItem = NoteManager.Instance.GetNoteItem(curNoteCandidateID);
                                m_curScore += GetSingleNoteScore(GetYPos2Time(targetNoteItem.RedAndBlueNote.Transform.position.y), m_curAudioSourceTime);
                                m_combo++;

                                m_HP += 0.1f;

                                // Effect
                                targetNoteItem.Transform.gameObject.SetActive(false);
                            }
                            break;

                        case StageData.NoteData.NoteType.Double:
                            if (curPressedKeyState.HasFlag(PressedKeyState.DoubleClicked))
                            {
                                INoteItem targetNoteItem = NoteManager.Instance.GetNoteItem(curNoteCandidateID);
                                m_curScore += GetSingleNoteScore(GetYPos2Time(targetNoteItem.RedAndBlueNote.Transform.position.y), m_curAudioSourceTime);
                                m_combo++;

                                m_HP += 0.1f;

                                // Effect
                                targetNoteItem.Transform.gameObject.SetActive(false);
                            }
                            break;

                        case StageData.NoteData.NoteType.Long:
                            if (curPressedKeyState.HasFlag(PressedKeyState.LongClicked) && !m_longNoteTable.ContainsKey(curNoteCandidateID))
                            {
                                INoteItem targetNoteItem = NoteManager.Instance.GetNoteItem(curNoteCandidateID);
                                m_longNoteTable.Add(curNoteCandidateID, new Tuple<float, float>(GetYPos2Time(targetNoteItem.GreenNote.CurveStartYPos), m_curAudioSourceTime));
                            }
                            break;
                    }
                }
            }
        }

        public float CurAudioSourceTime
        {
            set
            {
                m_curAudioSourceTime = value;
            }
        }

        public float CurScore
        {
            get
            {
                return m_curScore;
            }
        }
        public float HP
        {
            get
            {
                return m_HP;
            }
        }

        public bool BIsGameOver
        {
            get
            {
                return m_HP <= 0.0f || m_bisSomethingTroubled;
            }
        }

        /// <summary>
        /// 현재 시점에서 달성된 Combo
        /// </summary>
        public int Combo
        {
            get
            {
                return m_combo;
            }
        }

        internal bool BIsGreenRegion
        {
            set
            {
                m_bisGreenRegion = value;
            }
        }
        internal bool BIsOnGreenLine
        {
            set
            {
                m_bisOnGreenLine = value;
            }
        }

        internal bool BIsBlueRegion
        {
            set
            {
                m_bisBlueRegion = value;
            }
        }

        internal bool BIsSomethingTroubled
        {
            set
            {
                m_bisSomethingTroubled = value;
            }
        }

        /// <summary>
        /// Scene Load 시 ScoreManager GameObject의 경우 필히 StartScoring()를 호출해야 한다
        /// </summary>
        public void StartScoring()
        {
            m_bisStartScoring = true;
        }
        public void StopScoring()
        {
            m_bisStartScoring = false;
        }

        internal void PushNoteCandidate(in Guid targetNoteID)
        {
            m_noteCandidates.Push(targetNoteID);

            SortNoteCandidates();
        }
        internal void RemoveNoteCandidate(in Guid targetNoteID)
        {
            if (m_longNoteTable.ContainsKey(targetNoteID))
            {
                m_curScore += GetSingleNoteScore(m_longNoteTable[targetNoteID].Item1, m_longNoteTable[targetNoteID].Item2);
                m_combo++;

                m_HP += 0.1f;

                m_longNoteTable.Remove(targetNoteID);

                // Effect
                INoteItem targetNoteItem = NoteManager.Instance.GetNoteItem(targetNoteID);
                targetNoteItem.Transform.gameObject.SetActive(false);
            }

            List<Guid> noteCandidateBuffer = m_noteCandidates.ToList();
            if (noteCandidateBuffer.Contains(targetNoteID))
            {
                m_HP -= 0.1f;
                m_combo = 0;

                noteCandidateBuffer.Remove(targetNoteID);

                Stack<Guid> resultNoteCandidates = new Stack<Guid>();
                foreach (Guid curNoteID in noteCandidateBuffer)
                {
                    resultNoteCandidates.Push(curNoteID);
                }

                m_noteCandidates = resultNoteCandidates;

                SortNoteCandidates();
            }
        }

        private float GetSingleNoteScore(in float noteAppearTime, in float keyPressedTime)
        {
            float resultScore = 1000000.0f / (StageDataInterface.NoteDataInterface.GetNoteIDs().Length + NoteManager.Instance.RedLineCornerNoteCount);

            float timeDistance = Mathf.Abs(noteAppearTime - keyPressedTime);
            if (0.1f <= timeDistance && timeDistance < 0.2f)
            {
                resultScore *= 0.7f;
            }
            else if (0.2f <= timeDistance)
            {
                resultScore = 0.0f;
            }

            return resultScore;
        }

        private float GetYPos2Time(in float yPos)
        {
            StageData.StageConfigData stageConfig = StageDataInterface.StageConfigDataInterface.GetStageConfigData();
            float curNoteRatio = ((yPos - DeterminLine.Instance.transform.localPosition.y)
                                / (stageConfig.LengthPerBit / stageConfig.BitSubDivision)) / GridManager.Instance.GetTotalFrameCount();
            
            StageMetadata stageMetadata = StageMetadataInterface.GetStageMetadata();
            float curTime = stageMetadata.MusicLength * curNoteRatio;
            curTime -= stageConfig.MusicStartOffsetTime;

            return curTime;
        }

        private void SortNoteCandidates()
        {
            Guid[] noteIDs = StageDataInterface.NoteDataInterface.GetNoteIDs();
            Dictionary<float, List<Guid>> noteByAppearYPos = new Dictionary<float, List<Guid>>();
            foreach (Guid noteID in m_noteCandidates)
            {
                float curNoteAppearYPos = float.MinValue;
                if (noteIDs.Contains(noteID))
                {
                    curNoteAppearYPos = NoteManager.Instance.GetNoteItem(noteID).Transform.position.y;
                }
                else if (NoteManager.Instance.GetRedLineCornerNote(noteID) != null)
                {
                    curNoteAppearYPos = NoteManager.Instance.GetRedLineCornerNote(noteID).Transform.position.y;
                }

                if (!noteByAppearYPos.ContainsKey(curNoteAppearYPos))
                {
                    noteByAppearYPos.Add(curNoteAppearYPos, new List<Guid>());
                }
                noteByAppearYPos[curNoteAppearYPos].Add(noteID);
            }

            foreach (float key in noteByAppearYPos.Keys)
            {
                noteByAppearYPos[key].Sort((noteID_0, noteID_1) =>
                {
                    int result = 0;

                    float appearYPos_0 = float.MinValue;
                    if (noteIDs.Contains(noteID_0))
                    {
                        appearYPos_0 = NoteManager.Instance.GetNoteItem(noteID_0).Transform.position.y;
                    }
                    else if (NoteManager.Instance.GetRedLineCornerNote(noteID_0) != null)
                    {
                        appearYPos_0 = NoteManager.Instance.GetRedLineCornerNote(noteID_0).Transform.position.y;
                    }

                    float appearYPos_1 = float.MinValue;
                    if (noteIDs.Contains(noteID_1))
                    {
                        appearYPos_1 = NoteManager.Instance.GetNoteItem(noteID_1).Transform.position.y;
                    }
                    else if (NoteManager.Instance.GetRedLineCornerNote(noteID_1) != null)
                    {
                        appearYPos_1 = NoteManager.Instance.GetRedLineCornerNote(noteID_1).Transform.position.y;
                    }

                    if (appearYPos_0 < appearYPos_1)
                    {
                        result = -1;
                    }
                    else if (appearYPos_0 > appearYPos_1)
                    {
                        result = 1;
                    }

                    return result;
                });
            }

            var sortedKeys = noteByAppearYPos.Keys.ToList();
            sortedKeys.Sort();

            Stack<Guid> resultNoteCandidates = new Stack<Guid>();
            foreach (float key in sortedKeys)
            {
                foreach (Guid noteID in noteByAppearYPos[key])
                {
                    resultNoteCandidates.Push(noteID);
                }
            }

            m_noteCandidates = resultNoteCandidates;
        }

        protected override void Dispose(bool bisDisposing)
        {
            if (bisDisposing)
            {

            }
        }
    }
}