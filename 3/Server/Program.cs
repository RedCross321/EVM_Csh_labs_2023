using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server;
public struct Ad
{
    public double A;
    public double B;
}
public struct Ud
{
    public double Result;
    public override string ToString() => $"Res = {Result}";
}
internal class Program
{
    private static int id = 0;
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
                var data = new Ad();
                Console.WriteLine($"Введите значение A -> ");
                var nach = Console.ReadLine();
                if (!double.TryParse(nach, out double zalupa))
                {
                    Console.WriteLine("Ты не ввел цифры, попробуй заново\n");
                    continue;
                }
                else
                {
                    data.A = zalupa;
                }
                Console.WriteLine($"Введите значение B -> ");
                var conch = Console.ReadLine();
                if (!double.TryParse(conch, out double ochko))
                {
                    Console.WriteLine("Ты не ввел цифры, попробуй заново\n");
                    continue;
                }
                else
                {
                    data.B = ochko;
                }


                mutex.WaitOne();
                queue.Enqueue(data, 1);
                mutex.ReleaseMutex();
            }
        });
    }

    private static Task serverTask(CancellationToken token)
    {
        return Task.Run(() =>
        {
            List<Ud> uds = new List<Ud>();
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (queue.Count >= 1)
                    {
                        mutex.WaitOne();
                        var data = queue.Dequeue();
                        mutex.ReleaseMutex();
                        Task task_3 = runclient(token, uds, data);
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
    private static async Task runclient(CancellationToken token, List<Ud> uds, Ad data) 
    {
        id++;
        string name = $"tonel_{id}";
        using (Process myProcess = new Process())
        {
            myProcess.StartInfo.FileName = "C:\\Users\\akbeke\\Desktop\\3\\Client\\bin\\Debug\\net7.0\\Client.exe";
            myProcess.StartInfo.Arguments = name;
            myProcess.Start();


            var stream = new NamedPipeServerStream($"{name}", PipeDirection.InOut);
            await stream.WaitForConnectionAsync();

            byte[] spam = new byte[Unsafe.SizeOf<Ad>()];

            MemoryMarshal.Write(spam, ref data);

            await stream.WriteAsync(spam, token);
            byte[] array = new byte[Unsafe.SizeOf<Ud>()];
            
            
            await stream.ReadAsync(array, token);

            uds.Add(MemoryMarshal.Read<Ud>(array));
            await myProcess.WaitForExitAsync();


        }
    }
    static async Task Main(string[] args)
    {
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            up.Cancel();
        };

        Console.WriteLine("Клиент подключен!\n");

        Task task_1 = serverTask(token);
        Task task_2 = clientTask(token);
        await Task.WhenAll(task_1, task_2);
    }
}
