using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Generators
{
    public class RandomGenerator
    {
        protected readonly Random random;

        public RandomGenerator(Random random)
        {
            this.random = random;
        }
    }
}
