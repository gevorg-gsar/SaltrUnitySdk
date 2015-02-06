﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk
{
    public class ExceptionConstants
    {
        public const string EmailIsRequired = "Email is required.";
        public const string DeviceIdIsRequired = "Field 'deviceId' is required.";

        public const string SaltrFailedLoadAppData = "[SALTR] Failed to load appData.";
        public const string SaltrFailedDecodeLevels = "[SALTR] Failed to decode Levels.";
        public const string SaltrFailedLoadContent = "[SALTR] Level content load has failed.";
        public const string SaltrFailedDecodeFeatures = "[SALTR] Failed to decode Features.";
        public const string SaltrFailedDecodeExperiments = "[SALTR] Failed to decode Experiments.";
        public const string SaltrAppDataLoadRefused = "[SALTR] AppData load refused. Previous load is not complete";
        public const string SaltrFailedFindParserForLevelType = "[SALTR] Failed to find parser for current level type.";
        
    }
}