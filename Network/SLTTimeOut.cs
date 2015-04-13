
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Saltr.UnitySdk.Network
{
    public class SLTTimeOut
    {
        public float Timeout { get; set; }

        public float BeforProgress { get; set; }

        public float BeforTime { get; set; }

        public bool CheckTimeout(float progress)
        {
            float now = Time.time;
            if ((now - BeforTime) > Timeout)
            {
                // timeout
                return true;
            }
            // update progress
            if (BeforProgress != progress)
            {
                BeforProgress = progress;
                BeforTime = now;
            }
            return false;
        }
    }
}
