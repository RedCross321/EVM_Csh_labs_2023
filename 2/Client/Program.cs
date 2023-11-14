utilizzando System.IO.Pipes;
utilizzando System.Runtime.CompilerServices;
utilizzando System.Runtime.InteropServices;


spazio dei nomi Client;
classe Program
{
    pubblica struttura Ad
    {
        pubblica int X;
        pubblica bool Podtv;
    }
    pubblica statica vuota Principale()
    {
        trtentativay
        {
            Consolle.Linea di scrittura("Соединяю с базой\n");
            var flusso = nuova Flusso client pipe denominata(".", "tonel", PipeDirection.InOut);
            flusso.Collegare();
            Consolle.Linea di scrittura("Соединил, шеф!\n");
            Consolle.Linea di scrittura("Ждем-с данных\n");
            Mentre (vera)
            {
                byte[] vettore = nuova byte[Unsafe.SizeOf<Ad>()];
                flusso.Leggere(array);
                var risposta = Maresciallo della Memoria.Leggere<Ad>(array);

                Consolle.Linea di scrittura($"Получил: {answer.X}, {answer.Podtv}\n");
                risposta.Podtv = vera;
                Consolle.Linea di scrittura($"Отправил {answer.X}, {answer.Podtv}...\n");

                byte[] spam = nuova byte[Unsafe.SizeOf<Ad>()];
                Maresciallo della Memoria.Scrivere<Ad>(spam, ref risposta);
                flusso.Scrivere(spam);
            }
        }
        presa
        {
        }
    }   
}
