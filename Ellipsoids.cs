using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carto
{
    public class Ellipsoid
    {
        public string Name;
        public string Designator;
        public double A;
        public double B;
        public double F;
        public double IF;
        public double EC;
        public double EC2;

        public Ellipsoid(string Name, string TwoLetter, double A, double B, double F,
                         double IF, double EC, double EC2)
        {
            this.Designator = TwoLetter;
            this.Name = Name;
            this.A = A;
            this.B = B;
            this.F = F;
            this.IF = IF;
            this.EC = EC;
            this.EC2 = EC2;
        }
    }


    public class Ellipsoids
    {
        static Ellipsoid[] Ellips = new Ellipsoid[] {
            new Ellipsoid("WGS84","E0",6378137,6356752.31424518,0.00335281066474748,298.257223563,0.0818191908426215,0.00669437999014132),
            new Ellipsoid("Airy 1830","E1",6377563.396,6356256.90890985,0.00334085069283898,299.32496,0.0816733745006665,0.00667054010232612),
            new Ellipsoid("Airy Modified 1849","E2",6377340.189,6356034.44761111,0.00334085069283898,299.32496,0.0816733745006665,0.00667054010232612),
            new Ellipsoid("Australian National Spheroid","E3",6378160,6356774.71919531,0.00335289186923722,298.25,0.0818201799960599,0.00669454185458764),
            new Ellipsoid("Bessel 1841","E4",6377397.155,6356078.96345955,0.00334277308160762,299.1528218,0.0816968299956621,0.00667437203134011),
            new Ellipsoid("Bessel Modified","E5",6377492.018,6356173.5087127,0.00334277318217481,299.1528128,0.0816968312225275,0.00667437223180214),
            new Ellipsoid("Bessel Namibia","E6",6377483.865,6356165.38296633,0.00334277318217481,299.1528128,0.0816968312225275,0.00667437223180214),
            new Ellipsoid("Clarke 1858","E7",6378293.63683822,6356617.97933774,0.00339834738483757,294.260676369257,0.0823719976978045,0.00678514600472712),
            new Ellipsoid("Clarke 1866","E8",6378206.4,6356583.8,0.00339007530392876,294.978698213901,0.082271854223004,0.00676865799729122),
            new Ellipsoid("Clarke 1866 Michigan","E9",6378450.04723951,6356826.62026467,0.00339007545950709,294.978684676652,0.0822718561076206,0.00676865830739303),
            new Ellipsoid("Clarke 1880 (Benoit)","E10",6378300.79,6356566.43,0.0034075470435756,293.466234570509,0.0824832268421709,0.00680348271029702),
            new Ellipsoid("Clarke 1880 (IGN)","E11",6378249.2,6356515,0.00340754952001565,293.466021293627,0.082483256763418,0.00680348764629994),
            new Ellipsoid("Clarke 1880 (RGS)","E12",6378249.145,6356514.86954978,0.00340756137869933,293.465,0.082483400044185,0.00680351128284906),
            new Ellipsoid("Clarke 1880 (Arc)","E13",6378249.145,6356514.96639875,0.00340754619444173,293.4663077,0.0824832165826248,0.0068034810178162),
            new Ellipsoid("Clarke 1880 (SGA 1922)","E14",6378249.2,6356514.99694178,0.00340754999949228,293.46598,0.0824832625566273,0.00680348860198551),
            new Ellipsoid("Everest 1830 (1937 Adjustment)","E15",6377276.345,6356075.41314024,0.00332444929666289,300.8017,0.0814729809826527,0.00663784663019969),
            new Ellipsoid("Everest 1830 (1967 Definition)","E16",6377298.556,6356097.5503009,0.00332444929666289,300.8017,0.0814729809826527,0.00663784663019969),
            new Ellipsoid("Everest 1830 (1975 Definition)","E17",6377301.243,6356100.231,0.00332444888396499,300.801737341596,0.0814729759340353,0.00663784580754789),
            new Ellipsoid("Everest 1830 Modified","E18",6377304.063,6356103.03899315,0.00332444929666289,300.8017,0.0814729809826527,0.00663784663019969),
            new Ellipsoid("GRS 1980","E19",6378137,6356752.31414028,0.00335281068119356,298.2572221,0.0818191910429527,0.0066943800229232),
            new Ellipsoid("Helmert 1906","E20",6378200,6356818.16962789,0.00335232986925914,298.3,0.0818133340169312,0.00669342162296594),
            new Ellipsoid("Indonesian National Spheroid","E21",6378160,6356774.50408554,0.00335292559522812,298.247,0.08182059080946,0.0066946090804091),
            new Ellipsoid("International 1924","E22",6378388,6356911.94612795,0.00336700336700337,297,0.0819918899790298,0.00672267002233332),
            new Ellipsoid("International 1967","E23",6378160,6356774.51608878,0.00335292371329995,298.2471674,0.0818205678859093,0.00669460532917269),
            new Ellipsoid("Krassowsky 1940","E24",6378245,6356863.01877305,0.00335232986925914,298.3,0.0818133340169312,0.00669342162296594),
            new Ellipsoid("NWL 9D","E25",6378145,6356759.76948868,0.00335289186923722,298.25,0.0818201799960599,0.00669454185458764),
            new Ellipsoid("NWL 10D","E26",6378135,6356750.52001609,0.00335277945416751,298.26,0.0818188106627487,0.00669431777826672),
            new Ellipsoid("Plessis 1817","E27",6376523,6355862.93325557,0.00324002073613271,308.64,0.0804334739887247,0.00646954373789485),
            new Ellipsoid("Struve 1860","E28",6378298.3,6356657.14266956,0.00339293590744071,294.73,0.082306499140769,0.00677435980080942),
            new Ellipsoid("War Office","E29",6378300.583,6356752.27021959,0.00337837837837838,296,0.0821300390617785,0.00674534331628926),
            new Ellipsoid("WGS84","E30",6378137,6356752.31424783,0.00335281066433155,298.2572236,0.081819190837555,0.00669437998931225),
            new Ellipsoid("GEM 10C","E31",6378137,6356752.31424783,0.00335281066433155,298.2572236,0.081819190837555,0.00669437998931225),
            new Ellipsoid("OSU86F","E32",6378136.2,6356751.51693008,0.00335281066433155,298.2572236,0.081819190837555,0.00669437998931225),
            new Ellipsoid("OSU91A","E33",6378136.3,6356751.6165948,0.00335281066433155,298.2572236,0.081819190837555,0.00669437998931225),
            new Ellipsoid("Clarke 1880","E34",6378249.13643753,6356514.95786218,0.00340754619495687,293.466307655635,0.082483216588849,0.00680348101884297),
            new Ellipsoid("GRS 1967","E35",6378160,6356774.51609071,0.00335292371299641,298.247167427,0.081820567882212,0.00669460532856765),
            new Ellipsoid("Clarke Modified 1880","E36",6378249.145,6356514.86954978,0.00340756137869933,293.465,0.082483400044185,0.00680351128284906),
            new Ellipsoid("International 1979","E37",6378137,6356752.31414036,0.00335281068118232,298.257222101,0.0818191910428158,0.00669438002290079),
            new Ellipsoid("Hayford 1909","E38",6378388,6356911.94612795,0.00336700336700337,297,0.0819918899790298,0.00672267002233332),
            new Ellipsoid("Average Terrestrial System 1977","E39",6378135,6356750.30492159,0.00335281317789691,298.257,0.0818192214555232,0.00669438499958795),
            new Ellipsoid("Everest (1830 Definition)","E40",6377301.24346728,6356100.23063417,0.00332444901435858,300.801725543365,0.0814729775291666,0.00663784606746808),
            new Ellipsoid("WGS 72","E41",6378135,6356750.52001609,0.00335277945416751,298.26,0.0818188106627487,0.00669431777826672) };

        public static bool Exists(string Name)
        {
            int i = Find(Name);
            if (i == -1)
                return false;
            else
                return true;
        }

        public static int Count
        {
            get
            {
                return Ellips.Length;
            }

        }

        private static int Find(string Name)
        {
            Name = Name.Trim().ToUpper();
            for (int i = 0; i < Ellips.Length; i++)
            {
                Ellipsoid T = Ellips[i];
                T.Name = T.Name.ToUpper();
                T.Designator = T.Designator.ToUpper();
                if (Name == T.Name) return i;
                if (Name == T.Designator) return i;
            }
            return -1;
        }

        private static string[] Search(string Partial)
        {
            List<string> L = new List<string>();
            Partial = Partial.Trim().ToUpper();
            for (int i = 0; i < Ellips.Length; i++)
            {
                Ellipsoid T = Ellips[i];
                T.Name = T.Name.ToUpper();
                T.Designator = T.Designator.ToUpper();
                if (T.Name.Contains(Partial))
                    L.Add(T.Name);
                else if (T.Designator.Contains(Partial))
                {
                    L.Add(T.Name);
                }
            }

            string[] Finds = L.ToArray();
            return Finds;
        }

        public static Ellipsoid Get(string Name)
        {
            int i = Find(Name);
            if (i == -1) throw new Exception("Ellipsoid with designator " + Name + " was not found in the legal set.");
            return Ellips[i];
        }

        public static string[] ListNames()
        {
            string[] T = new string[Ellips.Length];
            for (int i = 0; i < Ellips.Length; i++)
            {
                T[i] = Ellips[i].Name;
            }
            return T;
        }

        public static string[] ListDesignators()
        {
            string[] T = new string[Ellips.Length];
            for (int i = 0; i < Ellips.Length; i++)
            {
                T[i] = Ellips[i].Designator;
            }
            return T;
        }

        public static double CalcFlattening(double A, double B)
        {
            double F = 1.0 - (B / A);
            return F;
        }

        public static double CalcE2(double A, double B)
        {
            double F = CalcFlattening(A, B);
            double Ans = 2 * F - F * F;
            return Ans;
        }

    }
}