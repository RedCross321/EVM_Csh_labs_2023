using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace Server;

internal class Program
{
    public struct Ad
    {
        public int X;
        public int Y;
        public bool Podtv;
    }
    public static void Main()
    {
        var data = new Ad {X = 111, Y = 555, Podtv = false };

        Console.WriteLine("Жду клиентика\n");

        var stream = new NamedPipeServerStream("tonel", PipeDirection.InOut);
        stream.WaitForConnection();

        Console.WriteLine("Клиент подключен!\n");
        Console.WriteLine($"Отправляю {data.X}, {data.Y}, {data.Podtv}\n");

        byte[] spam = new byte[Unsafe.SizeOf<Ad>()];
        MemoryMarshal.Write<Ad>(spam, ref data);
        stream.Write(spam);

        byte[] array = new byte[Unsafe.SizeOf<Ad>()];
        stream.Read(array);
        var answer = MemoryMarshal.Read<Ad>(array);

        Console.WriteLine($"Ответ: {answer.X}, {answer.Y}, {answer.Podtv}\n");
    }
}