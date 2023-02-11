using FunctioalErrorHandling;

while (true)
{
    try
    {
        int data = Query.Read();
        Console.WriteLine($"current value: {data}");
        Console.Write("divide it by: ");
        string input = Console.ReadLine();
        int divider = int.Parse(input);
        int result = Domain.calculate(data, divider);
        Repository.Save(result);
        Console.WriteLine($"{result} saved");
    }
    catch (Exception e)
    {
        Console.WriteLine($"error of type {e.GetType().Name} : {e.Message}");
    }
}
public static class Domain
{
    public static int calculate(int data, int div) => data / div;
}
public static class Query
{
    public static int Read() => EF.Read();
}
public static class Repository
{
    public static void Save(int data) => EF.Save(data);
}
public static class EF
{ 
    public static int Read() 
    {
        var randomValue = new Random(DateTime.Now.Millisecond).Next(0, 5);
        if (randomValue == 1) throw new RandomException("random text");
        return randomValue;
    }
    public static void Save(int data)
    {
        var randomValue = new Random(DateTime.Now.Millisecond).Next(0, 5);
        if (randomValue == 2) throw new AnotherRandomException("random text");
    }
}
