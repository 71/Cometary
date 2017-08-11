namespace Cometary.Tests
{
    using Composition;

    [Compose(typeof(Immutable))]
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

    [Compose(typeof(Named))]
    public class Pet
    {
    }
}
