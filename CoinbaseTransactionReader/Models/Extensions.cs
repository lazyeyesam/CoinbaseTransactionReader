using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace CoinbaseTransactionReader.Models
{
    public static class Extensions
    {
        public static List<string[]> ReadAsList(this IFormFile file)
        {
            var regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            var list = new List<string[]>();
            using var reader = new StreamReader(file.OpenReadStream());
            while (reader.Peek() >= 0)
            {
                var line = reader.ReadLine();
                if (line != null)
                    list.Add(regex.Split(line));
            }
            return list;
        }

        public static string GetDescription(this Enum value)
        {
            var genericEnumType = value.GetType();
            var memberInfo = genericEnumType.GetMember(value.ToString());
            if (memberInfo.Length <= 0) return value.ToString();

            var attributes = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            return attributes.Any() ? ((System.ComponentModel.DescriptionAttribute)attributes.ElementAt(0)).Description : value.ToString();
        }

    }
}
