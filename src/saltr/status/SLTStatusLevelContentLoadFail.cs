﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.status
{
   public class SLTStatusLevelContentLoadFail : SLTStatus
    {
		public SLTStatusLevelContentLoadFail():base(SLTStatus.CLIENT_LEVEL_CONTENT_LOAD_FAIL, "Level content load has failed.")
		{
		}
    }
}