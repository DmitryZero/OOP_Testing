namespace PatentPO;
//Singleton
public class DateClass{
    public DateOnly date { get; set;} = new DateOnly();
    private static DateClass? instance;
    private static object syncRoot = new Object();
    private DateClass()
    { 

    }

    public static DateClass getInstance()
    {
        if (instance == null)
        {
            lock (syncRoot)
            {
                if (instance == null)
                    instance = new DateClass();
            }
        }
        return instance;
    }    
}