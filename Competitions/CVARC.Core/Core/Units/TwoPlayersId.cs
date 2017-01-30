using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVARC.V2
{
    public class TwoPlayersId
    {
        public const string Left = "Left";
        public const string Right = "Right";
        public static IEnumerable<string> Ids
        {
            get
            {
                yield return Left;
                yield return Right;
            }
        }


    }
}
