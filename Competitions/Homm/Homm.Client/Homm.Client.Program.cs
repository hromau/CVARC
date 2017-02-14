using System;
using System.Linq;
using HoMM.Sensors;
using HoMM;
using HoMM.Rules;
using HoMM.ClientClasses;
using System.Collections.Generic;

namespace Homm.Client
{
    class Program
    {
        // Вставьте сюда свой личный CvarcTag для того, чтобы учавствовать в онлайн соревнованиях.
        public static readonly Guid CvarcTag = Guid.Parse("00000000-0000-0000-0000-000000000000");


        public static void Main()
        {
            TextGame.Start();
        } 
    }
}