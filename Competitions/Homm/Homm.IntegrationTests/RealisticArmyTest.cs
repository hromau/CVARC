using FluentAssertions;
using HoMM;
using HoMM.ClientClasses;
using NUnit.Framework;

namespace Homm.IntegrationTests
{
    [TestFixture]
    public class RealisticArmyTest
    {
        // bounds are 1, 5, 10, 20, 40, 60, 80
        // out of bounds step is 25

        private static UnitType anyType = UnitType.Infantry;

        private static RealisticArmy Army(int preciceCount) => new RealisticArmy(new SimpleArmy { { anyType, preciceCount } });

        public static int[][] ConversionsExamples = new int[][]
        {
            // precice count, from , to
            new int[] {0, 0, 1},
            new int[] {1, 1, 4},
            new int[] {2, 1, 4},
            new int[] {3, 1, 4},
            new int[] {4, 1, 4},
            new int[] {5, 5, 9},
            new int[] {6, 5, 9},
            new int[] {7, 5, 9},
            new int[] {8, 5, 9},
            new int[] {9, 5, 9},
            new int[] {10, 10, 19},
            new int[] {16, 10, 19},
            new int[] {20, 20, 39},
            new int[] {21, 20, 39},
            new int[] {33, 20, 39},
            new int[] {39, 20, 39},
            new int[] {40, 40, 59},
            new int[] {50, 40, 59},
            new int[] {60, 60, 79},
            new int[] {70, 60, 79},
            new int[] {80, 80, 104},
            new int[] {100, 80, 104},
            new int[] {105, 105, 129},
            new int[] {125, 105, 129},
            new int[] {130, 130, 154},
            new int[] {140, 130, 154},
            new int[] {155, 155, 179},
        };

        [Test]
        [TestCaseSource(nameof(ConversionsExamples))]
        public void RealisticArmy_TestConversion(int preciceCount, int expectedFrom, int expectedTo)
        {
            var army = Army(preciceCount);
            army[anyType].From.Should().Be(expectedFrom);
            army[anyType].To.Should().Be(expectedTo);
        }
    }
}
