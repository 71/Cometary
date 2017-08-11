using Shouldly;
using Xunit;

namespace Cometary.Tests
{
    public class CompositionTests
    {
        [Fact]
        public void ShouldApplyDynamicComponent()
        {
            ImmutablePerson greg = new ImmutablePerson("Greg", new Pet());
            ImmutablePerson bob = greg.WithName("Bob");

            greg.ShouldNotBe(bob);
            greg.Pet.ShouldBe(bob.Pet);

            greg.Name.ShouldBe("Greg");
            bob.Name.ShouldBe("Bob");
        }

        [Fact]
        public void ShouldApplySimpleComponent()
        {
            Pet pet = new Pet { Name = "Jerry" };

            pet.Name = "Jerry";
            pet.Name.ShouldBe("Jerry");
        }
    }
}
