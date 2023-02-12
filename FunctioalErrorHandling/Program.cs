using FunctioalErrorHandling;

while (true)
{
    Console.WriteLine("----------------------------------------------------");
    try
    {
        int? data = null;
        try
        {
            data = Query.Read();
        }
        catch (Exception e)
        { 
            Console.WriteLine($"can not read data. error:{e.Message}");
            continue;
        }
        
        Console.WriteLine($"current value: {data}");
        Console.Write("divide it by: ");
        string input = Console.ReadLine();
        int? divisor = null;
        try
        {
            divisor = int.Parse(input);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{input} is not valid a integer");
            continue;
        }
        int? result = null;
        try 
        {
            result = Domain.calculate(data.Value, divisor.Value);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{data.Value} can not be divided by {divisor.Value}. error:{e.Message}");
            continue;
        }
        try 
        {
            Repository.Save(result.Value);
        }
        catch (Exception e)
        {
            Console.WriteLine($"saving {result.Value} failed. error:{e.Message}");
            continue;
        }

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
    public static Result<int, Exception> SafeRead() => Result<int, Exception>.Invoke(EF.Read);
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
