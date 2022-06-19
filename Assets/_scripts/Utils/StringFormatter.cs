

namespace Utils
{
    public class StringFormatter
    {
        public static string ArrayToString(string[] strArr)
        {
            string result = "{";
            for (int i = 0; i < strArr.Length - 1; i++)
            {
                result += strArr[i] + ", ";
            }
            result += strArr[strArr.Length - 1] + "}";

            return result;
        }

        public static char StringToChar(string str)
        {
            return str[0];
        }
    }
}

