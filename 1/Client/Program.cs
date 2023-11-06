using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Client;

class Program
{
    public struct Ad
    {
        public int X;
        public int Y;
        public bool Podtv;
    }
    public static void Main()
    {
        Console.WriteLine("Соединяю с базой\n");

        var stream = new NamedPipeClientStream(".", "tonel", PipeDirection.InOut);
        stream.Connect();

        Console.WriteLine("Соединил, шеф!\n");

        Console.WriteLine("Ждем-с данных\n");

        byte[] array = new byte[Unsafe.SizeOf<Ad>()];
        stream.Read(array);
        var answer = MemoryMarshal.Read<Ad>(array);

        Console.WriteLine($"Получил: {answer.X}, {answer.Y}, {answer.Podtv}\n");

        answer.X += answer.Y;
        answer.Y -= answer.X;
        answer.Podtv = true;

        Console.WriteLine($"Отправил {answer.X}, {answer.Y}, {answer.Podtv}...\n");

        byte[] spam = new byte[Unsafe.SizeOf<Ad>()];
        MemoryMarshal.Write<Ad>(spam, ref answer);
        stream.Write(spam);
    }
}