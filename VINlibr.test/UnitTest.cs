
using System;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace VINlib.test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod_NumberEquivalent_CheckLetters()
        {
            Assert.AreEqual(1, VIN.NumberEquivalent('A'));
            Assert.AreEqual(1, VIN.NumberEquivalent('a'));

            Assert.AreEqual(8, VIN.NumberEquivalent('H'));
            Assert.AreEqual(8, VIN.NumberEquivalent('h'));

            Assert.AreEqual(2, VIN.NumberEquivalent('S'));
            Assert.AreEqual(2, VIN.NumberEquivalent('s'));

            Assert.AreEqual(9, VIN.NumberEquivalent('Z'));
            Assert.AreEqual(9, VIN.NumberEquivalent('z'));
        }

        [TestMethod]
        public void TestMethod_NumberEquivalent_CheckNumbers()
        {
            Assert.AreEqual(0, VIN.NumberEquivalent('0'));
            Assert.AreEqual(5, VIN.NumberEquivalent('5'));
            Assert.AreEqual(9, VIN.NumberEquivalent('9'));
        }

        [TestMethod]
        public void TestMethod_NumberEquivalent_CheckOtherSymbols()
        {
            char[] ExceptionThrowingSymbols = new char[] { ' ', '/', '&', '?', 'ф', 'Ф' };
            foreach (char c in ExceptionThrowingSymbols)
            {
                try
                {
                    VIN.NumberEquivalent(c);
                    Assert.Fail($"symbol '{c}' should throw an exception");
                }
                catch (ArgumentException) { }
                catch (Exception) { Assert.Fail($"symbol '{c}' should throw an ArgumentException"); }
            }
        }

        [TestMethod]
        public void TestMethod_IndexWeight_CheckAcceptableIndexes()
        {
            //indexes 0-6
            Assert.AreEqual(8, VIN.IndexWeight(0));
            Assert.AreEqual(7, VIN.IndexWeight(1));
            Assert.AreEqual(2, VIN.IndexWeight(6));
            //for index 7
            Assert.AreEqual(10, VIN.IndexWeight(7));
            //for sum
            Assert.AreEqual(0, VIN.IndexWeight(8));
            //indexes 9-16
            Assert.AreEqual(9, VIN.IndexWeight(9));
            Assert.AreEqual(8, VIN.IndexWeight(10));
            Assert.AreEqual(7, VIN.IndexWeight(11));
            Assert.AreEqual(8, VIN.IndexWeight(10));
            Assert.AreEqual(2, VIN.IndexWeight(16));
        }

        [TestMethod]
        public void TestMethod_IndexWeight_CheckUnacceptableIndexes()
        {
            int[] ExceptionThrowingIndexes = new int[] { -10, -1, 17, 20 };
            foreach (int index in ExceptionThrowingIndexes)
            {
                Action action = () => VIN.IndexWeight(index); 
                Assert.ThrowsException<IndexOutOfRangeException>(action);
            }
        }

        [TestMethod]
        public void TestMethod_ValidityCheck_CodeIsValid()
        {
            Assert.AreEqual(true, VIN.ValidityCheck("aaaaaaaaaaaaaaaaa"));
            Assert.AreEqual(true, VIN.ValidityCheck("AAAAAAAAAAAAAAAAA"));
            Assert.AreEqual(true, VIN.ValidityCheck("AAAAAA92AAAAbAA78"));
            Assert.AreEqual(true, VIN.ValidityCheck("12312312312312312"));
        }
        
        [TestMethod]
        public void TestMethod_ValidityCheck_CodeIsInvalid()
        {
            Assert.AreEqual(false, VIN.ValidityCheck("aaaa"));
            Assert.AreEqual(false, VIN.ValidityCheck("AAAAAAAAAAAAAAA/A"));
            Assert.AreEqual(false, VIN.ValidityCheck("AAAAAA92AAAAbAQIO"));
            Assert.AreEqual(false, VIN.ValidityCheck("12312312312312312AK"));
        }

        [TestMethod]
        public void TestMethod_ChecksumVerification_VINexist()
        {
            Assert.AreEqual(true, VIN.ChecksumVerification("JHMCM56557C404453"));
            Assert.AreEqual(true, VIN.ChecksumVerification("WBAGB330402182616"));
        }

        [TestMethod]
        public void TestMethod_ChecksumVerification_VINdoesNotExist()
        {
            Assert.AreEqual(false, VIN.ChecksumVerification("WAUZZZ44ZEN096063"));
            Assert.AreEqual(false, VIN.ChecksumVerification("WVWDB4505LK234567"));
        }

        [TestMethod]
        public void TestMethod_GetGeographicalArea_ExpectedEqualsResult()
        {
            Assert.AreEqual("Африка", VINinfo.GetGeographicalArea('A'));
            Assert.AreEqual("Африка", VINinfo.GetGeographicalArea('C'));
            Assert.AreEqual("Азия", VINinfo.GetGeographicalArea('N'));
            Assert.AreEqual("Азия", VINinfo.GetGeographicalArea('L'));
            Assert.AreEqual("Океания", VINinfo.GetGeographicalArea('6'));
            Assert.AreEqual("Южная Америка", VINinfo.GetGeographicalArea('9'));

        }

        [TestMethod]
        public void TestMethod_GetGeographicalArea_SymbolNotAcceptable()
        {
            char[] ExceptionThrowingSymbols = new char[] { 'ф', '}', '/', '%', (char)47, (char)91 };
            foreach (char c in ExceptionThrowingSymbols)
            {
                Action action = () => VINinfo.GetGeographicalArea(c);
                Assert.ThrowsException<IndexOutOfRangeException>(action);
            }
        }

        [TestMethod]
        public void TestMethod_CharIsIncluded_CharIsOrWillBeIncluded()
        {
            (char, char, char)[] CharsIsIncluded = new (char, char, char)[] {
                ('A','A','A'), ('A','B','A'),('A','B','B'), ('A','C','B'),
                ('A','0','9'), ('A','9','9'),('A','9','Z'), ('Y','9','Z')};
            (char, char, char)[] CharsWillBeIncluded = new (char, char, char)[] {
                ('A','A','B'), ('A','B','C'),('A','Z','0'), ('A','8','0'),
                ('1','9','0')};
            foreach ((char start, char end, char c) in CharsIsIncluded)
                Assert.AreEqual(true, VINinfo.CharIsIncluded(c, start, end));
            foreach ((char start, char end, char c) in CharsWillBeIncluded)
                Assert.AreEqual(false, VINinfo.CharIsIncluded(c, start, end));
        }

        [TestMethod]
        public void TestMethod_CharIsIncluded_CharCanNoBeFound()
        {
            (char, char, char)[] CharsCanNoBeFound = new (char, char, char)[] {
                ('A','B',(char)('A'-1)),('B','Z','A'),('9','0','Z'),
                ('3','0','1'), ('A','0',(char)(2000))};
            foreach ((char start, char end, char c) in CharsCanNoBeFound)
            {
                Action action = () => VINinfo.CharIsIncluded(c, start, end);
                Assert.ThrowsException<IndexOutOfRangeException>(action);
            }
        }

        [TestMethod]
        public void TestMethod_GetCountry_ExpectedEqualsResult()
        {
            Assert.AreEqual("Кот-д’Ивуар", VINinfo.GetCountry("AJ"));
            Assert.AreEqual("Кот-д’Ивуар", VINinfo.GetCountry("AL"));
            Assert.AreEqual("Южная Корея", VINinfo.GetCountry("KR"));
            Assert.AreEqual("Южная Корея", VINinfo.GetCountry("KN"));
            Assert.AreEqual("США", VINinfo.GetCountry("4A"));
            Assert.AreEqual("США", VINinfo.GetCountry("59"));
            Assert.AreEqual("Австралия", VINinfo.GetCountry("6W"));
            Assert.AreEqual("Австралия", VINinfo.GetCountry("6C"));
            Assert.AreEqual("Бразилия", VINinfo.GetCountry("93"));
            Assert.AreEqual("Бразилия", VINinfo.GetCountry("96"));
        }

        [TestMethod]
        public void TestMethod_GetCountry_CountryNotFound()
        {
            string[] UnusedCodes = new string[] {"CS", "C9","C0", "EL","E8","E0","NT", "NY", 
                "N5","N0", "U8","U9", "U0","ZS", "ZU","ZW", "6X","6Z", "61","69","60", 
                "7F", "71", "79", "70", "83", "89", "80", "90" };
            foreach (string cc in UnusedCodes)
            {
                Action action = () => VINinfo.GetCountry(cc);
                Assert.ThrowsException<IndexOutOfRangeException>(action);
            }
        }        
        
        [TestMethod]
        public void TestMethod_GetProductionYear_ExpectedEqualsResult()
        {
            Assert.AreEqual(2010, VINinfo.GetProductionYear('A'));
            Assert.AreEqual(2012, VINinfo.GetProductionYear('C'));
            Assert.AreEqual(2018, VINinfo.GetProductionYear('J'));
            Assert.AreEqual(2000, VINinfo.GetProductionYear('Y'));
            Assert.AreEqual(2001, VINinfo.GetProductionYear('1'));
            Assert.AreEqual(2007, VINinfo.GetProductionYear('7'));
        }        
        
        [TestMethod]
        public void TestMethod_GetProductionYear_YearCharIsInvalid()
        {
            char[] UnusedChars = new char[] {'a','O','Q', 'I', 'U', 'Z', '0', '/', '$', 'а', 'А' };
            foreach (char c in UnusedChars)
            {
                Action action = () => VINinfo.GetProductionYear(c);
                Assert.ThrowsException<IndexOutOfRangeException>(action);
            }
        }        
        
        [TestMethod]
        public void TestMethod_VINandVINinfoConstructors_ExpectedEqualsResult()
        {
            string expectedIdentNumber = "JHMCM56557C404453";
            string expectedGeographicalArea = "Азия";
            string expectedCountry = "Япония";
            bool expectedIsLargeManufacturer = true;
            int expectedProductionYear = 2007;

            VINinfo vin_info = VIN.DecodeVIN("JHMCM56557C404453");
            Assert.AreEqual(expectedIdentNumber, vin_info.identNumber);
            Assert.AreEqual(expectedGeographicalArea, vin_info.geographicalArea);
            Assert.AreEqual(expectedCountry, vin_info.country);
            Assert.AreEqual(expectedIsLargeManufacturer, vin_info.isLargeManufacturer);
            Assert.AreEqual(expectedProductionYear, vin_info.productionYear);
        }

        [TestMethod]
        public void TestMethod_VINconstructor_IsDoesntExist()
        {
            string[] didnotPassChecksumVerification = new string[] { "JHMCM56557C404450", "AHMCM56557C40445F", "AHMCM5655XC404451",
            "JHMCM56557C40445Q", "JHMCM56557C4044509", "АHMCM5655XC404453" };

            foreach (string identNumber in didnotPassChecksumVerification)
            {
                Action action = () => VIN.DecodeVIN(identNumber);
                Assert.ThrowsException<InvalidOperationException>(action);
            }
        }

        [TestMethod]
        public void TestMethod_ToString_ExpectedEqualsResult()
        {
            VINinfo vin_info = VIN.DecodeVIN("JHMCM56557C404453");

            string expectedString = $"VIN: JHMCM56557C404453\n"
                + "WMI: JHM\n"
                + "VDS: CM5655\n"
                + "VIS: 7C404453\n\n"
                + "Country: Япония\n"
                + "Geographical area: Азия\n"
                + "Production year: (1977|2007|2037)\n"
                + "Manufacturer produces "
                + "more"
                + " than 500 road vehicles per year\n";

            Assert.AreEqual(expectedString, vin_info.ToString());
        }   
    }
}