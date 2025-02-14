﻿using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("VINlibr.test")]

namespace VINlib
{
    //Vehicle identification number
    public static class VIN
    {
        internal static int NumberEquivalent(char c)
        {
            c = Char.ToUpper(c);
            if (c >= 65 && c <= 90)
            {
                if (c >= 83) ++c;//s == 2, but why
                return (c - 65) % 9 + 1;
            }
            else if (c >= 48 && c <= 57) return c - 48;
            else throw new ArgumentException("Invalid sign of VIN", nameof(c));
        }

        internal static int IndexWeight(int index)
        {
            if (index >= 0 && index <= 6) return 8 - index;
            else if (index == 7) return 10;
            else if (index == 8) return 0;//sum
            else if (index >= 9 && index <= 16) return 9 - index % 9;
            else throw new IndexOutOfRangeException($"The index must be between 0 and 16, but it is {index}");
        }

        internal static bool ValidityCheck(string identNumber)
        {
            Regex regex = new Regex("^[A-HJ-NPR-Z0-9]{17}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return regex.IsMatch(identNumber);
        }

        //returns true if verification is seccessful
        public static bool ChecksumVerification(string identNumber)
        {
            if (ValidityCheck(identNumber))
            {
                if (identNumber[8] != 'x' && identNumber[8] != 'X' && !(identNumber[8] >= 48 && identNumber[8] <= 57)) return false;
                int checkSum = 0;
                for (int i = 0; i < identNumber.Length; ++i)
                    checkSum += NumberEquivalent(identNumber[i]) * IndexWeight(i);
                int expectedDifference = identNumber[8] == 'X' || identNumber[8] == 'x' ? 10 : NumberEquivalent(identNumber[8]);

                if (checkSum - checkSum / 11 * 11 == expectedDifference) return true;
            }
            return false;
        }

        static public VINinfo DecodeVIN(string identNumber)
        {
            if(ChecksumVerification(identNumber)) return new VINinfo(identNumber);
            throw new InvalidOperationException($"VIN {identNumber} doesnt exist");
        }
    }

    readonly public struct VINinfo
    {
        readonly public string identNumber;
        readonly public string geographicalArea;
        readonly public string country;
        readonly public bool isLargeManufacturer;
        readonly public int productionYear;

        public VINinfo(string identNumber)
        {
            this.identNumber = identNumber.ToUpper();
            //WMI
            geographicalArea = GetGeographicalArea(identNumber[0]);
            country = GetCountry(identNumber[0].ToString() + identNumber[1].ToString());
            isLargeManufacturer = identNumber[2] != 9;
            //VIS
            productionYear = GetProductionYear(identNumber[9]);
        }

        internal static string GetGeographicalArea(char c)
        {
            if (c >= 'A' && c <= 'Z' || c >= '0' && c <= '9')
            {
                char[] startRange = { 'A', 'J', 'S', '1', '6', '8' };
                char[] endRange = { 'H', 'R', 'Z', '5', '7', '9' };
                string[] areas = { "Африка", "Азия", "Европа", "Северная Америка", "Океания", "Южная Америка" };
                for (int i = 0; i < 6; ++i)
                    if (c >= startRange[i] && c <= endRange[i]) return areas[i];
            }
            throw new IndexOutOfRangeException($"Acceptable symbols: A-Z, 0-9. '{c}' is not included.");
        }

        internal static bool CharIsIncluded(char c, char start, char end, int num = 0)
        {//is number
            if (c == '0') c = (char)100;
            else if (c <= 57) c += (char)42;//numbers now - 91:100, letters - 65:90
            if (start == '0') start = (char)100;
            else if (start <= 57) start += (char)42;
            if (end == '0') end = (char)100;
            else if (end <= 57) end += (char)42;

            if (c >= start && c <= end) return true;
            else if (c >= end && c <= 100) return false;
            throw new IndexOutOfRangeException($"'{c}' don't using to WMI {num} position");
        }

        internal static string GetCountry(string cc)
        {
            string[] startRange = {"AA", "AJ",    "BA",   "BF",   "BL",   "CA",   "CF",   "CL",   "DA",   "DF",   "DL",   "EA",
                    "EF",   "FA",   "FF",   "JA",   "KA",   "KF",   "KL",   "KS",   "LA",   "MA",   "MF",   "ML",   "MS",   "NA",
                    "NF",   "NL",   "PF",   "PL",   "RA",   "RL",   "RS",   "SA",   "SN",   "SU",   "S1",   "TA",   "TJ",   "TR",
                    "TW",   "UU",   "U5",   "V3",   "V6",   "WA",   "XF",   "XS",   "XX",   "X3",   "YA",   "YF",   "YL",   "YS",
                    "YX",   "Y3",   "Y6",   "ZA",   "ZX",   "Z3",   "Z6",   "1A",   "2A",   "3A",   "3X",   "38",   "4A",   "5A",
                    "6A",   "7A",   "8A",   "8F",   "8L",   "8S",   "8X",   "9A",   "9F",   "9L",   "9S",   "9X",   "93"};
            string[] endRange = {"AH", "AN",  "BE",   "BK",   "BR",   "CE",   "CK",   "CR",   "DE",   "DK",   "DR",   "EE",   "EK",
                    "FE",   "FK",   "JT",   "KE",   "KK",   "KR",   "K0",   "L0",   "ME",   "MK",   "MR",   "M0",   "NE",   "NK",
                    "NR",   "PK",   "PR",   "RE",   "RR",   "R0",   "SM",   "ST",   "SZ",   "S4",   "TH",   "TP",   "TV",   "T1",
                    "UZ",   "U7",   "V5",   "V0",   "W0",   "XK",   "XW",   "X2",   "X0",   "YE",   "YK",   "YR",   "YW",   "Y2",
                    "Y5",   "Y0",   "ZR",   "Z2",   "Z5",   "Z0",   "10",   "20",   "3W",   "37",   "30",   "40",   "50",   "6W",
                    "7E",   "8E",   "8K",   "8R",   "8W",   "82",   "9E",   "9K",   "9R",   "9W",   "92",   "99"};
            string[] countries = {"ЮАР", "Кот-д’Ивуар",  "Ангола",   "Кения",    "Танзания", "Бенин",    "Мадагаскар",   "Тунис",
                    "Египет",   "Марокко",  "Замбия",   "Эфиопия",  "Мозамбик", "Гана", "Нигерия",  "Япония",   "Шри Ланка",
                    "Израиль",  "Южная Корея",  "Казахстан",    "Китай",    "Индия",    "Индонезия",    "Таиланд",  "Мьянма",
                    "Иран", "Пакистан", "Турция",   "Сингапур", "Малайзия", "ОАЭ",  "Вьетнам",  "Саудовская Аравия",
                    "Великобритания",   "Восточная Германия",   "Польша",   "Латвия",   "Швейцария",    "Чехия",    "Венгрия",
                    "Португалия",   "Румыния",  "Словакия", "Хорватия", "Эстония",  "Германия", "Греция",   "СССР/СНГ",
                    "Люксембург",   "Россия",   "Бельгия",  "Финляндия",    "Мальта",   "Швеция",   "Норвегия", "Беларусь",
                    "Украина",  "Италия",   "Словения", "Литва",    "Россия",   "США",  "Канада",   "Мексика",  "Коста Рика",
                    "Каймановы острова",    "США",  "США",  "Австралия",    "Новая Зеландия",   "Аргентина",    "Чили",
                    "Эквадор",  "Перу", "Венесуэла",    "Бразилия", "Колумбия", "Парагвай", "Уругвай",  "Тринидад и Тобаго",
                    "Бразилия"};
            for (int i = 0; i < countries.Length; ++i)
            {
                if (CharIsIncluded(cc[0], startRange[i][0], endRange[i][0], 0))
                    if (CharIsIncluded(cc[1], startRange[i][1], endRange[i][1], 1)) return countries[i];
            }
            throw new IndexOutOfRangeException($"'{cc}' don't using to WMI");
        }

        internal static int GetProductionYear(char c)
        {
            Regex regex = new Regex("^[A-HJ-NPR-TV-Y1-9]{1}$", RegexOptions.Compiled);
            if (regex.IsMatch(c.ToString()))
            {
                string yearChars = "Y123456789ABCDEFGHJKLMNPRSTVWX";
                Dictionary<char, int> CharsAndYears = new Dictionary<char, int>();
                int cur_year = 2000;
                foreach (char yearChar in yearChars)
                {
                    CharsAndYears[yearChar] = cur_year;
                    ++cur_year;
                }
                return CharsAndYears[c];
            }
            else throw new IndexOutOfRangeException($"'{c}'should be in A-Y, 1-9");
        }

        public override string ToString() =>
                $"VIN: {identNumber}\n"
                + $"WMI: {identNumber.Substring(0, 3)}\n"
                + $"VDS: {identNumber.Substring(3, 6)}\n"
                + $"VIS: {identNumber.Substring(9)}\n\n"
                + $"Country: {country}\n"
                + $"Geographical area: {geographicalArea}\n"
                + $"Production year: ({productionYear - 30}|{productionYear}|{productionYear + 30})\n"
                + "Manufacturer produces "
                + (isLargeManufacturer ? "more" : "less")
                + " than 500 road vehicles per year\n";
        
    }
}