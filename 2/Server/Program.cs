using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Server;
public struct Ad
{
    public int X;
    public bool Podtv;
    public override string ToString() => $"Данные = {X}, Ответ = {Podtv}";
}
internal class Program
{
    static CancellationTokenSource up = new CancellationTokenSource();
    static CancellationToken token = up.Token;
    static PriorityQueue<Ad, int> queue = new PriorityQueue<Ad, int>();
    static Mutex mutex = new Mutex();
    private static Task clientTask(CancellationToken token)
    {
        return Task.Run(() =>
        {
            while (!token.IsCancellationRequested)
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

                mutex.WaitOne();
                queue.Enqueue(data, Convert.ToInt32(priority));
                mutex.ReleaseMutex();
            }
        });
    }

    private static Task serverTask(NamedPipeServerStream stream, CancellationToken token)
    {
        return Task.Run(() =>
        {
            List<Ad> uds = new List<Ad>();
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (queue.Count >= 1)
                    {
                        mutex.WaitOne();
                        var data = queue.Dequeue();
                        mutex.ReleaseMutex();
                        byte[] spam = new byte[Unsafe.SizeOf<Ad>()];
                        MemoryMarshal.Write<Ad>(spam, ref data);
                        stream.Write(spam);
                        byte[] array = new byte[Unsafe.SizeOf<Ad>()];
                        stream.Read(array);
                        uds.Add(MemoryMarshal.Read<Ad>(array));
                    }
                }
                catch (Exception)
                {                   
                }
            }
            foreach (var item in uds)
            {
                Console.WriteLine(item);
            }
        });
    }
    
    static async Task Main(string[] args)
    {
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            up.Cancel();
        };
        Console.WriteLine("Жду клиентика\n");
        var stream = new NamedPipeServerStream("tonel", PipeDirection.InOut);
        stream.WaitForConnection();
        Console.WriteLine("Клиент подключен!\n");
        Task task_1 = serverTask(stream, token);
        Task task_2 = clientTask(token);
        await Task.WhenAll(task_1, task_2);
    }
}