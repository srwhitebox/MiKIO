using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace XiMPLib.xType {
    public class xcTrimming {
        public static StringTrimming toTrimming(string trimming) {
            switch (trimming) {
                case"character":
                case"char":
                    return StringTrimming.Character;
                case "ellipsis":
                case "...":
                case "ellipsisechar":
                case "ellipsise_char":
                case "ellipsisecharacter":
                case "ellipsise_character":
                    return StringTrimming.EllipsisCharacter;
                case "ellipsispath":
                case "ellipsis_path":
                    return StringTrimming.EllipsisPath;
                case "ellipsisword":
                case "ellipsis_word":
                    return StringTrimming.EllipsisWord;
                case "word":
                    return StringTrimming.Word;
                default:
                    return StringTrimming.None;
            }
        }
    }
}
