utilizzando System.IO.Pipes;
utilizzando System.Runtime.CompilerServices;
utilizzando System.Runtime.InteropServices;

dello spazio dei nomi Server;
struttura pubblica Ad
{
    pubblico int X;
    pubblico bool Podtv;
    pubblico oltrepassare corda ToCorda() => $"Данные = {X}, Ответ = {Podtv}";
}

classe interna Program
{
    statica CancellationTokenSource up = nuova CancellationTokenSource();
    statica CancellationToken token = up.Token;
    statica PriorityQueue<Ad, int> queue = nuova PriorityQueue<Ad, int>();
    statica Mutex mutex = nuova Mutex();
    privata statica Compito clientCompito(CancellationToken token)
    {
        ritorno Compito.Correre(() =>
        {
            Mentre (!token.IsCancellationRequested)
            {
                Consolle.Linea di scrittura($"Введите значение -> ");
                var valore = Consolle.Linea di lettura();
                Se (valore.Lunghezza == 0)
                {
                    Consolle.Linea di scrittura("Ты не ввел цифры, попробуй заново\n");
                    Continua;
                }

                Consolle.Linea di scrittura($"Введите приоритет -> ");
                var priorità = Consolle.Linea di lettura();
                Se (priorità.Lunghezza == 0)
                {
                    Consolle.Linea di scrittura("Ты не ввел цифры, попробуй заново\n");
                    Continua;
                }
                
                dati var = nuova Ad() { X = Convert.ToInt32(valore), Podtv = falsa };

                mutex.AspettaUno();
                coda.Accodare(data, Convertire.ToInt32(priorità));
                mutex.RilasciaMutex();
            }
        });
    }

    privata statica Compito serverCompito(NamedPipeServerStream stream, CancellationToken token)
    {
        ritorno Compito.Correre(() =>
        {
            Elenco<Ad> uds = nuova Elenco<Ad>();
            Mentre (!token.IsCancellationRequested)
            {
                tentativa
                {
                    Se (queue.Count >= 1)
                    {
                        mutex.AspettaUno();
                        dati var = coda.Decoda();
                        mutex.RilasciaMutex();
                        byte[] spam = nuova byte[Unsafe.SizeOf<Ad>()];
                        Maresciallo della Memoria.Scrivere<Ad>(spam, ref data);
                        stream.Scrivere(spam);
                        byte[] array = nuova byte[Unsafe.SizeOf<Ad>()];
                        stream.Leggere(array);
                        uds.Aggiungere(MemoryMarshal.Read<Ad>(array));
                    }
                }
                presa (Eccezione)
                {                   
                }
            }
            
            per ciascuno (var item in uds)
            {
                Consolle.Linea di scrittura(item);
            }
        });
    }
    
    statica asincrona Compito Principale(string[] args)
    {
        Console.AnnullaTastoPress += (s, e) =>
        {
            e.Annulla = true;
            up.Annulla();
        };
        Consolle.Linea di scrittura("Жду клиентика\n");
        var flusso = nuova NamedPipeServerStream("tonel", PipeDirection.InOut);
        flusso.
        Attendi la connessione();
        Consolle.Linea di scrittura("Клиент подключен!\n");
        Compito task_1 = serverCompito(stream, token);
        Compito task_2 = clientCompito(token);
        attendere Compito.QuandoTutto(task_1, task_2);
    }
}
