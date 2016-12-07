using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carto
{
    /// <summary>
    /// The data record used by DatumTranformInfo
    /// </summary>
    public class DatumShiftRec 
    {
        public readonly bool Simple;
        public readonly string Name;
        public readonly string Location;
        public readonly string Ellipsoid; //May not be there
        public readonly double DeltaX;
        public readonly double DeltaY;
        public readonly double DeltaZ;
        public readonly string[] Places;
        public readonly string LongName;

        public DatumShiftRec(string Name, string Location, string Ellipsoid, double DeltaX, double DeltaY, double DeltaZ, bool Simple, params string[] Places)
        {
            this.Simple = Simple;
            this.Name = Name;
            this.Location = Location;
            this.Ellipsoid = Ellipsoid;
            this.DeltaX = DeltaX;
            this.DeltaY = DeltaY;
            this.DeltaZ = DeltaZ;
            if (Places != null)
            {
                this.Places = new string[Places.Length];
                Array.Copy(Places, this.Places, Places.Length);
            }
            else
                this.Places = null;

            if (!this.Simple)
                this.LongName = this.Name + "." + this.Location;
            else
                this.LongName = this.Name + "." + "Nonspecific";

        }
    }

    //Great reference to check datums
    //http://publib.boulder.ibm.com/infocenter/idshelp/v10/index.jsp?topic=/com.ibm.geod.doc/geod308.htm
    public static class DatumShiftInfo
    {
        static DatumShiftRec[] DatumShift = new DatumShiftRec[] {
                new DatumShiftRec("Adindan","Burkina Faso","Clarke 1880",-118,-14,218,false,null),
                new DatumShiftRec("Adindan","Cameroon","Clarke 1880",-134,-2,210,false,null),
                new DatumShiftRec("Adindan","Ethiopia","Clarke 1880",-165,-11,206,false,null),
                new DatumShiftRec("Adindan","Mali","Clarke 1880",-123,-20,220,false,null),
                new DatumShiftRec("Adindan","Mean","Clarke 1880",-162,-12,206,false,"Mean for Ethiopia","Mali","Senegal","Sudan"),
                new DatumShiftRec("Adindan","Nonspecific","Clarke 1880",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("Adindan","Senegal","Clarke 1880",-128,-18,224,false,null),
                new DatumShiftRec("Adindan","Sudan","Clarke 1880",-161,-14,205,false,null),
                new DatumShiftRec("Afgooye","Nonspecific","Krassovsky 1940",-43,-163,45,false,"Somalia"),
                new DatumShiftRec("Ain el Abd 1970","Bahrain","International",-150,-250,-1,false,null),
                new DatumShiftRec("Ain el Abd 1970","Nonspecific","International",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("Ain el Abd 1970","Saudi Arabia","International",-143,-236,7,false,null),
                new DatumShiftRec("American Samoa 1962","Nonspecific","Clarke 1866",-115,118,426,false,"American Samoa Islands"),
                new DatumShiftRec("Anna 1 Astro 1965","Nonspecific","Australian National",-491,-22,435,false,"Cocos Islands"),
                new DatumShiftRec("Antigua Island Astro 1943","Nonspecific","Clarke 1880",-270,13,62,false,"Antigua (Leeward Islands)"),
                new DatumShiftRec("Arc 1950","Botswana","Clarke 1880",-138,-105,-289,false,null),
                new DatumShiftRec("Arc 1950","Burundi","Clarke 1880",-153,-5,-292,false,null),
                new DatumShiftRec("Arc 1950","Lesotho","Clarke 1880",-125,-108,-295,false,null),
                new DatumShiftRec("Arc 1950","Malawi","Clarke 1880",-161,-73,-317,false,null),
                new DatumShiftRec("Arc 1950","Mean","Clarke 1880",-143,-90,-294,false,"Mean for Botswana","Lesotho","Malawi","Swaziland","Zaire","Zambia","Zimbabwe"),
                new DatumShiftRec("Arc 1950","Nonspecific","Clarke 1880",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("Arc 1950","Swaziland","Clarke 1880",-134,-105,-295,false,null),
                new DatumShiftRec("Arc 1950","Zaire","Clarke 1880",-169,-19,-278,false,null),
                new DatumShiftRec("Arc 1950","Zambia","Clarke 1880",-147,-74,-283,false,null),
                new DatumShiftRec("Arc 1950","Zimbabwe","Clarke 1880",-142,-96,-293,false,null),
                new DatumShiftRec("Arc 1960","Mean","Clarke 1880",-160,-6,-302,false,"Mean for Kenya","Tanzania"),
                new DatumShiftRec("Arc 1960","Nonspecific","Clarke 1880",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("Ascension Island 1958","Nonspecific","International",-205,107,53,false,"Ascension Island"),
                new DatumShiftRec("Astro Beacon E 1945","Nonspecific","International",145,75,-272,false,"Iwo Jima"),
                new DatumShiftRec("Astro DOS 71/4","Nonspecific","International",-320,550,-494,false,"St Helena Island"),
                new DatumShiftRec("Astro Tern Island (FRIG) 1961","Nonspecific","International",114,-116,-333,false,"Tern Island"),
                new DatumShiftRec("Astronomical Station 1952","Nonspecific","International",124,-234,-25,false,"Marcus Island"),
                new DatumShiftRec("Australian Geodetic 1966","Nonspecific","Australian National",-133,-48,148,false,"Australia","Tasmania"),
                new DatumShiftRec("Australian Geodetic 1984","Nonspecific","Australian National",-134,-48,149,false,"Australia","Tasmania"),
                new DatumShiftRec("Ayabelle Lighthouse","Nonspecific","Clarke 1880",-79,-129,145,false,"Djibouti"),
                new DatumShiftRec("Bellevue (IGN)","Nonspecific","International",-127,-769,472,false,"Efate and Erromango Islands"),
                new DatumShiftRec("Bermuda 1957","Nonspecific","Clarke 1866",-73,213,296,false,"Bermuda"),
                new DatumShiftRec("Bissau","Nonspecific","International",-173,253,27,false,"Guinea-Bissau"),
                new DatumShiftRec("Bogota Observatory","Nonspecific","International",307,304,-318,false,"Colombia"),
                new DatumShiftRec("Bukit Rimpah","Nonspecific","Bessel 1841",-384,664,-48,false,"Indonesia (Bangka and Belitung Ids)"),
                new DatumShiftRec("Camp Area Astro","Nonspecific","International",-104,-129,239,false,"Antarctica (McMurdo Camp Area)"),
                new DatumShiftRec("Campo Inchauspe","Nonspecific","International",-148,136,90,false,"Argentina"),
                new DatumShiftRec("Canton Astro 1966","Nonspecific","International",298,-304,-375,false,"Phoenix Islands"),
                new DatumShiftRec("Cape Canaveral","Nonspecific","Clarke 1866",-2,151,181,false,"Bahamas","Florida"),
                new DatumShiftRec("Cape","Nonspecific","Clarke 1880",-136,-108,-292,false,"South Africa"),
                new DatumShiftRec("Carthage","Nonspecific","Clarke 1880",-263,6,431,false,"Tunisia"),
                new DatumShiftRec("Chatham Island Astro 1971","Nonspecific","International",175,-38,113,false,"New Zealand (Chatham Island)"),
                new DatumShiftRec("Chua Astro","Nonspecific","International",-134,229,-29,false,"Paraguay"),
                new DatumShiftRec("Corrego Alegre","Nonspecific","International",-206,172,-6,false,"Brazil"),
                new DatumShiftRec("Dabola","Nonspecific","Clarke 1880",-83,37,124,false,"Guinea"),
                new DatumShiftRec("Deception Island","Nonspecific","Clarke 1880",260,12,-147,false,"Deception Island","Antarctia"),
                new DatumShiftRec("Djakarta (Batavia)","Nonspecific","Bessel 1841",-377,681,-50,false,"Indonesia (Sumatra)"),
                new DatumShiftRec("DOS 1968","Nonspecific","International",230,-199,-752,false,"New Georgia Islands (Gizo Island)"),
                new DatumShiftRec("Easter Island 1967","Nonspecific","International",211,147,111,false,"Easter Island"),
                new DatumShiftRec("European 1950 (ED 50)","Cyprus","International",-104,-101,-140,false,"Cyprus"),
                new DatumShiftRec("European 1950 (ED 50)","Egypt","International",-130,-117,-151,false,"Egypt"),
                new DatumShiftRec("European 1950 (ED 50)","England","International",-86,-96,-120,false,"England","Ireland","Scotland","Shetland Islands"),
                new DatumShiftRec("European 1950 (ED 50)","England Channel","International",-86,-96,-120,false,"England","Channel Islands","Scotland","Shetland Islands"),
                new DatumShiftRec("European 1950 (ED 50)","Finland Norway","International",-87,-95,-120,false,"Finland","Norway"),
                new DatumShiftRec("European 1950 (ED 50)","Greece","International",-84,-95,-130,false,null),
                new DatumShiftRec("European 1950 (ED 50)","Iran","International",-117,-132,-164,false,null),
                new DatumShiftRec("European 1950 (ED 50)","Italy Sardinia","International",-97,-103,-120,false,"Italy (Sardinia)"),
                new DatumShiftRec("European 1950 (ED 50)","Italy Sicily","International",-97,-88,-135,false,"Italy (Sicily)"),
                new DatumShiftRec("European 1950 (ED 50)","Malta","International",-107,-88,-149,false,null),
                new DatumShiftRec("European 1950 (ED 50)","Mean Europe","International",-87,-98,-121,false,"Mean for Austria","Belgium","Denmark","Finland","France","W Germany","Gibralter","Greece", "Italy","Luxembourg","Netherlands","Norway","Portugal","Spain","Sweden","Switzerland"),
                new DatumShiftRec("European 1950 (ED 50)","Middle East","International",-103,-106,-141,false,"Mean for Iraq","Israel","Jordan","Lebanon","Kuwait","Saudi Arabia","Syria"),
                new DatumShiftRec("European 1950 (ED 50)","Nonspecific","International",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("European 1950 (ED 50)","North West Europe","International",-87,-96,-120,false,"Mean for Austria","Denmark","France","W Germany","Netherlands","Switzerland"),
                new DatumShiftRec("European 1950 (ED 50)","Portugal Spain","International",-84,-107,-120,false,"Portugal","Spain"),
                new DatumShiftRec("European 1950 (ED 50)","Tunisia","International",-112,-77,-145,false,"Tunisia"),
                new DatumShiftRec("European 1979 (ED 79)","Mean","International",-86,-98,-119,false,"Mean for Austria","Finland","Netherlands","Norway","Spain","Sweden","Switzerland"),
                new DatumShiftRec("European 1979 (ED 79)","Nonspecific","International",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("Fort Thomas 1955","Nonspecific","Clarke 1880",-7,215,225,false,"Nevis","St. Kitts (Leeward Islands)"),
                new DatumShiftRec("Gan 1970","Nonspecific","International",-133,-321,50,false,"Republic of Maldives"),
                new DatumShiftRec("Geodetic DatumShiftRec 1949","Nonspecific","International",84,-22,209,false,"New Zealand"),
                new DatumShiftRec("Graciosa Base SW 1948","Nonspecific","International",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("Guam 1963","Nonspecific","Clarke 1866",-100,-248,259,false,"Guam"),
                new DatumShiftRec("Gunung Segara","Nonspecific","Bessel 1841",-403,684,41,false,"Indonesia (Kalimantan)"),
                new DatumShiftRec("GUX 1 Astro","Nonspecific","International",252,-209,-751,false,"Guadalcanal Island"),
                new DatumShiftRec("Herat North","Nonspecific","International",-333,-222,114,false,"Afghanistan"),
                new DatumShiftRec("Hjorsey 1955","Nonspecific","International",-73,46,-86,false,"Iceland"),
                new DatumShiftRec("Hong Kong 1963","Nonspecific","International",-156,-271,-189,false,"Hong Kong"),
                new DatumShiftRec("Hu-Tzu-Shan","Nonspecific","International",-637,-549,-203,false,"Taiwan"),
                new DatumShiftRec("Indian (Bangladesh)","Nonspecific","Everest (India 1830)",282,726,254,false,"Bangladesh"),
                new DatumShiftRec("Indian (India and Nepal)","Nonspecific","Everest (India 1956)",295,736,257,false,"India","Nepal"),
                new DatumShiftRec("Indian (Pakistan)","Nonspecific","Everest (Pakistan)",283,682,231,false,"Pakistan"),
                new DatumShiftRec("Indian 1954","Nonspecific","Everest (India 1830)",217,823,299,false,"Thailand"),
                new DatumShiftRec("Indian 1960","Nonspecific","Everest (India 1830)",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("Indian 1960","Vietnam Cons Son","Everest (India 1830)",182,915,344,false,"Vietnam (Con Son Island)"),
                new DatumShiftRec("Indian 1960","Vietnam Near 16N","Everest (India 1830)",198,881,317,false,"Vietnam (Near 16?N)"),
                new DatumShiftRec("Indian 1975","Nonspecific","Everest (India 1830)",209,818,290,false,"Thailand"),
                new DatumShiftRec("Indonesian 1974","Nonspecific","Indonesian 1974",-24,-15,5,false,"Indonesia"),
                new DatumShiftRec("Ireland 1965","Nonspecific","Modified Airy",506,-122,611,false,"Ireland"),
                new DatumShiftRec("ISTS 061 Astro 1968","Nonspecific","International",-794,119,-298,false,"South Georgia Islands"),
                new DatumShiftRec("ISTS 073 Astro 1969","Nonspecific","International",208,-435,-229,false,"Diego Garcia"),
                new DatumShiftRec("Johnston Island 1961","Nonspecific","International",189,-79,-202,false,"Johnston Island"),
                new DatumShiftRec("Kandawala","Nonspecific","Everest (India 1830)",-97,787,86,false,"Sri Lanka"),
                new DatumShiftRec("Kerguelen Island 1949","Nonspecific","International",145,-187,103,false,"Kerguelen Island"),
                new DatumShiftRec("Kertau 1948","Nonspecific","Everest (Malay. and Singapore 1948)",-11,851,5,false,"West Malaysia and Singapore"),
                new DatumShiftRec("Kusaie Astro 1951","Nonspecific","International",647,1777,-1124,false,"Caroline Islands"),
                new DatumShiftRec("L. C. 5 Astro 1961","Nonspecific","Clarke 1866",42,124,147,false,"Cayman Brac Island"),
                new DatumShiftRec("Leigon","Nonspecific","Clarke 1880",-130,29,364,false,"Ghana"),
                new DatumShiftRec("Liberia 1964","Nonspecific","Clarke 1880",-90,40,88,false,"Liberia"),
                new DatumShiftRec("Luzon","Nonspecific","Clarke 1866",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("Luzon","Philippines","Clarke 1866",-133,-77,-51,false,"Philippines (Excluding Mindanao)"),
                new DatumShiftRec("Luzon","Philippines Mindanao","Clarke 1866",-133,-79,-72,false,"Philippines (Mindanao)"),
                new DatumShiftRec("Mahe 1971","Nonspecific","Clarke 1880",41,-220,-134,false,"Mahe Island"),
                new DatumShiftRec("Massawa","Nonspecific","Bessel 1841",639,405,60,false,"Ethiopia (Eritrea)"),
                new DatumShiftRec("Merchich","Nonspecific","Clarke 1880",31,146,47,false,"Morocco"),
                new DatumShiftRec("Midway Astro 1961","Nonspecific","International",912,-58,1227,false,"Midway Islands"),
                new DatumShiftRec("Minna","Cameroon","Clarke 1880",-81,-84,115,false,"Cameroon"),
                new DatumShiftRec("Minna","Nigeria","Clarke 1880",-92,-93,122,false,"Nigeria"),
                new DatumShiftRec("Minna","Nonspecific","Clarke 1880",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("Montserrat Island Astro 1958","Nonspecific","Clarke 1880",174,359,365,false,"Montserrat (Leeward Islands)"),
                new DatumShiftRec("M'Poraloko","Nonspecific","Clarke 1880",-74,-130,42,false,"Gabon"),
                new DatumShiftRec("Nahrwan","Nonspecific","Clarke 1880",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("Nahrwan","Oman","Clarke 1880",-247,-148,369,false,"Oman (Masirah Island)"),
                new DatumShiftRec("Nahrwan","Saudi Arabia","Clarke 1880",-243,-192,477,false,null),
                new DatumShiftRec("Nahrwan","United Arab Emirates","Clarke 1880",-249,-156,381,false,null),
                new DatumShiftRec("Naparima BWI","Nonspecific","International",-10,375,165,false,"Trinidad and Tobago"),
                new DatumShiftRec("North American 1927 (NAD 27)","Alaska","Clarke 1866",-5,135,172,false,"Alaska (Excluding Aleutian Ids)"),
                new DatumShiftRec("North American 1927 (NAD 27)","Alaska Aleutian East","Clarke 1866",-2,152,149,false,"Alaska (Aleutian Ids East of 180?W)"),
                new DatumShiftRec("North American 1927 (NAD 27)","Alaska Aleutian West","Clarke 1866",2,204,105,false,"Alaska (Aleutian Ids West of 180?W)"),
                new DatumShiftRec("North American 1927 (NAD 27)","Bahamas","Clarke 1866",-4,154,178,false,"Bahamas (Except San Salvador Id)"),
                new DatumShiftRec("North American 1927 (NAD 27)","Bahamas San Salvador","Clarke 1866",1,140,165,false,"Bahamas (San Salvador Island)"),
                new DatumShiftRec("North American 1927 (NAD 27)","Canada","Clarke 1866",-10,158,187,false,"Mean for Canada"),
                new DatumShiftRec("North American 1927 (NAD 27)","Canada East","Clarke 1866",-22,160,190,false,"Canada (New Brunswick","Newfoundland","Nova Scotia","Quebec)"),
                new DatumShiftRec("North American 1927 (NAD 27)","Canada Middle","Clarke 1866",-9,157,184,false,"Canada (Manitoba","Ontario)"),
                new DatumShiftRec("North American 1927 (NAD 27)","Canada North","Clarke 1866",4,159,188,false,"Canada (Northwest Territories","Saskatchewan)"),
                new DatumShiftRec("North American 1927 (NAD 27)","Canada West","Clarke 1866",-7,162,188,false,"Canada (Alberta","British Columbia)"),
                new DatumShiftRec("North American 1927 (NAD 27)","Canada Yukon","Clarke 1866",-7,139,181,false,"Canada (Yukon)"),
                new DatumShiftRec("North American 1927 (NAD 27)","Canal Zone","Clarke 1866",0,125,201,false,null),
                new DatumShiftRec("North American 1927 (NAD 27)","Caribean","Clarke 1866",-3,142,183,false,"Mean for Antigua","Barbados","Barbuda","Caicos Islands","Cuba","Dominican Republic","Grand Cayman", "Jamaica","Turks Islands"),
                new DatumShiftRec("North American 1927 (NAD 27)","Central America","Clarke 1866",0,125,194,false,"Mean for Belize","Costa Rica","El Salvador","Guatemala","Honduras","Nicaragua"),
                new DatumShiftRec("North American 1927 (NAD 27)","Conus","Clarke 1866",-8,160,176,false,"Mean for CONUS"),
                new DatumShiftRec("North American 1927 (NAD 27)","Conus East","Clarke 1866",-9,161,179,false,"Mean for CONUS (East of Mississippi River Including Louisiana","Missouri","Minnesota)"),
                new DatumShiftRec("North American 1927 (NAD 27)","Conus West","Clarke 1866",-8,159,175,false,"Mean for CONUS (West of Mississippi River Excluding Louisiana","Minnesota","Missouri)"),
                new DatumShiftRec("North American 1927 (NAD 27)","Cuba","Clarke 1866",-9,152,178,false,null),
                new DatumShiftRec("North American 1927 (NAD 27)","Greenland","Clarke 1866",11,114,195,false,"Greenland (Hayes Peninsula)"),
                new DatumShiftRec("North American 1927 (NAD 27)","Mexico","Clarke 1866",-12,130,190,false,null),
                new DatumShiftRec("North American 1927 (NAD 27)","Nonspecific","Clarke 1866",-8,160,176,false,"Mean for CONUS"),
                new DatumShiftRec("North American 1983 (NAD 83)","Alaska","GRS 80",0,0,0,false,"Alaska (Excluding Aleutian Ids)"),
                new DatumShiftRec("North American 1983 (NAD 83)","Alaska Aleutian","GRS 80",-2,0,4,false,"Aleutian Ids"),
                new DatumShiftRec("North American 1983 (NAD 83)","Canada","GRS 80",0,0,0,false,"Canada"),
                new DatumShiftRec("North American 1983 (NAD 83)","Central America","GRS 80",0,0,0,false,null),
                new DatumShiftRec("North American 1983 (NAD 83)","Conus","GRS 80",0,0,0,false,null),
                new DatumShiftRec("North American 1983 (NAD 83)","Hawaii","GRS 80",1,1,-1,false,null),
                new DatumShiftRec("North American 1983 (NAD 83)","Mexico","GRS 80",0,0,0,false,null),
                new DatumShiftRec("North American 1983 (NAD 83)","Nonspecific","GRS 80",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("North Sahara 1959","Nonspecific","Clarke 1880",-186,-93,310,false,"Algeria"),
                new DatumShiftRec("NTF (Nouvelle Triangulation de France)","Nonspecific","Clarke 1880 (IGN)",-168,-60,320,false,"France (incl. Corsica)"),
                new DatumShiftRec("Observatorio Meteorologico 1939","Nonspecific","International",-425,-169,81,false,"Azores (Corvo and Flores Islands)"),
                new DatumShiftRec("Old Egyptian 1907","Nonspecific","Helmert 1906",-130,110,-13,false,"Egypt"),
                new DatumShiftRec("Old Hawaiian","Hawaii","Clarke 1866",89,-279,-183,false,null),
                new DatumShiftRec("Old Hawaiian","Kauai","Clarke 1866",45,-290,-172,false,null),
                new DatumShiftRec("Old Hawaiian","Maui","Clarke 1866",65,-290,-190,false,null),
                new DatumShiftRec("Old Hawaiian","Mean","Clarke 1866",61,-285,-181,false,"Mean for Hawaii","Kauai","Maui","Oahu"),
                new DatumShiftRec("Old Hawaiian","Nonspecific","Clarke 1866",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("Old Hawaiian","Oahu","Clarke 1866",58,-283,-182,false,null),
                new DatumShiftRec("Oman","Nonspecific","Clarke 1880",-346,-1,224,false,"Oman"),
                new DatumShiftRec("Ordnance Survey Great Britain 1936","England","Airy 1830",371,-112,434,false,"England"),
                new DatumShiftRec("Ordnance Survey Great Britain 1936","England Wales","Airy 1830",371,-111,434,false,"England","Isle of Man","Wales"),
                new DatumShiftRec("Ordnance Survey Great Britain 1936","Mean","Airy 1830",375,-111,431,false,"Mean for England","Isle of Man","Scotland","Shetland Islands","Wales"),
                new DatumShiftRec("Ordnance Survey Great Britain 1936","Nonspecific","Airy 1830",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("Ordnance Survey Great Britain 1936","Scotland","Airy 1830",384,-111,425,false,"Scotland","Shetland Islands"),
                new DatumShiftRec("Ordnance Survey Great Britain 1936","Wales","Airy 1830",370,-108,434,false,null),
                new DatumShiftRec("Pico de las Nieves","Nonspecific","International",-307,-92,127,false,"Canary Islands"),
                new DatumShiftRec("Pitcairn Astro 1967","Nonspecific","International",185,165,42,false,"Pitcairn Island"),
                new DatumShiftRec("Point 58","Nonspecific","Clarke 1880",-106,-129,165,false,"Mean for Burkina Faso and Niger"),
                new DatumShiftRec("Pointe Noire 1948","Nonspecific","Clarke 1880",-148,51,-291,false,"Congo"),
                new DatumShiftRec("Porto Santo 1936","Nonspecific","International",-499,-249,314,false,"Porto Santo","Madeira Islands"),
                new DatumShiftRec("Provisional South American 1956","Bolivia","International",-270,188,-388,false,"Bolivia"),
                new DatumShiftRec("Provisional South American 1956","Chile North","International",-270,183,-390,false,"Chile (Northern","Near 19?S)"),
                new DatumShiftRec("Provisional South American 1956","Chile South","International",-305,243,-442,false,"Chile (Southern","Near 43?S)"),
                new DatumShiftRec("Provisional South American 1956","Colombia","International",-282,169,-371,false,null),
                new DatumShiftRec("Provisional South American 1956","Ecuador","International",-278,171,-367,false,null),
                new DatumShiftRec("Provisional South American 1956","Guyana","International",-298,159,-369,false,null),
                new DatumShiftRec("Provisional South American 1956","Mean","International",-288,175,-376,false,"Mean for Bolivia","Chile","Colombia","Ecuador","Guyana","Peru","Venezuela"),
                new DatumShiftRec("Provisional South American 1956","Nonspecific","International",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("Provisional South American 1956","Peru","International",-279,175,-379,false,null),
                new DatumShiftRec("Provisional South American 1956","Venezuela","International",-295,173,-371,false,null),
                new DatumShiftRec("Provisional South Chilean 1963","Nonspecific","International",16,196,93,false,"Chile (Near 53?S) (Hito XVIII)"),
                new DatumShiftRec("Puerto Rico","Nonspecific","Clarke 1866",11,72,-101,false,"Puerto Rico","Virgin Islands"),
                new DatumShiftRec("Pulkovo 1942","Nonspecific","Krassovsky 1940",28,-130,-95,false,"Russia"),
                new DatumShiftRec("Qatar National","Nonspecific","International",-128,-283,22,false,"Qatar"),
                new DatumShiftRec("Qornoq","Nonspecific","International",164,138,-189,false,"Greenland (South)"),
                new DatumShiftRec("Reunion","Nonspecific","International",94,-948,-1262,false,"Mascarene Islands"),
                new DatumShiftRec("Rijks Driehoeksmeting","Nonspecific","Bessel 1841",-593,-26,-478,false,"Netherlands"),
                new DatumShiftRec("Rome 1940","Nonspecific","International",-225,-65,9,false,"Italy (Sardinia)"),
                new DatumShiftRec("S-42 (Pulkovo 1942)","Nonspecific","Krassovsky 1940",28,-121,-77,false,"Hungary"),
                new DatumShiftRec("Santo (DOS) 1965","Nonspecific","International",170,42,84,false,"Espirito Santo Island"),
                new DatumShiftRec("Sao Braz","Nonspecific","International",-203,141,53,false,"Azores (Sao Miguel","Santa Maria Ids)"),
                new DatumShiftRec("Sapper Hill 1943","Nonspecific","International",-355,21,72,false,"East Falkland Island"),
                new DatumShiftRec("Schwarzeck","Nonspecific","Bessel 1841 (Namibia)",616,97,-251,false,"Namibia"),
                new DatumShiftRec("Selvagem Grande 1938","Nonspecific","International",-289,-124,60,false,"Salvage Islands"),
                new DatumShiftRec("S-JTSK","Nonspecific","Bessel 1841",589,76,480,false,"Czechoslavakia (Prior 1 JAN 1993)"),
                new DatumShiftRec("South American 1969","Argentina","South American 1969",-62,-1,-37,false,null),
                new DatumShiftRec("South American 1969","Bolivia","South American 1969",-61,2,-48,false,null),
                new DatumShiftRec("South American 1969","Brazil","South American 1969",-60,-2,-41,false,null),
                new DatumShiftRec("South American 1969","Chile","South American 1969",-75,-1,-44,false,null),
                new DatumShiftRec("South American 1969","Colombia","South American 1969",-44,6,-36,false,null),
                new DatumShiftRec("South American 1969","Ecuador","South American 1969",-48,3,-44,false,null),
                new DatumShiftRec("South American 1969","Ecuador Baltra","South American 1969",-47,26,-4,false,"Ecuador (Baltra","Galapagos)"),
                new DatumShiftRec("South American 1969","Guyana","South American 1969",-53,3,-47,false,null),
                new DatumShiftRec("South American 1969","Mean","South American 1969",-57,1,-41,false,"Mean for Argentina","Bolivia","Brazil","Chile","Colombia","Ecuador","Guyana","Paraguay","Peru","Trinidad and Tobago","Venezuela"),
                new DatumShiftRec("South American 1969","Nonspecific","South American 1969",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("South American 1969","Paraguay","South American 1969",-61,2,-33,false,null),
                new DatumShiftRec("South American 1969","Peru","South American 1969",-58,0,-44,false,null),
                new DatumShiftRec("South American 1969","Trinidad and Tobago","South American 1969",-45,12,-33,false,null),
                new DatumShiftRec("South American 1969","Venezuala","South American 1969",-45,8,-33,false,null),
                new DatumShiftRec("South Asia","Nonspecific","Modified Fischer 1960",7,-10,-26,false,"Singapore"),
                new DatumShiftRec("Tananarive Observatory 1925","Nonspecific","International",-189,-242,-91,false,"Madagascar"),
                new DatumShiftRec("Timbalai 1948","Nonspecific","Everest (Sabah Sarawak)",-679,669,-48,false,"Brunei","E. Malaysia (Sabah Sarawak)"),
                new DatumShiftRec("Tokyo","Japan","Bessel 1841",-148,507,685,false,null),
                new DatumShiftRec("Tokyo","Mean","Bessel 1841",-148,507,685,false,"Mean for Japan","South Korea","Okinawa"),
                new DatumShiftRec("Tokyo","Nonspecific","Bessel 1841",-999.999,-999.999,-999.999,true,null),
                new DatumShiftRec("Tokyo","Okinawa","Bessel 1841",-158,507,676,false,null),
                new DatumShiftRec("Tokyo","South Korea","Bessel 1841",-146,507,687,false,null),
                new DatumShiftRec("Tristan Astro 1968","Nonspecific","International",-632,438,-609,false,"Tristan da Cunha"),
                new DatumShiftRec("Viti Levu 1916","Nonspecific","Clarke 1880",51,391,-36,false,"Fiji (Viti Levu Island)"),
                new DatumShiftRec("Voirol 1960","Nonspecific","Clarke 1880",-123,-206,219,false,"Algeria"),
                new DatumShiftRec("Wake Island Astro 1952","Nonspecific","International",276,-57,149,false,"Wake Atoll"),
                new DatumShiftRec("Wake-Eniwetok 1960","Nonspecific","Hough 1960",102,52,-38,false,"Marshall Islands"),
                new DatumShiftRec("WGS 1984","Nonspecific","WGS84",0,0,0,false,"Global Definition"),
                new DatumShiftRec("Yacare","Nonspecific","International",-155,171,37,false,"Uruguay"),
                new DatumShiftRec("Zanderij","Nonspecific","International",-265,120,-358,false,"Suriname") };

        public static bool Exists(string Name)
        {
            int i = FindIndex(Name);
            if (i == -1)
                return false;
            else
                return true;
        }

        public static int Count
        {
            get
            {
                return DatumShift.Length;
            }

        }

        public static string PrettyPrint(DatumShiftRec D, bool AppendCRLF = false)
        {
            char CR = (char)13;
            char LF = (char)10;
            string CRLF = CR.ToString() + LF.ToString();
            StringBuilder S = new StringBuilder();
            S.Append("Name = " + D.Name.ToString() + CRLF);
            S.Append("Is a simple defintion = " + D.Simple.ToString().ToLower());
            S.Append("Primary location for defintion = " + D.Location.ToString() + CRLF);
            S.Append("Long name = " + D.LongName);
            S.Append("Ellipsoid used = " + D.Ellipsoid.ToString() + CRLF);
            S.Append("Delta X, Y, and Z = " + D.DeltaX.ToString() + ", " + D.DeltaY.ToString() + D.DeltaZ.ToString() + CRLF);
            S.Append("Places specified for use: " + CRLF);
            foreach (string Str in D.Places)
            {
                S.Append("    " + Str + CRLF);
            }
            if (AppendCRLF) S.Append(CRLF);
            return S.ToString();
        }

        //public static int Find(string LongName)
        //{
        //    string Name = LongName.Trim().ToUpper();
        //    for (int i = 0; i < DatumShift.Length; i++)
        //    {
        //        Datum T = DatumShift[i];
        //        string TName = T.LongName.ToUpper();
        //        if (Name == TName) return i;
        //    }
        //    return -1;
        //}

        public static string[] Search(string Partial)
        {
            List<string> L = new List<string>();
            Partial = Partial.Trim().ToUpper();
            for (int i = 0; i < DatumShift.Length; i++)
            {
                DatumShiftRec T = DatumShift[i];
                string Name = T.Name.ToUpper();
                string LongName = T.LongName.ToUpper();
                if (LongName.Contains(Partial))
                {

                    L.Add(T.LongName);
                }
                else if (T.Ellipsoid.Contains(Partial))
                {
                    L.Add(T.LongName);
                }
                else if ((T.Places != null) && (T.Places.Length > 0))
                {
                    for (int ii = 0; ii < T.Places.Length; ii++)
                    {
                        string PlaceName = T.Places[ii].ToUpper();
                        if (PlaceName.Contains(Partial)) L.Add(T.LongName);
                    }
                }
            }

            string[] Finds = L.ToArray();
            return Finds;
        }

        public static DatumShiftRec Get(string Name)
        {
            int i = FindIndex(Name);
            if (i == -1) throw new Exception("Datum with simple or long name " + Name + " was not found in the legal set.");
            return DatumShift[i];
        }


        public static int FindIndex(string Name)
        {
            Name = Name.Trim().ToUpper();
            for (int i = 0; i < DatumShift.Length; i++)
            {
                DatumShiftRec T = DatumShift[i];
                string TName = T.Name.ToUpper();
                string TLongName = T.LongName.ToUpper();
                if (Name == T.Name) return i;
                if (TLongName == Name) return i;
            }
            return -1;
        }


        public static DatumShiftRec Get(int i)
        {
            return DatumShift[i];
        }


        public static string[] ListLongNames()
        {
            string[] T = new string[DatumShift.Length];
            for (int i = 0; i < DatumShift.Length; i++)
            {
                T[i] = DatumShift[i].LongName;
            }
            return T;
        }


        public static string[] ListNames()
        {
            string[] T = new string[DatumShift.Length];
            for (int i = 0; i < DatumShift.Length; i++)
            {
                T[i] = DatumShift[i].Name;
            }
            return T;
        }
    }

    /// <summary>
    /// The operations to do NAD83 to NAD27 transforms or other transforms
    /// </summary>
    public class DatumTransform
    {
        private DatumShiftRec SrcDatum;
        private  DatumShiftRec TarDatum;
        private Ellipsoid SrcEllipsoid;
        private Ellipsoid TarEllipsoid;
        private Ellipsoid WGSEllipsoid;


        public static void NAD83toNAD27(double Lat83, double Lon83, double Elev83, out double Lat27, out double Lon27, out double Elev27)
        {
            Lat27 = double.MaxValue;
            Lon27 = double.MaxValue;
            Elev27 = double.MaxValue;
            DatumShiftRec D83 = DatumShiftInfo.Get("North American 1983 (NAD 83).Conus");
            DatumShiftRec D27 = DatumShiftInfo.Get("North American 1927 (NAD 27).Conus");
            DatumTransform T = new DatumTransform(D83, D27);
            T.Forward(Lat83, Lon83, Elev83, out Lat83, out Lon83, out Elev27);
        }

        public static void NAD27toNAD83(double Lat27, double Lon27, double Elev27, out double Lat83, out double Lon83, out double Elev83)
        {
            Lat83 = double.MaxValue;
            Lon83 = double.MaxValue;
            Elev83 = double.MaxValue;
            DatumShiftRec D27 = DatumShiftInfo.Get("North American 1927 (NAD 27).Conus");
            DatumShiftRec D83 = DatumShiftInfo.Get("North American 1983 (NAD 83).Conus");
            DatumTransform T = new DatumTransform(D27, D83);
            T.Forward(Lat27, Lon27, Elev27, out Lat83, out Lon83, out Elev83);
        }



        public DatumTransform(DatumShiftRec SrcDatum, DatumShiftRec TarDatum) 
        {
            this.SrcDatum = SrcDatum;
            this.TarDatum = TarDatum;
            SrcEllipsoid = Ellipsoids.Get(SrcDatum.Ellipsoid);
            TarEllipsoid = Ellipsoids.Get(TarDatum.Ellipsoid);
            WGSEllipsoid = Ellipsoids.Get("WGS84");
            if (SrcEllipsoid == null)
                throw new Exception("The ellipsoid for the source datum could not be found.");
            if (TarEllipsoid == null)
                throw new Exception("The ellipsoid for the target datum could not be found.");
        }

        public void Forward(double InLat, double InLon, double InHeight, out double OutLat, out double OutLon, out double OutHeight)
        {
            
            //Convert to WGS-84
            Molodensky ToWGS = new Molodensky(SrcDatum.DeltaX, SrcDatum.DeltaY, SrcDatum.DeltaZ, SrcEllipsoid, WGSEllipsoid);
            double WGSLat = double.MaxValue;
            double WGSLon = double.MaxValue;
            double WGSHeight = double.MaxValue;
            ToWGS.Convert(InLat, InLon, InHeight, out WGSLat, out WGSLon, out WGSHeight);
            if (TarDatum.LongName == "WGS 1984.Nonspecific")
            {
                OutLat = WGSLat;
                OutLon = WGSLon;
                OutHeight = WGSHeight;
                return;
            }
            else
            {
                //Convert from WGS-84 to target units
                Molodensky ToTar = new Molodensky(TarDatum.DeltaX, TarDatum.DeltaY, TarDatum.DeltaZ, WGSEllipsoid, TarEllipsoid, true);
                ToTar.Convert(WGSLat, WGSLon, WGSHeight, out OutLat, out OutLon, out OutHeight);
            }
        }
    }


    /// <summary>
    /// Molodensky operations for datum transformation
    /// </summary>
    public class Molodensky
    {
        private double DX;  //Delta X, given
        private double DY;  //Delta Y, given
        private double DZ;  //Delta Z, given
        private double DF;  //Delta flattening
        private double DA;  //Delta height, given
        private double SA;  //Source a radius
        private double SF;  //Source flattening
        private double TA;  //Target a radius
        private double TF;  //Target flattening
        private double SE2; //Source eccentricity squared


        public Molodensky(double DeltaX, double DeltaY, double DeltaZ, Ellipsoid Source, Ellipsoid Target, bool FlipSigns = false)
        {
            Setup(DeltaX, DeltaY, DeltaZ, Source.A, Source.B, Target.A, Target.B, FlipSigns);
        }

        private void Setup(double DeltaX, double DeltaY, double DeltaZ,
                          double SourceA, double SourceB,
                          double TargetA, double TargetB,
                          bool FlipSigns)
        {
            DX = DeltaX;
            DY = DeltaY;
            DZ = DeltaZ;
            DA = TargetA - SourceA;
            if (FlipSigns)
            {
                DX = Utils.FlipSigns(DX);
                DY = Utils.FlipSigns(DY);
                DZ = Utils.FlipSigns(DZ);
            }
            SA = SourceA;
            SF = Ellipsoids.CalcFlattening(SourceA, SourceB);

            TA = TargetA;
            TF = Ellipsoids.CalcFlattening(TargetA, TargetB);

            DF = TF - SF;
            SE2 = Ellipsoids.CalcE2(SourceA, SourceB);

        }


        public Molodensky(double DeltaX, double DeltaY, double DeltaZ,
                          double SourceA, double SourceB,
                          double TargetA, double TargetB,
                          bool FlipSigns = false)
        {

            Setup(DeltaX, DeltaY, DeltaZ, SourceA, SourceB, TargetA, TargetB, FlipSigns);
        }


        public void Convert(double SrcLat, double SrcLon, double SrcHeight, out double OutLat, out double OutLon, out double OutHeight)
        {
            SrcLat = Utils.D2R(SrcLat);
            SrcLon = Utils.D2R(SrcLon);

            double Slat = Math.Sin(SrcLat);
            double Clat = Math.Cos(SrcLat);
            double Slon = Math.Sin(SrcLon);
            double Clon = Math.Cos(SrcLon);
            double Ssqlat = Slat * Slat;
            double Div = 1.0 / (1.0 - SF);

            double Rn = SA / Math.Sqrt(1.0 - SE2 * Ssqlat);
            double Rm = SA * (1.0 - SE2) / Math.Pow((1.0 - SE2 * Ssqlat), 1.5);

            double Dlat = (((((-DX * Slat * Clon - DY * Slat * Slon) + DZ * Clat)
                        + (DA * ((Rn * SE2 * Slat * Clat) / SA)))
                        + (DF * (Rm * Div + Rn / Div) * Slat * Clat)))
                        / (Rm + SrcHeight);

            double Dlon = (-DX * Slon + DY * Clon) / ((Rn + SrcHeight) * Clat);

            double Dh = (DX * Clat * Clon) + (DY * Clat * Slon) + (DZ * Slat)
                 - (DA * (SA / Rn)) + ((DF * Rn * Ssqlat) / Div);

            OutLat = Utils.R2D(SrcLat + Dlat);
            OutLon = Utils.R2D(SrcLon + Dlon);
            OutHeight = SrcHeight + Dh;
        }

    }


    //I am not sure this works.  The one above seems to work better.
    //public class NAD27Tranform
    //{

    //    /// <summary>
    //    /// Converts from NAD27 to WGS84 using regression (MRE) method
    //    /// Extremely accurate, but only does NAD27 to WGS84
    //    /// </summary>
    //    /// <param name="NADLat">Input latitude in NAD 27 (South negative)</param>
    //    /// <param name="NADLon">Input longitude in NAD 27 (West negative)</param>
    //    /// <param name="WGSLat">Output latitude in WGS 84</param>
    //    /// <param name="WGSLon">Output longitude in WGS 84</param>
    //    public static void NAD27toWGS84(double NADLat, double NADLon, out double WGSLat, out double WGSLon)
    //    {
    //        const double K = 0.05235988;
    //        double U = K * (NADLat - 37.0);
    //        double V = K * (NADLon + 95.0);

    //        double U2 = U * U;
    //        double U3 = U2 * U;
    //        double U4 = U3 * U;
    //        double U5 = U4 * U;
    //        double U6 = U5 * U;
    //        double U7 = U6 * U;
    //        double U8 = U7 * U;
    //        double U9 = U8 * U;

    //        double V2 = V * V;
    //        double V3 = V2 * V;
    //        double V4 = V3 * V;
    //        double V5 = V4 * V;
    //        double V6 = V5 * V;
    //        double V7 = V6 * V;
    //        double V8 = V7 * V;
    //        double V9 = V8 * V;

    //        //Need to check these formulae to make sure I didnt do something wrong
    //        double DeltaLat = 0.16984 - 0.76173 * U + 0.09585 * V + 1.09919 * U2 - 4.57801 * U3 - 1.13239 * U2 * V
    //                          + 0.49831 * V3 - 0.98399 * U3 * V + 0.12415 * U * V3 + 0.11450 * V4 + 27.05396 * U5
    //                          + 2.03449 * U4 * V + 0.73357 * U2 * V3 - 0.37548 * V5 - 0.14197 * V6 - 59.96555 * U7
    //                          + 0.07439 * V7 - 4.76082 * U8 + 0.03385 * V8 + 49.04320 * U9 - 1.30575 * U6 * V3
    //                          - 0.07653 * U3 * V9 + 0.08646 * U4 * V9;
    //        DeltaLat = DeltaLat / 3600.0;

    //        double DeltaLon = -0.88437 + 2.05061 * V + 0.26361 * U2 - 0.76804 * U * V + 0.13374 * V2 - 1.31974 * U3
    //                            - 0.52162 * U2 * V - 1.05853 * U * V2 - 0.49211 * U2 * V2 + 2.17204 * U * V3 - 0.06004 * V4
    //                            + 0.30139 * U4 * V + 1.88585 * U * V4 - 0.81162 * U * V5 - 0.05183 * V6 - 0.96723 * U * V6
    //                            - 0.12948 * U3 * V5 + 3.41827 * U9 - 0.44507 * U8 * V + 0.18882 * U * V8 - 0.01444 * V9
    //                            + 0.04794 * U * V9 - 0.59013 * U9 * V3;
    //        DeltaLon = DeltaLon / 3600.0;


    //        double LatSign = Utility.Sign(NADLat);
    //        WGSLat = Math.Abs(NADLat) + DeltaLat;
    //        WGSLat = WGSLat * LatSign;
    //        WGSLat += DeltaLat;

    //        double LonSign = Utility.Sign(NADLon);
    //        WGSLon = Math.Abs(NADLon) + DeltaLon;
    //        WGSLon = WGSLon * LonSign;
    //        WGSLon += DeltaLon;

    //    }

    //}
}
