using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ui;
using Xunit;
using Xunit.Sdk;

namespace uiTest
{
    public class PathDefinitionTests
    {
        private static PathDefinition NicePathDefinition => new()
        {
            ColCount = 10,
            RowCount = 10,
            EffectiveKeyString = "abccda",
            Coords = NicePathElems
        };

        private static readonly ImmutableArray<CellCoord> NicePathElems = new CellCoord[]
        {
            /*
             ...
             .d.
             cba
             */
            CellCoord.Create(2, 2),
            CellCoord.Create(0, 2),
            CellCoord.Create(0, 2),
            CellCoord.Create(1, 1),
            CellCoord.Create(2, 2)
        }.ToImmutableArray();

        public static TheoryData<PathDefinition, int, string> ValidationFixtures = new()
        {
            {NicePathDefinition, 0, ""},
            {NicePathDefinition with {Coords = NicePathElems.Take(4)}, 1, "not enough" },
            {NicePathDefinition with {Coords = NicePathElems.Take(4).Append(CellCoord.Create(1, 2))}, 1, "conflict"},
            {NicePathDefinition with {EffectiveKeyString = null}, 1, "No key string"},
            {NicePathDefinition with {EffectiveKeyString = string.Empty}, 1, "Empty key string"},
            {NicePathDefinition with {ColCount = 9}, 1, "too few cols"},
            {NicePathDefinition with {ColCount = 101}, 1, "too many cols"}, 
            {NicePathDefinition with {RowCount = 9}, 1, "too few rows"}, 
            {NicePathDefinition with {RowCount = 101}, 1, "too many rows"},
            {NicePathDefinition with {Coords = NicePathElems.Append(CellCoord.Create(11, 11))}, 1, "illegal coord"} 
        };

        [Theory]
        [MemberData(nameof(ValidationFixtures))]
        public void ValidationWorks(PathDefinition model, int expectedErrorCount, string testMsg)
        {
            var result = new List<ValidationResult>();
            Validator.TryValidateObject(model, new ValidationContext(model), result, true);
            try
            {
                Assert.Equal(expectedErrorCount, result.Count);
            }
            catch (EqualException ex)
            {
                throw new XunitException($"{testMsg}\n{ex}");
            }
        }
    }
}