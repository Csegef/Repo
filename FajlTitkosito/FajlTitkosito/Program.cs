using System.Security.Cryptography;
using System.Text;

namespace FajlTitkosito
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Titkosítás");

            // Adatok a titkosításhoz
            string fajlnev = "titkos.txt";
            string kulcs = "Titok_12";
            string titkositando = "Szigorúan titkos tartalom";

            // Titkosítás
            byte[] titkositottAdatok = Titkositas(fajlnev, kulcs, titkositando);
            File.WriteAllBytes("titkositott.bin", titkositottAdatok);
            Console.WriteLine("Titkosított fájl kiírva: titkositott.bin");

            Console.WriteLine("\nVisszafejtés:");

            // Visszafejtés
            (string eredetiFajlnev, string visszafejtettTartalom) = Visszafejtes(kulcs, "titkositott.bin");

            Console.WriteLine($"Eredeti fájlnév: {eredetiFajlnev}");
            Console.WriteLine($"Visszafejtett tartalom: {visszafejtettTartalom}");
        }

        static byte[] Titkositas(string fajlnev, string kulcs, string titkositando)
        {
            // Titkosító algoritmus és hash
            using Aes aes = Aes.Create();
            using SHA256 sha256 = SHA256.Create();

            // Kulcs és hash-ek generálása
            byte[] binKulcs = sha256.ComputeHash(Encoding.UTF8.GetBytes(kulcs));
            byte[] tartalomHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(titkositando));
            byte[] fajlnevBin = Encoding.UTF8.GetBytes(fajlnev);
            byte[] titkositandoBin = Encoding.UTF8.GetBytes(titkositando);

            // Inicializációs vektor
            aes.GenerateIV();
            aes.Key = binKulcs;

            // Titkosítás
            using ICryptoTransform titkosito = aes.CreateEncryptor(binKulcs, aes.IV);
            byte[] titkositott = titkosito.TransformFinalBlock(titkositandoBin, 0, titkositandoBin.Length);
            int tartalomHossz = titkositott.Length;

            // Adatok összeállítása memóriába
            using MemoryStream ms = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(ms);

            // Formátum: IV + fájlnév hossza + fájlnév + tartalom hash + tartalom hossza + titkosított tartalom
            writer.Write(aes.IV);
            writer.Write(fajlnevBin.Length);  // fájlnév hossza (4 byte)
            writer.Write(fajlnevBin);         // fájlnév
            writer.Write(tartalomHash);       // tartalom hash (32 byte)
            writer.Write(tartalomHossz);      // titkosított tartalom hossza (4 byte)
            writer.Write(titkositott);        // titkosított tartalom

            return ms.ToArray();
        }

        static (string fajlnev, string tartalom) Visszafejtes(string kulcs, string titkositottFajl)
        {
            // Beolvassuk a titkosított fájlt
            byte[] titkositottAdatok = File.ReadAllBytes(titkositottFajl);

            using MemoryStream ms = new MemoryStream(titkositottAdatok);
            using BinaryReader reader = new BinaryReader(ms);

            // Adatok kiolvasása a megadott formátumban
            byte[] iv = reader.ReadBytes(16);  // IV (16 byte)
            int fajlnevHossz = reader.ReadInt32();  // fájlnév hossza
            byte[] fajlnevBin = reader.ReadBytes(fajlnevHossz);  // fájlnév
            byte[] tartalomHash = reader.ReadBytes(32);  // tartalom hash (32 byte)
            int titkositottHossz = reader.ReadInt32();  // titkosított tartalom hossza
            byte[] titkositottTartalom = reader.ReadBytes(titkositottHossz);  // titkosított tartalom

            // Fájlnév visszaalakítása
            string fajlnev = Encoding.UTF8.GetString(fajlnevBin);

            // Visszafejtés
            using Aes aes = Aes.Create();
            using SHA256 sha256 = SHA256.Create();

            byte[] binKulcs = sha256.ComputeHash(Encoding.UTF8.GetBytes(kulcs));
            aes.Key = binKulcs;
            aes.IV = iv;

            using ICryptoTransform visszafejto = aes.CreateDecryptor(binKulcs, iv);
            byte[] visszafejtettBin = visszafejto.TransformFinalBlock(titkositottTartalom, 0, titkositottTartalom.Length);

            // Tartalom visszaalakítása
            string tartalom = Encoding.UTF8.GetString(visszafejtettBin);

            // Hash ellenőrzése
            byte[] ellenorzoHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(tartalom));
            if (!ellenorzoHash.SequenceEqual(tartalomHash))
            {
                throw new InvalidOperationException("A hash ellenőrzés sikertelen! A fájl sérült vagy a kulcs helytelen.");
            }

            return (fajlnev, tartalom);
        }
    }
}