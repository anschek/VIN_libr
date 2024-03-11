
using System;

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
                try
                {
                    VIN.IndexWeight(index);
                    Assert.Fail($"index [{index}] should throw an exception");
                }
                catch (IndexOutOfRangeException) { }
                catch (Exception) { Assert.Fail($"index [{index}] should throw an IndexOutOfRangeException"); }
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
            Assert.AreEqual("Африка", VIN.WMIinfo.GetGeographicalArea('a'));
            Assert.AreEqual("Африка", VIN.WMIinfo.GetGeographicalArea('C'));
            Assert.AreEqual("Азия", VIN.WMIinfo.GetGeographicalArea('o'));
            Assert.AreEqual("Азия", VIN.WMIinfo.GetGeographicalArea('l'));
            Assert.AreEqual("Океания", VIN.WMIinfo.GetGeographicalArea('6'));
            Assert.AreEqual("Южная Америка", VIN.WMIinfo.GetGeographicalArea('9'));

        }

        [TestMethod]
        public void TestMethod_GetGeographicalArea_SymbolNotAcceptable()
        {
            char[] ExceptionThrowingSymbols = new char[] { 'ф', '}', '/', '%', (char)47, (char)91 };
            foreach (char c in ExceptionThrowingSymbols)
            {
                try
                {
                    Assert.AreEqual("", VIN.WMIinfo.GetGeographicalArea(c));
                    Assert.Fail($"Symbol '{c}' should throw an exception");
                }
                catch (IndexOutOfRangeException) { }
                catch (Exception) { Assert.Fail($"Symbol '{c}' should throw an IndexOutOfRangeException"); }
            }
        }

        [TestMethod]
        public void TestMethod_CharIsIncluded_CharIsOrWillBeIncluded()
        {
            (char, char, char)[] CharsIsIncluded = new (char, char, char)[] {
                ('A','A','A'), ('A','B','A'),('A','B','B'), ('A','C','B'),
                ('A','9','0'), ('A','9','9'),('A','9','Z'), ('Y','9','Z')};
            (char, char, char)[] CharsWillBeIncluded = new (char, char, char)[] {
                ('A','A','B'), ('A','B','C'),('A','Z','0'), ('A','0','1'),
                ('0','3','9')};
            foreach ((char start, char end, char c) in CharsIsIncluded)
                Assert.AreEqual(true, VIN.WMIinfo.CharIsIncluded(c, start, end));
            foreach ((char start, char end, char c) in CharsWillBeIncluded)
                Assert.AreEqual(false, VIN.WMIinfo.CharIsIncluded(c, start, end));
        }

        [TestMethod]
        public void TestMethod_CharIsIncluded_CharCanNoBeFound()
        {
            (char, char, char)[] CharsCanNoBeFound = new (char, char, char)[] {
                ('A','B',(char)('A'-1)),('B','Z','A'),('0','9','Z'),
                ('3','9','0'), ('A','9',(char)('9'+1))};
            foreach ((char start, char end, char c) in CharsCanNoBeFound)
            {
                try
                {
                    Assert.AreEqual("", VIN.WMIinfo.CharIsIncluded(c, start, end));
                    Assert.Fail($"Symbol '{c}' cannot be found in ('{start}', '{end})");
                }
                catch (IndexOutOfRangeException) { }
                catch (Exception) { Assert.Fail($"Symbol '{c}' should throw an IndexOutOfRangeException " +
                    $"when going beyond the boundaries of the sequence ('{start}', '{end})" ); }
            }
        }
        //GetCountry
    }
}