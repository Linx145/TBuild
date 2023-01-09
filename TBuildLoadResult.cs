using System;
using System.Collections.Generic;

namespace TBuild
{
    public readonly struct TBuildLoadResult
    {
        public readonly List<int> chests;
        

        public TBuildLoadResult(List<int> chests)
        {
            this.chests = chests;
        }
    }
}
