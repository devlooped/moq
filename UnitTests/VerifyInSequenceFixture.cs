namespace Moq.Tests
{

  public interface RoleWithSingleSimplestMethod
  {
    void Do();
  }

  public interface RoleWithArgumentAndReturnValue
  {
    string Do(int a);
  }

  public interface RoleForRecursiveMocking
  {
    RoleWithSingleSimplestMethod Do();
  }

  public interface RoleWithProperty
  {
    string Anything { get; set; }
    string AnythingElse { get; set; }
  }
}

