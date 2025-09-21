using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace api.Services
{
    public static class PasswordGenerator
    {
        private const string Lower = "abcdefghijklmnopqrstuvwxyz";
        private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digit = "0123456789";
        // Chọn tập ký tự đặc biệt “an toàn URL/email”
        private const string Special = "@$!%*?&-_#";

        public static string Generate(int length = 12)
        {
            if (length < 8) length = 8;

            var req = new[]
            {
            RandomChar(Lower),
            RandomChar(Upper),
            RandomChar(Digit),
            RandomChar(Special)
        };

            var all = Lower + Upper + Digit + Special;
            var rest = new char[length - req.Length];
            for (int i = 0; i < rest.Length; i++)
                rest[i] = RandomChar(all);

            // Trộn đều
            var result = req.Concat(rest).ToArray();
            Shuffle(result);
            return new string(result);
        }

        private static char RandomChar(string set)
        {
            var idx = RandomNumberGenerator.GetInt32(set.Length);
            return set[idx];
        }

        private static void Shuffle(char[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
    }
}