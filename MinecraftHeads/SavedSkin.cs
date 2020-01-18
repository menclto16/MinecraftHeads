using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftHeads
{
    public class SavedSkin
    {
        public Bitmap Bitmap { get; set; }
        public string SkinName { get; set; }

        public SavedSkin(Bitmap bitmap, string skinName)
        {
            Bitmap = bitmap;
            SkinName = skinName;
        }
    }
}
