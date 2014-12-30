using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextHarbor
{
    static public class Extensions
    {
        static public List<Block> GetBlocks(this string text)
        {
            // splitting is not needed, we could 
            // send whole data in one block.

            var blocks = new List<Block>();

            int prevDotPos = 0;
            int previousBlockId = 0;
            for (int i = 0; i < text.Length; ++i)
            {
                var c = text[i];
                if (c == '.')
                {
                    blocks.Add(new Block
                    {
                        Id = previousBlockId++,
                        HarborData = new HarborData
                        {
                            BlockIndex = prevDotPos,
                            BlockLength = i - prevDotPos + 1
                        },
                        Text = text.Substring(prevDotPos, i - prevDotPos + 1)
                    });
                    prevDotPos = i+1;
                }
            }
            if (text.Length - prevDotPos > 1)
            {
                blocks.Add(new Block
                {
                    HarborData = new HarborData
                    {
                        BlockIndex = prevDotPos,
                        BlockLength = text.Length - prevDotPos
                    },
                    Text = text.Substring(prevDotPos)
                });
            }

            return blocks;
        }
    }
}