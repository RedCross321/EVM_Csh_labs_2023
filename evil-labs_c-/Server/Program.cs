using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace Server;

internal class Program
{

    static CancellationTokenSource up = new CancellationTokenSource();
    static CancellationToken dawn = up.Token;

    static PriorityQueue<Ad, int> queue = new PriorityQueue<Ad, int>();


    static Mutex mutex = new Mutex();

    public struct Ad
    {

        public int X;

        public bool Podtv;

        public override string ToString() => $"Данные = {X}, Ответ = {Podtv}";
    }
    static async Task Main(string[] args)
    {
        Console.WriteLine("Жду клиентика\n");
        var stream = new NamedPipeServerStream("tonel", PipeDirection.InOut);



        var clientTask = Task.Run(() => Client(stream, dawn));
        var serverTask = Task.Run(() => Server(stream, dawn));


        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            up.Cancel();
        };

        await Task.WhenAll(serverTask, clientTask);
       // Console.WriteLine("Всё");

    }

    static async void Server(NamedPipeServerStream stream, CancellationToken cancellationToken)
    {
        stream.WaitForConnection();
        Console.WriteLine("Клиент подключен!\n");



        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine($"Введите значение -> ");
            var value = Console.ReadLine();
            if (value.Length == 0)
            {
                Console.WriteLine("Ты не ввел цифры, попробуй заново\n");
                continue;
            }


            Console.WriteLine($"Введите приоритет -> ");
            var priority = Console.ReadLine();
            if (priority.Length == 0)
            {
                Console.WriteLine("Ты не ввел цифры, попробуй заново\n");
                continue;
            }



            var data = new Ad() { X = Convert.ToInt32(value), Podtv = false };
            //Console.WriteLine($"Отправляю {data.X}, {data.Podtv}\n");
            

            

            mutex.WaitOne();
            queue.Enqueue(data, Convert.ToInt32(priority));
            mutex.ReleaseMutex();

        }

        await Task.Delay(1000);
    }
    static async void Client(NamedPipeServerStream stream, CancellationToken cancellationToken)
    {
     
        List<Ad> uds = new List<Ad>();

        while (!cancellationToken.IsCancellationRequested)
        {
            if (queue.Count >= 1)
            {
                /*
                mutex.WaitOne();
                var data = queue.Dequeue();
                mutex.ReleaseMutex();
                byte[] spam = new byte[Unsafe.SizeOf<Ad>()];
                MemoryMarshal.Write<Ad>(spam, ref data);
                stream.Write(spam);
                byte[] array = new byte[Unsafe.SizeOf<Ad>()];
                stream.Read(array);
                uds.Add(MemoryMarshal.Read<Ad>(array));
                */
            }
        }
        while (queue.Count > 0)
        {
            var data = queue.Dequeue();
            byte[] spam = new byte[Unsafe.SizeOf<Ad>()];
            MemoryMarshal.Write<Ad>(spam, ref data);
            stream.Write(spam);
            byte[] array = new byte[Unsafe.SizeOf<Ad>()];
            stream.Read(array);
            uds.Add(MemoryMarshal.Read<Ad>(array));

        }
        foreach (var item in uds)
        {
            Console.WriteLine(item);
        }
        await Task.Delay(1000);
    }
}