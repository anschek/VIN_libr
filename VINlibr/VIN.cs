namespace VINlib
{
    //Vehicle identification number
    public class VIN
    {
        public static int NumberEquivalent(char c)
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

        public static int IndexWeight(int index)
        {
            if (index >= 0 && index <= 6) return 8 - index;
            else if (index == 7) return 10;
            else if (index == 8) return 0;//sum
            else if (index >= 9 && index <= 16) return 9 - index % 9;
            else throw new IndexOutOfRangeException($"The index must be between 0 and 16, but it is {index}");
        }

        //returns true if verification is seccessful
        static public bool ChecksumVerification(string identNumber)
        {
            if (identNumber.Length == 17)
            {

            }
            return false;
        }

    }
}