namespace Cometary.Tests
{
    using Composition;

    [Apply(typeof(Immutable))]
    public class ImmutablePerson
    {
        public string Name { get; }
        public int Age { get; }
        public Pet Pet { get; }

        public ImmutablePerson(string name, Pet pet)
        {
            Name = name;
            Age = 0;
            Pet = pet;
        }
    }

    [CopyFrom(typeof(Named))]
    public class Pet
    {
    }

    [Global]
    public class PetFactory
    {
        public Pet Create() => new Pet();
    }
}
