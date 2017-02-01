using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Generators
{
    public interface IMapGenerator
    {
        Map GenerateMap(int size);
    }
}
