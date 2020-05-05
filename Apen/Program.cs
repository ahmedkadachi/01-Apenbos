using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Threading;

namespace Apen
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int aantalBomen = 10;
            List<Boom> bomen = new List<Boom>();
            List<Aap> apen = new List<Aap>();
            Random r = new Random();

            //bos aanmaken
            Bos bos1 = new Bos(1,0, 100, 0, 100);

            //willekeurige bomen aanmaken
            bomen = MaakBomenAan(aantalBomen, bos1);
            
            //apen aanmaken
            apen = MaakApenAan();

            //Start Programma
            //Startplaats apen bepalen <!>GEEN 2 apen op dezelfde boom<!>
            PlaatsAapOpEenBoom(apen, bomen);

            //springen naar de dichts bijzijnde boom
            //als de afstand van de rand kleiner is dan de afstand van de boom dan gaat hij weg.
            Spring(apen, bomen, bos1);


            //-----------------------------------------------------------------------------------------------
            //naar mijn databank schrijven
            //Thread t = new Thread(() => SchrijvDatabank(apen, bomen, bos1));
            //t.Start();

            //naar een txt bestand schrijven
            //Thread t2 = new Thread(() => MaakTextBestandAan(apen, bomen));
            //t2.Start();

            //elips tekenen
            //Thread t3 = new Thread(() => TekenElips(apen, bomen, bos1));
            //t3.Start();

            await Task.Run(() => SchrijvDatabank(apen, bomen, bos1));
            await Task.Run(() =>MaakTextBestandAan(apen, bomen));
            await Task.Run(() =>TekenElips(apen, bomen, bos1));
        }

        private static async Task TekenElips(List<Aap> apen, List<Boom> bomen, Bos bos1)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\"+bos1.ID+"_escapeRoute.jpg";
            Bitmap bm = new Bitmap((bos1.XMax - bos1.XMin), (bos1.YMax - bos1.YMin));

            Graphics g = Graphics.FromImage(bm);
            Pen PenTree = new Pen(Color.Green, 1);
            
            
            //alle bomen tekenen
            foreach(Boom element in bomen)
            {
                g.DrawEllipse(PenTree, element.X, element.Y, 10, 10);
            }

            //tekenen van de eerste aap
            foreach(Aap element in apen)
            {
                Pen mijnPen = new Pen(Color.Red, 1);
                Brush mijnBrush = new SolidBrush(Color.Red);
                switch (element.ID)
                {
                    case 0: mijnPen = new Pen(Color.Red, 1);
                        mijnBrush = new SolidBrush(Color.Red);
                        break;
                    case 1:
                        mijnPen = new Pen(Color.Blue, 1);
                        mijnBrush = new SolidBrush(Color.Blue);
                        break;
                    case 2:
                        mijnPen = new Pen(Color.Yellow, 1);
                        mijnBrush = new SolidBrush(Color.Yellow);
                        break;
                    case 3:
                        mijnPen = new Pen(Color.Orange, 1);
                        mijnBrush = new SolidBrush(Color.Orange);
                        break;
                    case 4: mijnPen = new Pen(Color.White, 1);
                        mijnBrush = new SolidBrush(Color.White);
                        break;
                    default:
                        mijnPen = new Pen(Color.Red, 1);
                        mijnBrush = new SolidBrush(Color.Red);
                        break;
                }
                
                 for(int i = 0; i< element.Boom_Lijst.Count-1;i++)
                {
                    Console.Write(" T-START ");
                    if(i == 0)
                        await Task.Run(()=>g.FillEllipse(mijnBrush, element.Boom_Lijst[i].X, element.Boom_Lijst[i].Y, 10, 10));

                    if (element.Boom_Lijst[i+1].X != -1)
                       await Task.Run(()=> g.DrawLine(mijnPen, element.Boom_Lijst[i].X, element.Boom_Lijst[i].Y, element.Boom_Lijst[i+1].X, element.Boom_Lijst[i+1].Y));
                    Console.Write(" T-STOP ");
                }
            }
            bm.Save(path, ImageFormat.Jpeg);
        }

        private static async Task SchrijvDatabank(List<Aap> apen, List<Boom> bomen, Bos bos1)
        {
            DataBeheer databank = new DataBeheer(@"Data Source=MSI\SQLEXPRESS;Initial Catalog=Apen;Integrated Security=True");

            ////tabel woodRecords opvullen
            //databank.VerwijderAlleBomen();
            //databank.VoegBoomToe(bomen, bos1);

            ////tabel monkeyRecords opvullen
            //databank.VerwijderAlleApenGegevens();
            //databank.VoegAapGegevens(apen, bomen, bos1);

            ////tabel logs opvullen
            //databank.VerwijderAlleLogGegevens();
            //databank.VoegLogGegevens(apen, bos1);
            await Task.WhenAll(databank.VerwijderAlleBomen(),databank.VerwijderAlleApenGegevens(),databank.VerwijderAlleLogGegevens(),databank.VoegLogGegevens(apen, bos1), databank.VoegAapGegevens(apen, bomen, bos1), databank.VoegBoomToe(bomen, bos1));

        }
        private static async Task MaakTextBestandAan(List<Aap> aap, List<Boom> boom)
        {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\apen.txt";

            await using (StreamWriter sw = File.CreateText(path))
            {
                int meesteStappen = 0;
                foreach (Aap element in aap)
                {
                    if (meesteStappen < element.Boom_Lijst.Count)
                        meesteStappen = element.Boom_Lijst.Count;
                }

                for (int i = 0; i < meesteStappen; i++)
                {
                    foreach (Aap element in aap)
                    {
                        Console.Write(" L-START ");
                        if (element.Boom_Lijst.Count > i)
                            if (element.Boom_Lijst[i].X != -1)
                              await Task.Run(()=> sw.WriteLine(element.Naam + " is in tree " + element.Boom_Lijst[i].ID + " at (" + element.Boom_Lijst[i].X + "," + element.Boom_Lijst[i].Y + ")"));
                            else
                               await Task.Run(()=>sw.WriteLine(element.Naam + " is out the woods"));
                        Console.Write(" L-STOP ");
                    }
                }
            }
        }


        public static List<Boom> MaakBomenAan(int aantal, Bos bos1)
        {
            Random r = new Random();
            List<Boom> bomen = new List<Boom>();
            
            while (bomen.Count < aantal + 1)
            {
                Boom nieweBoom = new Boom(r.Next(bos1.XMin, bos1.YMax), r.Next(bos1.YMin, bos1.YMax), bomen.Count);
                bool isOK = true;
                foreach (Boom element in bomen)
                    if (nieweBoom.X == element.X && nieweBoom.Y == element.Y)
                        isOK = false;

                if (isOK)
                bomen.Add(nieweBoom);
                
            }
            return bomen;
        }

        public static List<Aap> MaakApenAan()
        {
            List<Aap> aapjesLijst = new List<Aap>();

            aapjesLijst.Add(new Aap(aapjesLijst.Count, "Ahmed"));
            aapjesLijst.Add(new Aap(aapjesLijst.Count, "Enias"));
            aapjesLijst.Add(new Aap(aapjesLijst.Count, "Robin"));
            aapjesLijst.Add(new Aap(aapjesLijst.Count, "David"));
            aapjesLijst.Add(new Aap(aapjesLijst.Count, "Yusuf"));
            return aapjesLijst;
        }

        public static void PlaatsAapOpEenBoom(List<Aap> apen, List<Boom> bomen)
        {
            foreach (Aap element in apen)
            {
                Random r = new Random();
                int IDBoom = r.Next(0, bomen.Count);
                bool isOK = true;
                foreach (Aap el in apen)
                {
                    if (el.Boom_Lijst == null)
                        if (IDBoom == el.Boom_Lijst[0].ID)
                            isOK = false;
                }
                if (isOK)
                    element.Boom_Lijst.Add(bomen[IDBoom]);
            }
            

        }
        public static void Spring(List<Aap> apen, List<Boom> bomen, Bos bos)
        {
            int aantalUitHetBos = 0;
            while (aantalUitHetBos < 5)
            {
                foreach (Aap element in apen)
                {
                    int huidigeX = element.Boom_Lijst[element.Boom_Lijst.Count-1].X;
                    int huidigeY = element.Boom_Lijst[element.Boom_Lijst.Count-1].Y;
                    if (huidigeX != -1)
                    {
                        double tijdelijkeAfstand = 2000000000;
                        Boom gekozenBoom = null;

                        //checken welke boom het dichtste is
                        foreach (Boom ele in bomen)
                        {
                            //check voor niet dezelfde boom te pakken
                            if (ele != element.Boom_Lijst[element.Boom_Lijst.Count-1])
                            {

                                //checken voor niet een van de vorige bomen te pakken
                                bool isOK = true;
                                foreach (Boom elementEigenAap in element.Boom_Lijst)
                                {
                                    if (elementEigenAap.ID == ele.ID)
                                        isOK = false;
                                }
                                foreach (Aap c in apen)
                                    foreach (Boom d in c.Boom_Lijst)
                                        if (d == ele)
                                            isOK = false;
                                if (isOK)
                                {
                                    double werkelijkeAfstand = Math.Sqrt(Math.Pow(huidigeX - ele.X, 2) + Math.Pow(huidigeY - ele.Y, 2));
                                    if (werkelijkeAfstand < tijdelijkeAfstand)
                                    {
                                        tijdelijkeAfstand = werkelijkeAfstand;
                                        gekozenBoom = ele;
                                    }
                                }
                            }
                        }

                        //de afstand van de dichtste muur berekenen
                        double distanceToBorder = (new List<double>() { bos.YMax - huidigeY, bos.XMax - huidigeX, huidigeY - bos.YMin, huidigeX - bos.XMin }).Min();

                        //de afstand tussen de dichtste muur en de afstand tussen de dichtste boom vergelijken
                        if (tijdelijkeAfstand < distanceToBorder)
                        {
                            element.Boom_Lijst.Add(gekozenBoom);
                            //Console.WriteLine("Aap " + element.Naam + " is op boom: " + gekozenBoom.ID);
                        }
                        else
                        {
                            //aap gaat weg
                            element.Boom_Lijst.Add(new Boom(-1, -1, -1));
                            aantalUitHetBos++;
                            //Console.WriteLine("Aap " + element.Naam + " is uit het bos met " + element.Boom_Lijst.Count+" stappen");
                        }
                    }

                }
            }
        }
    }
}
