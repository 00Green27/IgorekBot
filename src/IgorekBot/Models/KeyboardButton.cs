using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IgorekBot.Models
{
    [Serializable]
    public class KeyboardButton
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
}