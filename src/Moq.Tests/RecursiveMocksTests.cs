using Xunit;
using static Moq.Syntax;

namespace Moq.Tests.Recursive
{
    public class RecursiveMocksTests
    {
        [Fact]
        public void CanSetupRecursiveMockProperty()
        {
            var mock = new Mock<IRecursiveRoot>();

            Setup(() => mock.Object.Branch.Leaf.Name).Returns("foo");

            Assert.Equal("foo", mock.Object.Branch.Leaf.Name);
        }

        [Fact]
        public void CanSetupRecursiveMockMethod()
        {
            var mock = new Mock<IRecursiveRoot>();

            Setup(() => mock.Object.Branch.GetLeaf(1).Name).Returns("foo");

            Assert.Equal("foo", mock.Object.Branch.GetLeaf(1).Name);
            Assert.Null(mock.Object.Branch.GetLeaf(0));
        }

        [Fact]
        public void CanSetupRecursiveMockMethodInSetupScope()
        {
            var mock = new Mock<IRecursiveRoot>();
            IRecursiveLeaf leaf;

            using (Setup())
            {
                leaf = mock.Object.Branch.GetLeaf(1);
            }

            Mock.Get(leaf).Name.Returns("foo");

            Assert.Equal("foo", mock.Object.Branch.GetLeaf(1).Name);
            Assert.Null(mock.Object.Branch.GetLeaf(0));
        }

        [Fact]
        public void ReusesRecursiveMockAcrossSetups()
        {
            var mock = new Mock<IRecursiveRoot>();

            Setup(() => mock.Object.Branch.Leaf.Name).Returns("foo");
            var branch = mock.Object.Branch;

            Setup(() => mock.Object.Branch.GetLeaf(1).Name).Returns("bar");

            Assert.Same(branch, mock.Object.Branch);
            Assert.Equal("foo", mock.Object.Branch.Leaf.Name);
            Assert.Equal("bar", mock.Object.Branch.GetLeaf(1).Name);
        }
    }

    public interface IRecursiveRoot
    {
        IRecursiveBranch Branch { get; }
    }

    public interface IRecursiveBranch
    {
        IRecursiveLeaf Leaf { get; }

        IRecursiveLeaf GetLeaf(int index);
    }

    public interface IRecursiveLeaf
    {
        string Name { get; set; }
    }
}
