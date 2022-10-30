using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIAHScrape.Entities
{
    public class Servers
    {
        public static IDictionary<string, int> Info = new Dictionary<string, int>()
        {
            {"Bahamut", 1},
            {"Shiva", 2},
            {"Phoenix", 5},
            {"Carbuncle", 6},
            {"Fenrir", 7},
            {"Sylph", 8},
            {"Valefor", 9},
            {"Leviathan", 11},
            {"Odin", 12},
            {"Quetzalcoatl", 16},
            {"Siren", 17},
            {"Ragnarok", 20},
            {"Cerberus", 23},
            {"Bismarck", 25},
            {"Lakshmi", 27},
            {"Asura", 28}
        };
    }
}