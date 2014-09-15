using Susanoo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;

namespace JsonTester
{
    class Program
    {
        static ICommandProcessor<dynamic, ZipCodeModel>  command =
            CommandManager.DefineCommand("SELECT * FROM dbo.ZipCode", System.Data.CommandType.Text)
                    .DefineResults<ZipCodeModel>()
                    .Finalize();

        static void Main(string[] args)
        {
            using (var db = new DatabaseManager("MedTrack2"))
            {
                var sw = new Stopwatch();

                sw.Start();
                JsonConvert.SerializeObject(command.Execute(db));
                sw.Stop();

                Console.WriteLine(sw.ElapsedMilliseconds);

                sw.Reset();
                sw.Start();
                command.ExecuteToJson(db);
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);

                sw.Reset();
                
                sw.Start();
                GZip(JsonConvert.SerializeObject(command.Execute(db)));
                sw.Stop();

                Console.WriteLine(sw.ElapsedMilliseconds);

                sw.Reset();

                sw.Start();
                GZip(command.ExecuteToJson(db));
                
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
                

                Console.ReadLine();
            }
        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] GZip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionLevel.Fastest, false))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }
    }



    public class ZipCodeModel
    {
        public int Zipcode { get; set; }
        public string State { get; set; }
        public string FipsRegion { get; set; }
        public string City { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
