using System;

namespace ClassLibrary1
{
    public class Data
    {
        public string Name { get; set; }
    }

    public interface IX<in T>
    {
        void Do(T dto);
    }

    public class X : IX<Data>
    {

        public void Do(Data dto)
        {
            Console.WriteLine(dto.Name);
        }
    }
 
}
