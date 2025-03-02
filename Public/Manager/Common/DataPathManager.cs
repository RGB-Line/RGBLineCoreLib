using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


namespace RGBLineCoreLib
{
    public sealed class DataPathManager : SingleTonForGameObject<DataPathManager>
    {
        [SerializeField] private DataPathTable m_dataPathTable;


        public void Awake()
        {
            SetInstance(this);
        }

        internal DataPathTable DataPathTable
        {
            get
            {
                return m_dataPathTable;
            }
        }

        protected override void Dispose(bool bisDisposing)
        {
            if(bisDisposing)
            {
                m_dataPathTable = null;
            }
        }
    }
}